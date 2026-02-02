using LS.Standard.Data;
using LS.WPF.Core.MVVM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using LS.Flowchart.Components.ComponentModels;
using LS.Flowchart.Models.ProjectModel;
using static LS.Flowchart.Components.ComponentDelegates;
using static LS.Flowchart.ModuleControls.DeviceTag;

namespace LS.Flowchart.Components.NetWork
{
    public class COM : IComponent
    {
        public COM(ProjectDevice device)
        {
            Device = device;
        }

        public ProjectDevice Device { get; }
        Parameter_COM param;
        Parameter_COM _parameter
        {
            get
            {
                if (param == null)
                {
                    if (Device != null)
                    {
                        string strParam = JsonConvert.SerializeObject(Device.DeviceParameter);
                        param = JsonConvert.DeserializeObject<Parameter_COM>(strParam);
                    }
                }
                return param;
            }
        }
        public DeviceEnum DeviceType => Device.DeviceType;
        public string DeviceId => Device?.DeviceId;
        string logName => Device != null ? $"COM_{Device.Name}" : "COM";
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
        readonly object _lock = new object();
        private readonly static object _lock_record = new object();
        public BaseResult ClearRecord()
        {
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
                LogOperate.Error("", ex);
            }
        }

        bool _runThread = false;
        SerialPort serialPort;

        /// <summary>
        /// 查询是否已启动
        /// </summary>
        /// <returns></returns>
        public bool IsSatrt()
        {
            return serialPort != null && serialPort.IsOpen;
        }
        public BaseResult Start()
        {
            return OpenCom();
        }

        public BaseResult Stop()
        {
            try
            {
                _runThread = false;
                if (serialPort != null)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                    serialPort = null;
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("StopCOM", ex);
            }
            return BaseResult.Successed;
        }


        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        private BaseResult OpenCom()
        {
            try
            {
                if (serialPort != null)
                {
                    Stop();
                }

                serialPort = new SerialPort();
                serialPort.PortName = param.PortName;
                serialPort.BaudRate = param.BaudRate;
                serialPort.Parity = param.Parity;
                serialPort.StopBits = param.StopBits;
                serialPort.DataBits = param.DataBits;
                serialPort.ReadTimeout = param.TimeOut;

                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    return BaseResult.Successed;
                }
                else
                {
                    return new BaseResult(false, "打开串口失败");
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("OpenCom", ex);
                return new BaseResult(false, ex.Message);
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
                if (serialPort == null || !serialPort.IsOpen)
                {
                    return new BaseResult(false, "未启动连接，无法发送数据");
                }

                lock (_lock)
                {
                    if (isHEX)
                    {
                        LogOperate.General(logName, $"[{param.PortName}] --> {content}");
                        serialPort.Write(content);
                        return BaseResult.Successed;
                    }
                    else
                    {
                        LogOperate.General(logName, $"[{param.PortName}] --> {content}");
                        serialPort.Write(content);
                        return BaseResult.Successed;
                    }
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return new BaseResult(false, "发送失败," + ex.Message);
            }
        }
    }
}
