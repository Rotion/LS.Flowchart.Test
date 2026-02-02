using LS.Standard.Data;
using System;
using LS.Flowchart.Models.ProjectModel.Parameters;
using LS.Flowchart.Tools;

namespace LS.Flowchart.Models.ProjectModel
{
    /// <summary>
    /// 方案中的设备信息
    /// </summary>
    public class ProjectDevice
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string DeviceId { get; set; } = IDHelper.GetGuidId();

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceEnum DeviceType { get; set; } = DeviceEnum.COM;

        /// <summary>
        /// 设备参数
        /// </summary>
        public object DeviceParameter { get; set; }


    }
}
