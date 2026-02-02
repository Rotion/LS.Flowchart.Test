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
    public class UDP : IComponent
    {
        public UDP(ProjectDevice device)
        {
            Device = device;
        }

        bool isRunThread = false;
        string logName => Device != null ? $"UDP_{Device.Name}" : "UDP";
        readonly object _lock_record = new object();
        public ProjectDevice Device { get; }
        Parameter_UDP param;
        Parameter_UDP _parameter
        {
            get
            {
                if (param == null)
                {
                    if (Device != null)
                    {
                        string strParam = JsonConvert.SerializeObject(Device.DeviceParameter);
                        param = JsonConvert.DeserializeObject<Parameter_UDP>(strParam);
                    }
                }
                return param;
            }
        }
        public DeviceEnum DeviceType => Device.DeviceType;
        public string DeviceId => Device?.DeviceId;
        /// <summary>
        /// 状态变更通知
        /// </summary>
        public event DelegateOnStateChange OnStateChange;
        public List<MessageModel> ResponseRecords { get; set; } = new List<MessageModel>();
        /// <summary>
        /// 刷新触发
        /// </summary>
        public event DelegateRefresh OnRefresh;
        /// <summary>
        /// 消息回调
        /// </summary>
        public event DelegateOnResponse OnResponse;

        public string LocalIP => _parameter != null ? $"{_parameter.LocalIP}:{_parameter.LocalPort}" : "";
        public string TargetIP => _parameter != null ? $"{_parameter.TargetIP}:{_parameter.TargetPort}" : "";

        public BaseResult ClearRecord()
        {
            lock (_lock_record)
            {
                ResponseRecords.Clear();
            }
            OnRefresh?.Invoke(true);
            return BaseResult.Successed;
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
                LogOperate.Error(logName, ex);
            }
        }

        /// <summary>
        /// 查询是否已启动
        /// </summary>
        /// <returns></returns>
        public bool IsSatrt()
        {
            return _service != null;
        }


        public BaseResult Start()
        {
            isRunThread = true;
            Task.Run(UDP_Start);
            return BaseResult.Successed;
        }

        public BaseResult Stop()
        {
            isRunThread = false;
            try
            {
                _service?.Close();
                _service?.Dispose();
                _service = null;
            }
            catch { }
            try
            {
                _thread_hanle?.Abort();
            }
            catch { }
            return BaseResult.Successed;
        }

        private UdpClient _service;

        public UdpClient Service
        {
            get
            {
                return _service;
            }
        }
        private List<Socket> sockets = new List<Socket>();
        private Thread _thread_hanle;

        private void UDP_Start()
        {

            try
            {
                string ipAddress = _parameter.LocalIP;
                int port = _parameter.LocalPort;

                _service = new UdpClient(new IPEndPoint(IPAddress.Parse(ipAddress), port));
                LogOperate.Start($"启动UDP服务 {ipAddress} ： {port}");

                _service.Client.ReceiveTimeout = 500;
                _service.Client.SendTimeout = 500;
                _thread_hanle = new Thread(() =>
                {
                    HandleClient(_service.Client);
                });
                _thread_hanle.IsBackground = true;
                _thread_hanle.Start();
            }
            catch (Exception ex)
            {
                LogOperate.Error("UDP_Start 启动失败", ex);
            }
        }


        void HandleClient(Socket clientSocket)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;

                while (isRunThread)
                {
                    try
                    {
                        bytesRead = 0;
                        try
                        {
                            // 读取客户端发送的数据
                            bytesRead = clientSocket.Receive(buffer);
                        }
                        catch { }
                        if (bytesRead == 0)
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        // 将收到的数据转换为字符串
                        string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        LogOperate.General(logName, $"{TargetIP} - Received: {receivedData}");

                        OnResponse?.Invoke(this, receivedData);
                        AddResponseRecord(TargetIP, receivedData);
                    }
                    catch (Exception ex)
                    {
                        LogOperate.Error("Receive", ex);
                    }
                }

                LogOperate.General(logName, $"{LocalIP} - 断开连接");
                clientSocket.Close();
                sockets.Remove(clientSocket);
            }
            catch (Exception ex)
            {
                LogOperate.Error("HandleClient", ex);
            }
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
                if (_service == null)
                {
                    return new BaseResult(false, "未启动连接，无法发送数据");
                }
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
                    LogOperate.General(logName, $"[{id}][{TargetIP}] --> {content}");
                    resIndex = _service.Send(send, send.Length, _parameter.TargetIP, _parameter.TargetPort);
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

                    LogOperate.General(logName, $"[{id}][{TargetIP}] --> {content}");
                    resIndex = _service.Send(send, send.Length, _parameter.TargetIP, _parameter.TargetPort);
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
            catch (Exception ex)
            {
                string msg = ex.Message;
                LogOperate.General(logName, $"[{id}][{TargetIP}]  <--  发送失败,{msg}");
                return new BaseResult(false, "发送失败," + ex.Message);
            }
        }

    }
}
