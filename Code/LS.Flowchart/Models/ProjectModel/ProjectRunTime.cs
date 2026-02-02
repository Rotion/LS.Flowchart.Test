using System.Collections.Generic;
using LS.Flowchart.Cameras;
using LS.Flowchart.Components;

namespace LS.Flowchart.Models.ProjectModel
{
    /// <summary>
    /// 方案运行时的组件对象
    /// </summary>
    public class ProjectRunTime
    {
        /// <summary>
        /// 通讯设备对象
        /// </summary>
        public static List<IComponent> Components { get; set; } = new List<IComponent>();

        /// <summary>
        /// 相机设备对象
        /// </summary>
        public static List<ICamera> Cameras { get; set; } = new List<ICamera>();
    }
}
