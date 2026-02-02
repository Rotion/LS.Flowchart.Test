using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Flowchart.Cameras;
using LS.Flowchart.Tools;

namespace LS.Flowchart.Models.ProjectModel
{
    public class ProjectCamera
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID { get; set; } = IDHelper.GetGuidId();

        /// <summary>
        /// 相机模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 相机类型
        /// </summary>
        public CameraType Camera_Type { get; set; } = CameraType.HikVison;

        /// <summary>
        /// 相机参数
        /// 根据相机类型有所不同
        /// </summary>
        public dynamic CameraParam { get; set; }
    }
}
