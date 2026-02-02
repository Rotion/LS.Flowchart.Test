
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LS.Flowchart.Tools;

namespace LS.Flowchart.Models.ToolModels
{
    /// <summary>
    /// 模块数据结构
    /// </summary>
    public class ModuleItemModel
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID { get; set; } = IDHelper.GetGuidName();

        /// <summary>
        /// 流程ID
        /// </summary>
        public string ProcessID { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public Geometry Icon { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleTypeEnum ModuleType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否为默认的开始节点或结束节点
        /// </summary>
        public bool IsStartOrEnd { get; set; } = false;

        /// <summary>
        /// 模块配置数据
        /// </summary>
        public dynamic ModuleParamer { get; set; }

        /// <summary>
        /// 输入数据
        /// </summary>
        public dynamic InPutData { get; set; }

        /// <summary>
        /// 输出数据
        /// </summary>
        public dynamic OutPutData { get; set; }

        /// <summary>
        /// 记录在Canvas中的控件位置
        /// </summary>
        public Point InCanvasPoint { get; set; }


        /// <summary>
        ///  上层关系的连接点
        /// </summary>
        public List<PointRelationshipModel> UpperLevelPoints { get; set; } = new List<PointRelationshipModel>();
        /// <summary>
        ///  下层关系的连接点
        /// </summary>
        public List<PointRelationshipModel> LowerLevelPoints { get; set; } = new List<PointRelationshipModel>();



        /// <summary>
        /// 复制一个新对象
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ModuleItemModel Copy(ModuleItemModel source)
        {
            ModuleItemModel newModule = new ModuleItemModel();

            foreach (var p in typeof(ModuleItemModel).GetProperties())
            {
                p.SetValue(newModule, p.GetValue(source));
            }

            return newModule;
        }


    }
}
