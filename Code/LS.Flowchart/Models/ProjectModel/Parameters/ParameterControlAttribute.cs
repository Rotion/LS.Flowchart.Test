using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Flowchart.Models.ProjectModel.Parameters
{
    public class ParameterControlAttribute : Attribute
    {
        /// <summary>
        /// 控件类型
        /// </summary>
        public ParameterControlEnum ControlEnum { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 精度,针对浮点型数据有效
        /// </summary>
        public int Precision { get; set; } = 0;

        /// <summary>
        /// 下拉框数据源标识
        /// </summary>
        public string DropDownSource { get; set; }

        /// <summary>
        /// 结束符的字段名称
        /// </summary>
        public string EndCharProperty { get; set; }


        public ParameterControlAttribute(string name, ParameterControlEnum controlEnum = ParameterControlEnum.TextBox, int precision = 0, string dropDownSource = "",string endCharProperty="")
        {
            Name = name;
            ControlEnum = controlEnum;
            Precision = precision;
            DropDownSource = dropDownSource;
            EndCharProperty=endCharProperty;
        }
    }
}
