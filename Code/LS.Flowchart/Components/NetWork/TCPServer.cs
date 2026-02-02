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
    public class TCPServer : IComponent
    {
        public TCPServer(ProjectDevice device)
        {
            Device = device;
        }

        bool isRunThread = false;
        string logName => Device != null ? $"TCPServer_{Device.Name}" : "TCPServer";
        public ProjectDevice Device { get; }
        Parameter_TCPServer param;
        Parameter_TCPServer _parameter
        {
            get
            {
                if (param == null)
                {
                    if (Device != null)
                    {
                        string strParam = JsonConvert.SerializeObject(Device.DeviceParameter);
                        param = JsonConvert.DeserializeObject<Parameter_TCPServer>(strParam);
                    }
                }
                return param;
            }
        }
        public DeviceEnum DeviceType => Device.DeviceType;
        public string DeviceId => Device?.DeviceId;
        readonly object _lock_record = new object();
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

        private Socket _service = null;
        public Socket Service
        {
            get
            {
                return _service;
            }
        }

        private List<Socket> sockets = new List<Socket>();


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
            Task.Run(() =>
            {
                TCP_Start();
            });
            return BaseResult.Successed;
        }

        public BaseResult Stop()
        {
            isRunThread = false;
            try
            {
                _service?.Close();
                _service?.Dispose();
            }
            catch { }
            return BaseResult.Successed;
        }

        private void TCP_Start()
        {
            try
            {
                string ipAddress = _parameter.LocalIP;
                int port = _parameter.LocalPort;
                LogOperate.Info($"TCPSocket [{Device.Name}] IP：{ipAddress}   端口： {port}");
                _service = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 绑定 IP 地址和端口号
                _service.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
                _service.ReceiveTimeout = 500;
                _service.SendTimeout = 500;
                // 开始监听连接
                _service.Listen(10);

                LogOperate.Start($"启动Socket [{Device.Name}] {ipAddress} ： {port}");

                while (isRunThread)
                {
                    try
                    {
                        // 等待客户端连接
                        Socket clientSocket = _service.Accept();
                        //Console.WriteLine("Client connected");
                        LogOperate.General(logName, $"{clientSocket.LocalEndPoint.ToString()} - 连接成功");
                        if (!sockets.Exists(x => x.AddressFamily == clientSocket.AddressFamily && x.Handle == clientSocket.Handle))
                        {
                            sockets.Add(clientSocket);
                            Task.Run(() =>
                            {
                                // 处理客户端请求
                                HandleClient(clientSocket);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        LogOperate.Error("ReceiveData 发生异常", ex);
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("TCPSocket 启动失败", ex);
            }
        }

        void HandleClient(Socket clientSocket)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while (isRunThread)
                {
                    try
                    {
                        // 读取客户端发送的数据
                        bytesRead = clientSocket.Receive(buffer);
                        if (bytesRead == 0)
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        // 将收到的数据转换为字符串
                        string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        LogOperate.General(logName, $"{clientSocket.RemoteEndPoint.ToString()} - Received: {receivedData}");

                        OnResponse?.Invoke(this, receivedData);
                        AddResponseRecord(clientSocket.RemoteEndPoint.ToString(), receivedData);
                    }
                    catch (Exception ex)
                    {
                        LogOperate.Error("Receive", ex);
                    }
                }

                LogOperate.General(logName, $"{clientSocket.LocalEndPoint.ToString()} - 断开连接");
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
        /// <param name="isHEX">是否十六进制发送</param>
        /// <returns></returns>
        public BaseResult Send(string content, bool isHEX = false)
        {
            try
            {
                if (_service == null)
                {
                    return new BaseResult(false, "未启动连接，无法发送数据");
                }
                if (sockets != null && sockets.Count > 0)
                {
                    foreach (var socket in sockets)
                    {
                        try
                        {
                            if (socket.Connected)
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
                                    LogOperate.General(logName, $"[{socket.RemoteEndPoint.ToString()}] --> {content}");
                                    resIndex = socket.Send(send, send.Length, SocketFlags.None);
                                    if (resIndex < 0)
                                    {
                                        LogOperate.General(logName, $"[{socket.RemoteEndPoint.ToString()}] --> 发送失败");
                                    }
                                }
                                else
                                {
                                    byte[] send = Encoding.ASCII.GetBytes(content);
                                    byte[] data = new byte[1024];

                                    LogOperate.General(logName, $"[{socket.RemoteEndPoint.ToString()}] --> {content}");
                                    resIndex = socket.Send(send, send.Length, SocketFlags.None);
                                    if (resIndex < 0)
                                    {
                                        LogOperate.General(logName, $"[{socket.RemoteEndPoint.ToString()}] --> 发送失败");
                                    }
                                }
                                Thread.Sleep(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogOperate.General(logName, $"[{socket.RemoteEndPoint.ToString()}] --> 发送失败," + ex.Message);
                            LogOperate.Error(logName, ex);
                        }
                    }

                    sockets.RemoveAll(x => x.Connected == false);
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return new BaseResult(false, "发送失败," + ex.Message);
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
                LogOperate.Error(logName, ex);
            }
        }


    }
}
