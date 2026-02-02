using System;
using System.Collections.Generic;
using LS.Flowchart.Models.ToolModels;
using LS.Flowchart.Tools;

namespace LS.Flowchart.Models.ProjectModel
{
    /// <summary>
    /// 流程数据模型
    /// </summary>
    public class ProcessData
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public string ID { get; set; } = IDHelper.GetGuidName();

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 流程索引
        /// </summary>
        public int Index { get; set; } = 1;

        /// <summary>
        /// 模块列表集合
        /// </summary>
        public List<ModuleItemModel> ModuleItems { get; set; } = new List<ModuleItemModel>();



    }
}
