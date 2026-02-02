using System.IO.Ports;
using LS.Flowchart.Models.ProjectModel.Parameters;
using static LS.Flowchart.Models.ProjectModel.Parameters.Parameter_DataSource;

namespace LS.Flowchart.Components.NetWork
{
    /// <summary>
    /// 串口协议的参数
    /// </summary>
    public class Parameter_COM
    {   /// <summary>
        /// 串口号
        /// </summary>
        [ParameterControl("串口号", ParameterControlEnum.ComboBox, dropDownSource: DataSourceKey.COM)]
        public string PortName { get; set; } = "COM1";
        /// <summary>
        /// 波特率
        /// </summary>
        [ParameterControl("波特率", ParameterControlEnum.ComboBox, dropDownSource: DataSourceKey.BaudRate)]
        public int BaudRate { get; set; } = 9600;
        /// <summary>
        /// 数据位
        /// </summary>
        [ParameterControl("数据位", ParameterControlEnum.ComboBox, dropDownSource: DataSourceKey.DataBits)]
        public int DataBits { get; set; } = 8;
        /// <summary>
        /// 停止位
        /// </summary>
        [ParameterControl("停止位", ParameterControlEnum.ComboBox, dropDownSource: DataSourceKey.StopBits)]
        public StopBits StopBits { get; set; } = StopBits.One;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        [ParameterControl("奇偶校验", ParameterControlEnum.ComboBox, dropDownSource: DataSourceKey.Parity)]
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// 超时时间
        /// </summary>
        [ParameterControl("超时时间(ms)", ParameterControlEnum.INT)]
        public int TimeOut { get; set; } = 10;

        /// <summary>
        /// 自动重连
        /// </summary>
        [ParameterControl("自动重连", ParameterControlEnum.CheckBox)]
        public bool AutoReconnection { get; set; } = false;

        /// <summary>
        /// 接收结束符
        /// </summary>
        [ParameterControl("接收结束符", ParameterControlEnum.EndChar, endCharProperty: "EndChar")]
        public bool HasEndChar { get; set; } = false;

        /// <summary>
        /// 结束符
        /// </summary>
        public string EndChar { get; set; } = "";
    }
}
