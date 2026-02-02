using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Flowchart.Models.ProjectModel.Parameters
{
    /// <summary>
    /// 参数输入类型
    /// </summary>
    public enum ParameterControlEnum
    {
        /// <summary>
        /// 文本框
        /// </summary>
        TextBox = 1,
        /// <summary>
        /// 下拉框
        /// </summary>
        ComboBox = 2,
        /// <summary>
        /// 复选框
        /// </summary>
        CheckBox = 3,
        /// <summary>
        /// 整型数字输入框
        /// </summary>
        INT = 4,
        /// <summary>
        /// IP类型
        /// </summary>
        IP = 5,
        /// <summary>
        /// 浮点型数字输入框
        /// </summary>
        Double = 6,
        /// <summary>
        /// 长整型数字输入框
        /// </summary>
        Long = 7,
        /// <summary>
        /// 结束符
        /// </summary>
        EndChar = 8,
    }
}
