using LS.Standard.Data;
using LS.WPF.Core.MVVM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LS.Flowchart.Components.ComponentModels;
using LS.Flowchart.Models.ProjectModel;
using static LS.Flowchart.Components.ComponentDelegates;
using static LS.Flowchart.ModuleControls.DeviceTag;

namespace LS.Flowchart.Components.NetWork
{
    public class TCPClient : IComponent
    {
        /// <summary>
        /// TCP客户端
        /// </summary>
        /// <param name="parameter"></param>
        public TCPClient(ProjectDevice device)
        {
            Device = device;
        }

        public ProjectDevice Device { get; }
        Parameter_TCPClient param;
        Parameter_TCPClient _parameter
        {
            get
            {
                if (param == null)
                {
                    if (Device != null)
                    {
                        string strParam = JsonConvert.SerializeObject(Device.DeviceParameter);
                        param = JsonConvert.DeserializeObject<Parameter_TCPClient>(strParam);
                    }
                }
                return param;
            }
        }
        public DeviceEnum DeviceType => Device.DeviceType;
        public string DeviceId => Device?.DeviceId;
        IPEndPoint endPoint;
        Socket _client = null;
        string logName => Device != null ? $"TCPClient_{Device.Name}" : "TCPClient";
        bool isrunThread = false;
        bool isConnect = false;
        readonly object _lock = new object();
        readonly object _lock_record = new object();
        /// <summary>
        /// 接收消息的记录
        /// </summary>
        public List<MessageModel> ResponseRecords { get; set; } = new List<MessageModel>();
        /// <summary>
        /// 刷新触发
        /// </summary>
        public event DelegateRefresh OnRefresh;
        /// <summary>
        /// 消息回调
        /// </summary>
        public event DelegateOnResponse OnResponse;
        /// <summary>
        /// 状态变更通知
        /// </summary>
        public event DelegateOnStateChange OnStateChange;

        /// <summary>
        /// 查询是否已启动
        /// </summary>
        /// <returns></returns>
        public bool IsSatrt()
        {
            return (_client != null && _client.Connected);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public BaseResult Start()
        {
            try
            {
                if (_client != null)
                {
                    Stop();
                }
                endPoint = new IPEndPoint(IPAddress.Parse(_parameter.TargetIP), _parameter.TargetPort);
                isrunThread = true;
                if (_parameter.AutoReconnection)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(5000);
                        int time = 0;
                        while (isrunThread)
                        {
                            if (time <= 0)
                            {
                                time = 20;
                                if (!isConnect)
                                {
                                    if (_client == null || !_client.Connected)
                                    {
                                        Rconnection();
                                    }
                                }
                            }
                            time -= 1;
                            Thread.Sleep(50);
                        }
                    });
                }
                Task.Run(() =>
                {
                    Thread.Sleep(500);
                    int time = 0;
                    while (isrunThread)
                    {
                        if (time <= 0)
                        {
                            time = 5;
                            if (isConnect)
                            {
                                if (_client != null && _client.Connected)
                                {
                                    Response();
                                }
                            }
                        }
                        time -= 1;
                        Thread.Sleep(100);
                    }
                });
                return Connect();
            }
            catch (Exception ex)
            {
                LogOperate.General(logName, $"[{Device.Name}]启动TCPClient异常");
                LogOperate.Error(logName, ex);
                return new BaseResult(false, ex.Message);
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public BaseResult Stop()
        {
            try
            {
                isrunThread = false;
                return Close();
            }
            catch (Exception ex)
            {
                LogOperate.General(logName, $"[{Device.Name}]停止TCPClient异常");
                LogOperate.Error(logName, ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 清空记录的消息内容
        /// </summary>
        /// <returns></returns>
        public BaseResult ClearRecord()
        {
            lock (_lock_record)
            {
                ResponseRecords.Clear();
            }
            OnRefresh?.Invoke(true);
            return BaseResult.Successed;
        }

        private string IP
        {
            get
            {
                if (endPoint != null)
                {
                    return $"{endPoint.Address.ToString()}:{endPoint.Port}";
                }
                return "";
            }
        }


        /// <summary>
        /// 连接 
        /// </summary>
        /// <param name="timeout">连接超时时间</param>
        /// <returns></returns>
        private BaseResult Connect(int timeout = 3000, int receivetimeout = 6000)
        {
            return Connect(endPoint, timeout, receivetimeout);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="timeout">连接超时时间</param>
        /// <returns></returns>
        private BaseResult Connect(IPEndPoint ip, int timeout = 3000, int receivetimeout = 500)
        {
            try
            {
                endPoint = ip;
                if (endPoint == null || string.IsNullOrEmpty(endPoint.Address.ToString()))
                    return new BaseResult(false, "IP 不能为空");
                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                    _client = null;
                }

                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _client.SendTimeout = timeout;
                _client.ReceiveTimeout = receivetimeout;
                _client.ExclusiveAddressUse = false;
                _client.Connect(endPoint);

                if (_client.Connected)
                {
                    isConnect = true;
                    OnStateChange?.Invoke(this, true);
                    return new BaseResult(true, "连接成功");
                }
                else
                {
                    isConnect = false;
                    OnStateChange?.Invoke(this, false);
                    return new BaseResult(false, "连接失败");
                }
            }
            catch (Exception ex)
            {
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public BaseResult Close()
        {
            try
            {
                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                    _client = null;
                }
                isConnect = false;
                OnStateChange?.Invoke(this, false);
            }
            catch (Exception ex)
            {
                LogOperate.Error("TCP关闭连接异常", ex);
            }
            return BaseResult.Successed;
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        public BaseResult Send(string content, bool isHEX = false)
        {
            string id = Guid.NewGuid().ToString().Replace("-", "");
            try
            {
                if (_client == null || !_client.Connected)
                {
                    isConnect = false;
                    return new BaseResult(false, "未启动连接，无法发送数据");
                }

                lock (_lock)
                {
                    int resIndex = 0;
                    if (isHEX)
                    {
                        string hexData = content.Replace(" ", ""); // 去除空格
                        byte[] send = new byte[hexData.Length / 2];

                        // 每两个字符解析为一个字节
                        for (int i = 0; i < hexData.Length; i += 2)
                        {
                            string hexByte = hexData.Substring(i, 2);
                            send[i / 2] = Convert.ToByte(hexByte, 16); // 16表示十六进制基数
                        }
                        LogOperate.General(logName, $"[{id}][{IP}] --> {content}");
                        resIndex = _client.Send(send, send.Length, SocketFlags.None);
                        if (resIndex > 0)
                        {
                            return BaseResult.Successed;
                        }
                        else
                        {
                            return new BaseResult(false, "发送失败");
                        }
                    }
                    else
                    {
                        byte[] send = Encoding.ASCII.GetBytes(content);
                        byte[] data = new byte[1024];

                        LogOperate.General(logName, $"[{id}][{IP}] --> {content}");
                        resIndex = _client.Send(send, send.Length, SocketFlags.None);
                        if (resIndex > 0)
                        {
                            return BaseResult.Successed;
                        }
                        else
                        {
                            return new BaseResult(false, "发送失败");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                LogOperate.General(logName, $"[{id}][{IP}]  <--  发送失败,{msg}");
                Close();
                isConnect = false;
                return new BaseResult(false, "发送失败," + ex.Message);
            }
        }

        /// <summary>
        /// 接收消息回调
        /// </summary>
        public BaseResult Response()
        {
            try
            {
                // 接收服务器的应答
                string code = string.Empty;
                try
                {
                    if (_client.Connected)
                    {
                        int size = Math.Max(1024, _client.ReceiveBufferSize);
                        byte[] data = new byte[size];

                        Int32 bytes = _client.Receive(data, data.Length, SocketFlags.None);
                        code = System.Text.Encoding.Default.GetString(data, 0, bytes);
                        code = code.Replace(@"\0", "");


                        AddResponseRecord(IP, code);
                        OnResponse?.Invoke(this, code);
                        return new BaseResult(true, "接收成功", code);
                    }
                    else
                    {
                        Close();
                        return new BaseResult(false, "连接已断开");
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    //LogOperate.General(logName, $"[{IP}] <--  接收数据异常,{msg}");
                    //msg.Contains("连接") ||
                    if (msg.Contains("套接字") || msg.Contains("通讯"))
                    {
                        Close();
                    }
                    return new BaseResult(false, "接收消息异常: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                LogOperate.General(logName, $"接收消息异常: {ex.Message}");
                LogOperate.Error(logName, ex);
                return new BaseResult(false, "接收消息异常: " + ex.Message);
            }
        }

        /// <summary>
        /// 自动重连
        /// </summary>
        /// <returns></returns>
        public BaseResult Rconnection()
        {
            try
            {
                if (_client == null || !_client.Connected || !isConnect)
                {
                    return Connect();
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.General(logName, $"[{IP}] <--  自动重连失败,{ex.Message}");
                return new BaseResult(false, "自动重连失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 添加回复消息内容
        /// </summary>
        /// <param name="content"></param>
        private void AddResponseRecord(string source, string content)
        {
            try
            {
                MessageModel message = new MessageModel();
                message.Time = DateTime.Now;
                message.Source = source;
                message.Index = ResponseRecords.Count + 1;
                message.Message = content;

                lock (_lock_record)
                {
                    ResponseRecords.Add(message);
                }
                OnRefresh?.Invoke(false);
            }
            catch (Exception ex)
            {
                LogOperate.Error("", ex);
            }
        }
    }
}
