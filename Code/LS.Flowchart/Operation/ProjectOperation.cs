using LS.Standard.Data;
using LS.Flowchart;
using LS.Flowchart.ViewModels;
using LS.WPF.Core.MVVM;
using System;
using System.Xml.Linq;
using LS.Flowchart.Cameras.CameraProudct;
using LS.Flowchart.Cameras.Product;
using LS.Flowchart.Components;
using LS.Flowchart.Components.NetWork;
using LS.Flowchart.Models.ProjectModel;

namespace LS.Flowchart.Operation
{
    /// <summary>
    /// 方案的操作流程
    /// 包括启动，停止，流程执行等
    /// </summary>
    public class ProjectOperation
    {
        /// <summary>
        /// 启动方案
        /// </summary>
        /// <returns></returns>
        public static BaseResult StartProject()
        {
            try
            {
                //启动方案

                //界面显示
                if (VM_MainWindow.Instance != null)
                {
                    VM_MainWindow.Instance.InitProcess();
                }

                //组件加载
                var project = GlobalData.CurrentProject;
                if (project == null)
                {
                    return new BaseResult(false, "方案数据为空");
                }
                if (project.DeviceList != null && project.DeviceList.Count > 0)
                {
                    foreach (var device in project.DeviceList)
                    {
                        AddComponent(device);
                    }
                }

                foreach (var device in project.DeviceList)
                {
                    if (device.IsActive)
                    {
                        var comp = ProjectRunTime.Components.Find(x => x.DeviceId == device.DeviceId);
                        if (comp != null)
                        {
                            if (!comp.IsSatrt())
                            {
                                comp.Start();
                            }
                        }
                    }
                }

                //相机对象加载
                if(project.CameraList!=null && project.CameraList.Count > 0)
                {
                    foreach (var camera in project.CameraList)
                    {
                        AddCamera(camera);
                    }
                }

                //全局对象初始化
                foreach (var obj in project.GlobalObjects)
                {
                    obj.Value = obj.DefaultValue;
                }

                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("StartProject 发生异常", ex);
                return new BaseResult(false, $"启动方案失败,{ex.Message}");
            }
        }

        /// <summary>
        /// 停止方案
        /// </summary>
        /// <returns></returns>
        public static BaseResult StopProject()
        {
            try
            {
                //停止方案

                //停止组件
                foreach (var comp in ProjectRunTime.Components)
                {
                    comp.Stop();
                }
                //清空全局对象

                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("StopProject 发生异常", ex);
                return new BaseResult(false, $"停止方案失败,{ex.Message}");
            }
        }

        #region 组件管理

        /// <summary>
        /// 添加组件对象
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static BaseResult AddComponent(ProjectDevice device)
        {
            if (!ProjectRunTime.Components.Exists(x => x.DeviceId == device.DeviceId))
            {
                //TODO 还要有校验规则，比如是否有相同IP、端口的组件，端口是否被占用等等

                switch (device.DeviceType)
                {
                    case DeviceEnum.TCP_Client:
                        TCPClient tcpClient = new TCPClient(device);
                        tcpClient.OnStateChange += Device_OnStateChange;
                        ProjectRunTime.Components.Add(tcpClient);
                        break;
                    case DeviceEnum.TCP_Server:
                        TCPServer tcpServer = new TCPServer(device);
                        tcpServer.OnStateChange += Device_OnStateChange;
                        ProjectRunTime.Components.Add(tcpServer);
                        break;
                    case DeviceEnum.UDP:
                        UDP udp = new UDP(device);
                        udp.OnStateChange += Device_OnStateChange;
                        ProjectRunTime.Components.Add(udp);
                        break;
                    case DeviceEnum.COM:
                        COM com = new COM(device);
                        com.OnStateChange += Device_OnStateChange;
                        ProjectRunTime.Components.Add(com);
                        break;
                }
            }

            return BaseResult.Successed;
        }

        /// <summary>
        /// 组件状态变化 通知事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        private static void Device_OnStateChange(object sender, bool state)
        {
            try
            {
                IComponent component = (IComponent)sender;
                if (component != null)
                {
                    var index = GlobalData.CurrentProject.DeviceList.FindIndex(x => x.DeviceId == component.DeviceId);
                    if (index >= 0)
                    {
                        GlobalData.CurrentProject.DeviceList[index].IsActive = state;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Device_OnStateChange", ex);
            }
        }

        #endregion

        #region 相机管理

        /// <summary>
        /// 添加相机设备
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static BaseResult AddCamera(ProjectCamera camera)
        {
            try 
            {
                if(!ProjectRunTime.Cameras.Exists(x => x.ID == camera.ID))
                {
                    switch (camera.Camera_Type)
                    {
                        case Cameras.CameraType.LocalImage:
                            LocalImageCamera localImage = new LocalImageCamera(camera);
                            localImage.SetParam(camera.CameraParam);
                            ProjectRunTime.Cameras.Add(localImage);
                            break;
                        case Cameras.CameraType.HikVison:
                            HikVisonCamera hikVison = new HikVisonCamera(camera);
                            hikVison.SetParam(camera.CameraParam);
                            ProjectRunTime.Cameras.Add(hikVison);
                            break;
                    }
                }

                return BaseResult.Successed;
            }
            catch(Exception ex)
            {
                LogOperate.Error("AddCamera(ProjectCamera camera)", ex);
                return new BaseResult(false, $"添加相机失败,{ex.Message}");
            }
        }


        #endregion


        #region 全局变量

        /// <summary>
        /// 添加全局变量
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BaseResult AddGlobalObject(ProjectGlobalObject obj)
        {
            try
            {
                if (GlobalData.CurrentProject.GlobalObjects.Exists(x => x.Name == obj.Name))
                {
                    return new BaseResult(false, "已存在相同名称的全局变量");
                }
                GlobalData.CurrentProject.GlobalObjects.Add(obj);
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("AddGlobalObject 发生异常", ex);
                return new BaseResult(false, $"添加全局变量失败,{ex.Message}");
            }
        }

        /// <summary>
        /// 编辑全局变量
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BaseResult EditGlobalObject(ProjectGlobalObject obj)
        {
            try
            {
                if (GlobalData.CurrentProject.GlobalObjects.Exists(x => x.Name == obj.Name))
                {
                    var index = GlobalData.CurrentProject.GlobalObjects.FindIndex(x => x.Name == obj.Name);
                    if (index>=0)
                    {
                        GlobalData.CurrentProject.GlobalObjects[index] = obj;
                        return BaseResult.Successed;
                    }
                    return new BaseResult(false, "未知名称的全局变量");
                }
                else
                {
                    return new BaseResult(false, "未知名称的全局变量");
                }
                
            }
            catch (Exception ex)
            {
                LogOperate.Error("EditGlobalObject 发生异常", ex);
                return new BaseResult(false, $"编辑全局变量失败,{ex.Message}");
            }
        }

        /// <summary>
        /// 删除全局变量
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BaseResult DeleteGlobalObject(ProjectGlobalObject obj)
        {
            try
            {
                if (GlobalData.CurrentProject.GlobalObjects.Exists(x => x.Name == obj.Name))
                {
                    var index = GlobalData.CurrentProject.GlobalObjects.FindIndex(x => x.Name == obj.Name);
                    if (index >= 0)
                    {
                        GlobalData.CurrentProject.GlobalObjects.RemoveAt(index);
                        return BaseResult.Successed;
                    }
                    return new BaseResult(false, "未知名称的全局变量");
                }
                else
                {
                    return new BaseResult(false, "未知名称的全局变量");
                }

            }
            catch (Exception ex)
            {
                LogOperate.Error("DeleteGlobalObject 发生异常", ex);
                return new BaseResult(false, $"删除全局变量失败,{ex.Message}");
            }
        }

        /// <summary>
        /// 根据名称获取全局变量的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static dynamic GetGlobalObjectValue(string name)
        {
            try
            {
                var obj = GlobalData.CurrentProject.GlobalObjects.Find(x => x.Name == name);
                if (obj != null)
                {
                    switch (obj.ObjectType)
                    {
                        case ObjectType.String:
                            return Convert.ToString(obj.Value);
                        case ObjectType.Int:
                            return Convert.ToInt32(obj.Value);
                        case ObjectType.Double:
                            return Convert.ToDouble(obj.Value);
                        case ObjectType.Float:
                            return Convert.ToSingle(obj.Value);
                        case ObjectType.Bool:
                            return Convert.ToBoolean(obj.Value);
                        default:
                            return obj.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetGlobalObjectValue 发生异常", ex);
            }
            return new object();
        }

        /// <summary>
        /// 根据名称设置全局变量的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BaseResult SetGlobalObjectValue(string name,dynamic value)
        {
            try
            {
                var index = GlobalData.CurrentProject.GlobalObjects.FindIndex(x => x.Name == name);
                if (index>=0)
                {
                    var obj = GlobalData.CurrentProject.GlobalObjects[index];
                    switch (obj.ObjectType)
                    {
                        case ObjectType.String:
                            GlobalData.CurrentProject.GlobalObjects[index].Value = Convert.ToString(value);
                            break;
                        case ObjectType.Int:
                            GlobalData.CurrentProject.GlobalObjects[index].Value = Convert.ToInt32(value);
                            break;
                        case ObjectType.Double:
                            GlobalData.CurrentProject.GlobalObjects[index].Value = Convert.ToDouble(value);
                            break;
                        case ObjectType.Float:
                            GlobalData.CurrentProject.GlobalObjects[index].Value = Convert.ToSingle(value);
                            break;
                        case ObjectType.Bool:
                            GlobalData.CurrentProject.GlobalObjects[index].Value = Convert.ToBoolean(value);
                            break;
                        default:
                            GlobalData.CurrentProject.GlobalObjects[index].Value = value;
                            break;
                    }
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetGlobalObjectValue 发生异常", ex);
                return new BaseResult(false, $"修改全局变量的值失败,{ex.Message}");
            }
        }

        #endregion


    }
}
