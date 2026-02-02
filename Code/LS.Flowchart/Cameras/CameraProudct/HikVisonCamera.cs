using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.Tools;
using LS.Standard.Data;
using LS.WPF.Core.MVVM;
using MvCamCtrl.NET;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.InteropServices;

namespace LS.Flowchart.Cameras.Product
{
    /// <summary>
    /// 海康相机
    /// </summary>
    public class HikVisonCamera : ICamera
    {
        /// <summary>
        /// 相机参数
        /// </summary>
        public dynamic CameraParam { get; set; }

        public string ID => _projectCamera?.ID;

        public ProjectCamera Project_Camera => _projectCamera;

        public BaseResult SetParam(dynamic cameraParam)
        {
            try
            {
                BaseResult res = BaseResult.Successed;
                string msg = "";
                if (cameraParam is HikVisonCameraParam)
                {
                    CameraParam = cameraParam;
                    var param = cameraParam as HikVisonCameraParam;

                    if (myCamera == null || !IsConnect())
                    {
                        res = ConnectCamera(param.SerialNumber);
                    }

                    //设置图像宽度
                    res = SetImageWidth(param.Width);
                    if (!res)
                    {
                        msg += res.Message + ";";
                    }

                    //设置图像高度
                    res = SetImageHeight(param.Height);
                    if (!res)
                    {
                        msg += res.Message + ";";
                    }

                    //设置曝光
                    if (param.AutoExposure)
                    {
                        res = SetAutoExposure(true);
                        if (!res)
                        {
                            msg += res.Message + ";";
                        }
                    }
                    else
                    {
                        res = SetAutoExposure(false);
                        if (!res)
                        {
                            msg += res.Message + ";";
                        }
                        res = SetExposureTime(param.ExposureTime);
                        if (!res)
                        {
                            msg += res.Message + ";";
                        }
                    }

                    //设置增益
                    if (param.AutoGain)
                    {
                        res = SetAutoGain(true);
                        if (!res)
                        {
                            msg += res.Message + ";";
                        }
                    }
                    else
                    {
                        res = SetAutoGain(false);
                        if (!res)
                        {
                            msg += res.Message + ";";
                        }
                        res = SetGainNum(param.Gain);
                        if (!res)
                        {
                            msg += res.Message + ";";
                        }
                    }

                    //设置频率
                    res = SetFrameRate(param.ResultingFrameRate);
                    if (!res)
                    {
                        msg += res.Message + ";";
                    }

                    //设置触发模式
                    res = SetTriggerMode(true);
                    if (!res)
                    {
                        msg += res.Message + ";";
                    }

                    //设置触发方式
                    res = SetTriggerWay(param.TriggerWay);
                    if (!res)
                    {
                        msg += res.Message + ";";
                    }
                }
                else
                {
                    msg = "参数与类型不匹配，请检查数据源保存情况";
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    return new BaseResult(false, msg);
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("Camera SetParam", ex);
                return new BaseResult(false, ex.Message);
            }
        }
        public BaseResult SetExposure(uint exposure)
        {
            return SetExposureTime(exposure);
        }
        public BaseResult SetGain(uint gain)
        {
            return SetGainNum(gain);
        }
        public BaseResult Triggertrap()
        {
            return SoftTrigger();
        }


        //调用示例
        //Hikvision camera = new Hikvision();//创建相机对象并实例化
        //camera.connectCamera("123456");//连接相机，传入相机序列号123456
        //camera.startCamera();//开启相机采集
        //camera.setExposureTime(10000);//设置曝光时间为10ms
        //camera.softTrigger();//发送软触发采集图像
        //Himage image=camera.readImage();//获取采集到且转换后的图像
        //camera.stopCamera();//停止相机采集
        //camera.closeCamera();//关闭相机

        public HikVisonCamera(ProjectCamera projectCamera)
        {
            _projectCamera = projectCamera;
            InitCamera();
        }

        #region 属性


        ProjectCamera _projectCamera;

        public MyCamera myCamera;//相机对象
        public MyCamera.MV_CC_DEVICE_INFO_LIST deviceList;//设备列表
        public MyCamera.MV_CC_DEVICE_INFO deviceInfo;//设备对象
        public string seriesStr;//接收相机序列号
        public MyCamera.MVCC_INTVALUE stParam;//用于接收特定的参数

        //为读取、保存图像创建的数组
        bool m_bGrabbing = false;
        Thread m_hReceiveThread;
        MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
        UInt32 m_nBufSizeForDriver = 0;
        IntPtr m_BufForDriver;
        private static Object BufForDriverLock = new Object();
        //保存图像使用的参数
        UInt32 m_nBufSizeForSaveImage = 0;
        IntPtr m_BufForSaveImage;

        //图像显示控件
        IntPtr image_handle;

        /// <summary>
        /// 获取实时的图像数据 委托方法
        /// </summary>
        public event DelegateReceiveImage OnReceiveImage;

        #endregion


        #region 公共操作方法

        /// <summary>
        /// 初始化相机数据
        /// </summary>
        public void InitCamera()
        {
            deviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
        }

        /// <summary>
        /// 相机是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnect()
        {
            if (myCamera == null)
                return false;
            return myCamera.MV_CC_IsDeviceConnected_NET();
        }

        /// <summary>
        /// 寻找设备
        /// </summary>
        public List<CameraInfo> FindCameras()
        {
            List<CameraInfo> cameraInfos = new List<CameraInfo>();

            deviceList.nDeviceNum = 0;
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref deviceList);
            if (MyCamera.MV_OK != nRet)
            {
                return cameraInfos;
            }

            if (deviceList.nDeviceNum > 0)
            {
                for (int index = 0; index < deviceList.nDeviceNum; index++)
                {
                    CameraInfo info = new CameraInfo();
                    var camera = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(deviceList.pDeviceInfo[index], typeof(MyCamera.MV_CC_DEVICE_INFO));//获取设备
                    if (camera.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(camera.SpecialInfo.stGigEInfo, 0);
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                        //m_SerialNumber = gigeInfo.chSerialNumber;//获取序列号

                        info.SerialNumber = gigeInfo.chUserDefinedName;//获取用户名
                        info.CameraName = gigeInfo.chModelName;

                    }
                    else if (camera.nTLayerType == MyCamera.MV_USB_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(camera.SpecialInfo.stUsb3VInfo, 0);
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        info.SerialNumber = usbInfo.chUserDefinedName;
                        info.CameraName = usbInfo.chModelName;
                    }

                    if (!string.IsNullOrEmpty(info.SerialNumber))
                    {
                        cameraInfos.Add(info);
                    }
                }
            }
            return cameraInfos;
        }

        /// <summary>
        /// 相机修改IP
        /// 调用函数时可以传入需要改变的目标IP，如过没有传入则将相机IP设置为其所连接的网卡地址+1或-1
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public BaseResult ChangeIP(string IP = "")
        {
            try
            {
                //获取相机相关信息，例如相机所连接网卡的网址
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(deviceInfo.SpecialInfo.stGigEInfo, 0);
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                IPAddress cameraIPAddress;
                string tempStr = "";
                if (IP.Trim().Equals("") || !(IPAddress.TryParse(IP, out cameraIPAddress)))
                {
                    //当前网卡的IP地址
                    UInt32 nNetIp1 = (gigeInfo.nNetExport & 0xFF000000) >> 24;
                    UInt32 nNetIp2 = (gigeInfo.nNetExport & 0x00FF0000) >> 16;
                    UInt32 nNetIp3 = (gigeInfo.nNetExport & 0x0000FF00) >> 8;
                    UInt32 nNetIp4 = (gigeInfo.nNetExport & 0x000000FF);
                    //根据网卡IP设定相机IP，如果网卡ip第四位小于252，则相机ip第四位+1，否则相机IP第四位-1
                    UInt32 cameraIp1 = nNetIp1;
                    UInt32 cameraIp2 = nNetIp2;
                    UInt32 cameraIp3 = nNetIp3;
                    UInt32 cameraIp4 = nNetIp4;
                    if (nNetIp4 < 252)
                    {
                        cameraIp4++;
                    }
                    else
                    {
                        cameraIp4--;
                    }
                    tempStr = cameraIp1 + "." + cameraIp2 + "." + cameraIp3 + "." + cameraIp4;
                }
                else
                {
                    tempStr = IP;
                }
                IPAddress.TryParse(tempStr, out cameraIPAddress);
                long cameraIP = IPAddress.NetworkToHostOrder(cameraIPAddress.Address);
                //设置相机掩码
                uint maskIp1 = (gigeInfo.nCurrentSubNetMask & 0xFF000000) >> 24;
                uint maskIp2 = (gigeInfo.nCurrentSubNetMask & 0x00FF0000) >> 16;
                uint maskIp3 = (gigeInfo.nCurrentSubNetMask & 0x0000FF00) >> 8;
                uint maskIp4 = (gigeInfo.nCurrentSubNetMask & 0x000000FF);
                IPAddress subMaskAddress;
                tempStr = maskIp1 + "." + maskIp2 + "." + maskIp3 + "." + maskIp4;
                IPAddress.TryParse(tempStr, out subMaskAddress);
                long maskIP = IPAddress.NetworkToHostOrder(subMaskAddress.Address);
                //设置网关
                uint gateIp1 = (gigeInfo.nDefultGateWay & 0xFF000000) >> 24;
                uint gateIp2 = (gigeInfo.nDefultGateWay & 0x00FF0000) >> 16;
                uint gateIp3 = (gigeInfo.nDefultGateWay & 0x0000FF00) >> 8;
                uint gateIp4 = (gigeInfo.nDefultGateWay & 0x000000FF);
                IPAddress gateAddress;
                tempStr = gateIp1 + "." + gateIp2 + "." + gateIp3 + "." + gateIp4;
                IPAddress.TryParse(tempStr, out gateAddress);
                long gateIP = IPAddress.NetworkToHostOrder(gateAddress.Address);

                int temp = myCamera.MV_GIGE_ForceIpEx_NET((UInt32)(cameraIP >> 32), (UInt32)(maskIP >> 32), (UInt32)(gateIP >> 32));//执行更改相机IP的命令
                if (temp == 0)
                    //强制IP成功
                    return BaseResult.Successed;
                //强制IP失败
                return BaseResult.Failed;
            }
            catch (Exception ex)
            {
                //PCZDLogHelper.Error("相机修改IP异常", ex);
                return BaseResult.Failed;
            }
        }

        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="id">列表设备索引 0开始</param>
        /// <returns></returns>
        public BaseResult ConnectCamera(string sn)
        {
            string m_SerialNumber = "";//接收设备返回的序列号
            int temp;//接收命令执行结果
            myCamera = new MyCamera();
            try
            {
                if (deviceList.nDeviceNum <= 0)
                {
                    temp = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref deviceList);//更新设备列表
                    if (temp != 0)
                    {
                        //设备更新成功接收命令的返回值为0，返回值不为0则为异常
                        return new BaseResult(false, "刷新设备列表错误");
                    }
                }

                if (deviceList.nDeviceNum <= 0)
                {
                    return new BaseResult(false, "搜索到的设备数量为0");
                }

                for (int index = 0; index < deviceList.nDeviceNum; index++)
                {
                    var camera = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(deviceList.pDeviceInfo[index], typeof(MyCamera.MV_CC_DEVICE_INFO));//获取设备
                    if (camera.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(camera.SpecialInfo.stGigEInfo, 0);
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                        //m_SerialNumber = gigeInfo.chSerialNumber;//获取序列号
                        m_SerialNumber = gigeInfo.chUserDefinedName;//获取用户名                        
                    }
                    else if (camera.nTLayerType == MyCamera.MV_USB_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(camera.SpecialInfo.stUsb3VInfo, 0);
                        MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                        m_SerialNumber = usbInfo.chUserDefinedName;
                    }

                    if (m_SerialNumber == sn)
                    {
                        deviceInfo = camera;
                        break;
                    }
                }

                this.seriesStr = m_SerialNumber;
                temp = myCamera.MV_CC_CreateDevice_NET(ref deviceInfo);
                if (MyCamera.MV_OK != temp)
                {
                    //创建相机失败
                    return new BaseResult(false, "相机创建失败");
                }
                temp = myCamera.MV_CC_OpenDevice_NET();//
                if (MyCamera.MV_OK != temp)
                {
                    //打开相机失败
                    return new BaseResult(false, "打开相机失败");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                //PCZDLogHelper.Error("打开相机设备异常", ex);
                return new BaseResult(false, "打开相机设备发生异常，操作失败");
            }
        }

        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="ip">相机IP地址 需要IP转long</param>
        /// <returns></returns>
        public BaseResult ConnectCamera(long ip)
        {
            string m_SerialNumber = "";//接收设备返回的序列号
            int temp;//接收命令执行结果
            myCamera = new MyCamera();
            try
            {
                if (deviceList.nDeviceNum <= 0)
                {
                    temp = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref deviceList);//更新设备列表
                    if (temp != 0)
                    {
                        //设备更新成功接收命令的返回值为0，返回值不为0则为异常
                        return new BaseResult(false, "刷新设备列表错误");
                    }
                }

                bool isFind = false;
                for (int index = 0; index < deviceList.nDeviceNum; index++)
                {
                    deviceInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(deviceList.pDeviceInfo[index], typeof(MyCamera.MV_CC_DEVICE_INFO));//获取设备
                    if (deviceInfo.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                    {
                        //使用IP就只有网口通讯类型了
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(deviceInfo.SpecialInfo.stGigEInfo, 0);
                        MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                        if (gigeInfo.nCurrentIp == ip)
                        {
                            m_SerialNumber = gigeInfo.chUserDefinedName;//获取用户名
                            isFind = true;
                            break;
                        }
                    }
                }
                if (!isFind)
                {
                    return new BaseResult(false, $"未找到相应IP的相机设备，IP:[{IPHelper.LongToIp(ip)}]");
                }
                this.seriesStr = m_SerialNumber;
                temp = myCamera.MV_CC_CreateDevice_NET(ref deviceInfo);
                if (MyCamera.MV_OK != temp)
                {
                    //创建相机失败
                    return new BaseResult(false, "相机创建失败");
                }
                temp = myCamera.MV_CC_OpenDevice_NET();//
                if (MyCamera.MV_OK != temp)
                {
                    //打开相机失败
                    return new BaseResult(false, "打开相机失败");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("打开相机设备异常", ex);
                return new BaseResult(false, "打开相机设备发生异常，操作失败");
            }
        }

        /// <summary>
        /// 相机开始采集
        /// </summary>
        /// <returns></returns>
        public BaseResult StartCamera()
        {
            //采集标识
            m_bGrabbing = true;

            //收集回调数据的处理线程
            m_hReceiveThread = new Thread(ReceiveThreadProcess);
            m_hReceiveThread.Start();

            m_stFrameInfo.nFrameLen = 0;//取流之前先清除帧长度
            m_stFrameInfo.enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
            // ch:开始采集 | en:Start Grabbing
            int nRet = myCamera.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_bGrabbing = false;
                m_hReceiveThread.Join();
                return BaseResult.Failed;
            }

            return BaseResult.Successed;
        }

        /// <summary>
        /// 停止相机采集
        /// </summary>
        /// <returns></returns>
        public BaseResult StopCamera()
        {
            m_bGrabbing = false;
            m_hReceiveThread?.Join();

            if (myCamera == null)
                return BaseResult.Successed;
            int temp = myCamera.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != temp)
                return BaseResult.Failed;
            return BaseResult.Successed;
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <returns></returns>
        public BaseResult CloseCamera()
        {
            if (myCamera == null)
                return BaseResult.Successed;
            if (m_bGrabbing)
            {
                var res = StopCamera();//停止相机采集
                if (!res)
                    return res;
            }
            int temp = myCamera.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != temp)
                return BaseResult.Failed;
            temp = myCamera.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != temp)
                return BaseResult.Failed;
            return BaseResult.Successed;
        }

        /// <summary>
        /// 设置图像宽度
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public BaseResult SetImageWidth(uint width)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                if (width > 0)
                {
                    Res = myCamera.MV_CC_SetIntValue_NET("Width", width);
                    if (MyCamera.MV_OK != Res)
                    {
                        return new BaseResult(false, $"相机参数-设置相机宽度出错 ->【{width}】");
                    }
                }
                else
                {
                    return new BaseResult(false, $"相机参数-设置相机宽度不能为0 ->【{width}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetImageWidth", ex);
                return new BaseResult(false, ex.Message);
            }
        }


        /// <summar>
        /// 设置图像高度
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public BaseResult SetImageHeight(uint height)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                if (height > 0)
                {
                    Res = myCamera.MV_CC_SetIntValue_NET("Height", height);
                    if (MyCamera.MV_OK != Res)
                    {
                        return new BaseResult(false, $"相机参数-设置相机高度出错【{height}】");
                    }
                }
                else
                {
                    return new BaseResult(false, $"相机参数-设置相机高度不能为0 ->【{height}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetImageHeight", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 设置自动曝光
        /// </summary>
        /// <param name="isAutoExposure"></param>
        /// <returns></returns>
        public BaseResult SetAutoExposure(bool isAutoExposure)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                //设置连续自动曝光模式
                //ExposureAutoMode值为0，表示自动曝光模式关闭；
                //ExposureAutoMode值为1，表示单次自动曝光模式开启；
                //ExposureAutoMode值为2，表示连续自动曝光模式开启。
                uint ExposureAutoMode = isAutoExposure ? (uint)2 : 0;
                Res = myCamera.MV_CC_SetExposureAutoMode_NET(ExposureAutoMode);
                if (MyCamera.MV_OK != Res)
                {
                    return new BaseResult(false, $"相机参数-设置自动 曝光,参数【{ExposureAutoMode}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetAutoExposure", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 设置自动增益
        /// </summary>
        /// <param name="isAutoGain"></param>
        /// <returns></returns>
        public BaseResult SetAutoGain(bool isAutoGain)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                uint GainAutoMode = isAutoGain ? (uint)2 : (uint)0;
                Res = myCamera.MV_CC_SetGainMode_NET(GainAutoMode);
                if (MyCamera.MV_OK != Res)
                {
                    return new BaseResult(false, $"相机参数-设置自动 增益,参数【{GainAutoMode}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetAutoGain", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 设置曝光时间        
        /// </summary>
        /// <param name="exposureTime"></param>
        /// <returns></returns>
        public BaseResult SetExposureTime(float exposureTime)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                if (exposureTime >= 0)
                {
                    Res = myCamera.MV_CC_SetFloatValue_NET("ExposureTime", exposureTime);
                    if (MyCamera.MV_OK != Res)
                    {
                        return new BaseResult(false, $"相机参数-设置曝光时间出错【{exposureTime}】");
                    }
                }
                else
                {
                    return new BaseResult(false, $"相机参数-设置曝光时间不能为0 ->【{exposureTime}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetExposureTime", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="gain"></param>
        /// <returns></returns>
        public BaseResult SetGainNum(float gain)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                if (gain >= 0)
                {
                    Res = myCamera.MV_CC_SetFloatValue_NET("Gain", gain);
                    if (MyCamera.MV_OK != Res)
                    {
                        return new BaseResult(false, $"相机参数-设置增益出错【{gain}】");
                    }
                }
                else
                {
                    return new BaseResult(false, $"相机参数-设置增益不能为0 ->【{gain}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetGain", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 设置频率
        /// </summary>
        /// <param name="frameRate"></param>
        /// <returns></returns>
        public BaseResult SetFrameRate(float frameRate)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                if (frameRate > 0)
                {
                    Res = myCamera.MV_CC_SetFloatValue_NET("AcquisitionFrameRate", frameRate);
                    if (MyCamera.MV_OK != Res)
                    {
                        return new BaseResult(false, $"相机参数-设置频率错误,参数【{frameRate}】");
                    }
                }
                else
                {
                    return new BaseResult(false, $"相机参数-设置频率不能为0 ->【{frameRate}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetFrameRate", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public BaseResult SetTriggerMode(bool isTrigger)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                //1：触发模式  0：非触发模式
                uint mode = isTrigger ? (uint)1 : (uint)0;
                Res = myCamera.MV_CC_SetEnumValue_NET("TriggerMode", mode);
                if (MyCamera.MV_OK != Res)
                {
                    return new BaseResult(false, $"相机参数-设置触发模式失败,参数【{1}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetTriggerMode", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 设置触发源(方式)
        /// 0 - Line0;
        /// 1 - Line1;
        /// 2 - Line2;
        /// 3 - Line3;
        /// 4 - Counter;
        /// 7 - Software;
        /// </summary>
        /// <param name="way">  0 - Line0; 1 - Line1;2 - Line2; 3 - Line3; 4 - Counter;7 - Software;</param>
        /// <returns></returns>
        public BaseResult SetTriggerWay(uint way)
        {
            try
            {
                if (myCamera == null)
                {
                    return new BaseResult(false, "相机未连接");
                }
                int Res = 0;
                Res = myCamera.MV_CC_SetEnumValue_NET("TriggerSource", way);
                if (MyCamera.MV_OK != Res)
                {
                    return new BaseResult(false, $"相机参数-设置触发源失败,参数【{1}】");
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetTriggerMode", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 绑定显示图象控件
        /// </summary>
        /// <param name="handle"></param>
        public void BindDisplayControl(IntPtr handle)
        {
            image_handle = handle;
        }



        /// <summary>
        /// 获取相机参数
        /// 如：曝光时间 、增益、频率等
        /// </summary>
        public void GetCameraParam()
        {
            try
            {
                if (myCamera == null)
                    return;

                var param = new HikVisonCameraParam();
                if (CameraParam != null)
                {
                    param = CameraParam;
                }
                MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
                int nRet = myCamera.MV_CC_GetFloatValue_NET("ExposureTime", ref stParam);
                if (MyCamera.MV_OK == nRet)
                {
                    param.ExposureTime = stParam.fCurValue;
                }

                nRet = myCamera.MV_CC_GetFloatValue_NET("Gain", ref stParam);
                if (MyCamera.MV_OK == nRet)
                {
                    param.Gain = stParam.fCurValue;
                }

                nRet = myCamera.MV_CC_GetFloatValue_NET("ResultingFrameRate", ref stParam);
                if (MyCamera.MV_OK == nRet)
                {
                    param.ResultingFrameRate = stParam.fCurValue;
                }




                var width = GetWidth();
                if (param.Width <= 0 || (width > 0 && width < param.Width))
                {
                    param.Width = width;
                }
                var height = GetHeight();
                if (param.Height <= 0 || (height > 0 && height < param.Height))
                {
                    param.Height = height;
                }

                CameraParam = param;
            }
            catch (Exception ex)
            {
                LogOperate.Error("获取相机参数发生异常", ex);
            }
        }


        /// <summary>
        /// 软触发一次
        /// 注意：软触发采集图像需要将相机设置为触发模式，并设置触发源为soft。
        /// </summary>
        /// <param name="triggerString">TriggerSoftware</param>
        /// <returns></returns>
        public BaseResult SoftTrigger(string triggerString = "TriggerSoftware")
        {
            int temp = myCamera.MV_CC_SetCommandValue_NET(triggerString);
            if (MyCamera.MV_OK != temp)
                return BaseResult.Failed;
            return BaseResult.Successed;
        }


        /// <summary>
        /// 获取图像宽度
        /// </summary>
        /// <returns></returns>
        public uint GetWidth()
        {
            if (myCamera == null)
                return 0;
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int temp = myCamera.MV_CC_GetIntValue_NET("Width", ref stParam);
            if (MyCamera.MV_OK == temp)
                return stParam.nCurValue;
            return 0;
        }

        /// <summary>
        /// 获取图像高度
        /// </summary>
        /// <returns></returns>
        public uint GetHeight()
        {
            if (myCamera == null)
                return 0;
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int temp = myCamera.MV_CC_GetIntValue_NET("Height", ref stParam);
            if (MyCamera.MV_OK == temp)
                return stParam.nCurValue;
            return 0;
        }

        /// <summary>
        /// 设置心跳时间，成功返回0失败返回-1
        /// </summary>
        /// <param name="heartBeatTime"></param>
        /// <returns></returns>
        public BaseResult SetHeartBeatTime(uint heartBeatTime)
        {
            //心跳时间最小为500
            uint tempTime = heartBeatTime > 500 ? heartBeatTime : 500;
            int temp = myCamera.MV_CC_SetIntValue_NET("GevHeartbeatTimeout", tempTime);
            if (MyCamera.MV_OK != temp)
                return BaseResult.Failed;
            return BaseResult.Successed;
        }

        /// <summary>
        /// 输出一张Bitmap图片
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public BaseResult SaveBmp(string bmpFile, out Bitmap bmp)
        {
            bmp = new Bitmap(1, 1);
            try
            {
                if (!m_bGrabbing)
                {
                    return new BaseResult(false, "未开启采集");
                }

                if (RemoveCustomPixelFormats(m_stFrameInfo.enPixelType))
                {
                    return new BaseResult(false, "自定义像素格式");
                }

                IntPtr pTemp = IntPtr.Zero;
                MyCamera.MvGvspPixelType enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                if (m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8 || m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed)
                {
                    pTemp = m_BufForDriver;
                    enDstPixelType = m_stFrameInfo.enPixelType;
                }
                else
                {
                    UInt32 nSaveImageNeedSize = 0;
                    MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

                    lock (BufForDriverLock)
                    {
                        if (m_stFrameInfo.nFrameLen == 0)
                        {
                            return new BaseResult(false, "获取bitmap图像失败");
                        }

                        if (IsMonoData(m_stFrameInfo.enPixelType))
                        {
                            enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                            nSaveImageNeedSize = (uint)m_stFrameInfo.nWidth * m_stFrameInfo.nHeight;
                        }
                        else if (IsColorData(m_stFrameInfo.enPixelType))
                        {
                            enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                            nSaveImageNeedSize = (uint)m_stFrameInfo.nWidth * m_stFrameInfo.nHeight * 3;
                        }
                        else
                        {
                            return new BaseResult(false, "无效的像素类型");
                        }

                        if (m_nBufSizeForSaveImage < nSaveImageNeedSize)
                        {
                            if (m_BufForSaveImage != IntPtr.Zero)
                            {
                                Marshal.Release(m_BufForSaveImage);
                            }
                            m_nBufSizeForSaveImage = nSaveImageNeedSize;
                            m_BufForSaveImage = Marshal.AllocHGlobal((Int32)m_nBufSizeForSaveImage);
                        }

                        stConverPixelParam.nWidth = m_stFrameInfo.nWidth;
                        stConverPixelParam.nHeight = m_stFrameInfo.nHeight;
                        stConverPixelParam.pSrcData = m_BufForDriver;
                        stConverPixelParam.nSrcDataLen = m_stFrameInfo.nFrameLen;
                        stConverPixelParam.enSrcPixelType = m_stFrameInfo.enPixelType;
                        stConverPixelParam.enDstPixelType = enDstPixelType;
                        stConverPixelParam.pDstBuffer = m_BufForSaveImage;
                        stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
                        int nRet = myCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            return new BaseResult(false, "像素类型转换失败");
                        }
                        pTemp = m_BufForSaveImage;
                    }
                }

                lock (BufForDriverLock)
                {
                    if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    {
                        //Mono8 转 Bitmap
                        bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, pTemp);

                        ColorPalette cp = bmp.Palette;
                        for (int i = 0; i < 256; i++)
                        {
                            cp.Entries[i] = Color.FromArgb(i, i, i);
                        }
                        bmp.Palette = cp;
                        bmp.Save(bmpFile, ImageFormat.Bmp);
                    }
                    else
                    {
                        //BGR8 转 Bitmap
                        try
                        {
                            bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pTemp);
                            bmp.Save(bmpFile, ImageFormat.Bmp);
                        }
                        catch
                        {
                            return new BaseResult(false, "生成图像失败");
                        }
                    }
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("保存图像发生异常", ex);
                return new BaseResult(false, "保存图像异常");
            }
        }


        #endregion


        #region 私有方法

        /// <summary>
        /// 获取图像的回调线程
        /// </summary>
        private void ReceiveThreadProcess()
        {
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = myCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }

            UInt32 nPayloadSize = stParam.nCurValue;
            if (nPayloadSize > m_nBufSizeForDriver)
            {
                if (m_BufForDriver != IntPtr.Zero)
                {
                    Marshal.Release(m_BufForDriver);
                }
                m_nBufSizeForDriver = nPayloadSize;
                m_BufForDriver = Marshal.AllocHGlobal((Int32)m_nBufSizeForDriver);
            }

            if (m_BufForDriver == IntPtr.Zero)
            {
                return;
            }

            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
            MyCamera.MV_DISPLAY_FRAME_INFO stDisplayInfo = new MyCamera.MV_DISPLAY_FRAME_INFO();

            while (m_bGrabbing)
            {
                try
                {
                    lock (BufForDriverLock)
                    {
                        nRet = myCamera.MV_CC_GetOneFrameTimeout_NET(m_BufForDriver, nPayloadSize, ref stFrameInfo, 1000);
                        if (nRet == MyCamera.MV_OK)
                        {
                            m_stFrameInfo = stFrameInfo;
                        }
                    }

                    if (nRet == MyCamera.MV_OK)
                    {
                        if (RemoveCustomPixelFormats(stFrameInfo.enPixelType))
                        {
                            continue;
                        }
                        if (image_handle != IntPtr.Zero)
                        {
                            stDisplayInfo.hWnd = image_handle;
                        }
                        stDisplayInfo.pData = m_BufForDriver;
                        stDisplayInfo.nDataLen = stFrameInfo.nFrameLen;
                        stDisplayInfo.nWidth = stFrameInfo.nWidth;
                        stDisplayInfo.nHeight = stFrameInfo.nHeight;
                        stDisplayInfo.enPixelType = stFrameInfo.enPixelType;
                        myCamera.MV_CC_DisplayOneFrame_NET(ref stDisplayInfo);

                        Bitmap bmp = null;
                        if (stDisplayInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                        {
                            //Mono8 转 Bitmap
                            bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, stDisplayInfo.pData);

                            ColorPalette cp = bmp.Palette;
                            for (int i = 0; i < 256; i++)
                            {
                                cp.Entries[i] = Color.FromArgb(i, i, i);
                            }
                            bmp.Palette = cp;
                            //bmp.Save("image.bmp", ImageFormat.Bmp);
                        }
                        else
                        {
                            //BGR8 转 Bitmap
                            try
                            {
                                bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, stDisplayInfo.pData);
                                //bmp.Save("image.bmp", ImageFormat.Bmp);
                            }
                            catch
                            {
                                bmp = null;
                            }
                        }
                        if (bmp != null)
                            OnReceiveImage?.Invoke(bmp);
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Thread.Sleep(0);
                }
            }
        }


        /// <summary>
        /// 去除自定义的像素格式 
        /// </summary>
        /// <param name="enPixelFormat"></param>
        /// <returns></returns>
        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否为黑白图像
        /// </summary>
        /// <param name="enGvspPixelType"></param>
        /// <returns></returns>
        private Boolean IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 判断是否彩色照片
        /// </summary>
        /// <param name="enGvspPixelType"></param>
        /// <returns></returns>
        private Boolean IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }



        #endregion

    }


    /// <summary>
    /// 海康相机参数
    /// </summary>
    public class HikVisonCameraParam
    {
        /// <summary>
        /// 相机序列号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 相机名称
        /// 型号
        /// </summary>
        public string CameraName { get; set; }

        /// <summary>
        /// 图像宽度
        /// </summary>
        public uint Width { get; set; } = 1920;

        /// <summary>
        /// 图像高度
        /// </summary>
        public uint Height { get; set; } = 1080;

        /// <summary>
        /// 自动曝光
        /// </summary>
        public bool AutoExposure { get; set; } = true;
        /// <summary>
        /// 自动增益
        /// </summary>
        public bool AutoGain { get; set; } = true;

        /// <summary>
        /// 曝光时间
        /// </summary>
        public float ExposureTime { get; set; } = 0;

        /// <summary>
        /// 增益
        /// </summary>
        public float Gain { get; set; } = 0;

        /// <summary>
        /// 频率
        /// </summary>
        public float ResultingFrameRate { get; set; } = 30;

        /// <summary>
        /// 触发模式
        /// </summary>
        public uint TriggerWay { get; set; } = 7;

    }
}
