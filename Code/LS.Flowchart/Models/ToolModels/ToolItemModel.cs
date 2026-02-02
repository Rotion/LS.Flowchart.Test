using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LS.Flowchart.Models.ToolModels
{
    public class ToolItemModel
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public Geometry Icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 内容子项
        /// </summary>
        public ObservableCollection<ModuleItemModel> ModuleItems { get; set; }=new ObservableCollection<ModuleItemModel>();
             
    }
}
