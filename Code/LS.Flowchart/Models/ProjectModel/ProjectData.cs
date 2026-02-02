using System;
using System.Collections.Generic;

namespace LS.Flowchart.Models.ProjectModel
{
    /// <summary>
    /// 方案的数据结构
    /// </summary>
    [Serializable]
    public class ProjectData
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 方案文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 流程列表
        /// </summary>
        public List<ProcessData> ProcessList { get; set; } = new List<ProcessData>();

        /// <summary>
        /// 设备列表
        /// 串口，TCP等通讯设备
        /// </summary>
        public List<ProjectDevice> DeviceList { get; set; } = new List<ProjectDevice>();

        /// <summary>
        /// 相机列表
        /// </summary>
        public List<ProjectCamera> CameraList = new List<ProjectCamera>();

        /// <summary>
        /// 全局对象列表
        /// </summary>
        public List<ProjectGlobalObject> GlobalObjects { get; set; } = new List<ProjectGlobalObject>(); 




    }


}
