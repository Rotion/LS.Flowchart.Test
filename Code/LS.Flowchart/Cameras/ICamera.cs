using LS.Standard.Data;
using System.Drawing;
using LS.Flowchart.Models.ProjectModel;

namespace LS.Flowchart.Cameras
{
    /// <summary>
    /// 相机定义接口
    /// </summary>
    public interface ICamera
    {

        /// <summary>
        /// 唯一标识符
        /// </summary>
        string ID { get;  }

        /// <summary>
        /// 相机对象
        /// </summary>
        ProjectCamera Project_Camera { get; }

        /// <summary>
        /// 相机参数
        /// </summary>
        dynamic CameraParam { get; set; }

        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="cameraParam"></param>
        /// <returns></returns>
        BaseResult SetParam(dynamic cameraParam);

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="exposure"></param>
        /// <returns></returns>
        BaseResult SetExposure(uint exposure);

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="gain"></param>
        /// <returns></returns>
        BaseResult SetGain(uint gain);

        /// <summary>
        /// 触发拍照
        /// </summary>
        /// <returns></returns>
        BaseResult Triggertrap();

        /// <summary>
        /// 获取实时的图像数据 委托方法
        /// </summary>
        event DelegateReceiveImage OnReceiveImage;


    }
}
