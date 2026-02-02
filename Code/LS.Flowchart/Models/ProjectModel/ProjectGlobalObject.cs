using Newtonsoft.Json;
using System;
using LS.Flowchart.Tools;

namespace LS.Flowchart.Models.ProjectModel
{
    /// <summary>
    /// 项目全局对象
    /// </summary>
    [Serializable]
    public class ProjectGlobalObject
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID { get; set; } = IDHelper.GetGuidName();

        /// <summary>
        /// 全局变量名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 全局变量类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        [JsonIgnore]
        public object Value { get; set; }


    }

    /// <summary>
    /// 全局对象的数据类型
    /// </summary>
    public class ObjectType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        public const string String = "String";
        /// <summary>
        /// 整型
        /// </summary>
        public const string Int = "Int";
        /// <summary>
        /// 高精度浮点型
        /// </summary>
        public const string Double = "Double";
        /// <summary>
        /// 浮点型
        /// </summary>
        public const string Float = "Float";
        /// <summary>
        /// 布尔类型
        /// </summary>
        public const string Bool = "Bool";
        //public const string DateTime = "DateTime";
    }
}
