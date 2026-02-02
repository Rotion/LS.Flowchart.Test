using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Flowchart.Models.ProjectModel.Parameters;

namespace LS.Flowchart.Components.NetWork
{
    /// <summary>
    /// TCP客户端的参数
    /// </summary>
    public class Parameter_TCPClient
    {
        /// <summary>
        /// 目标IP
        /// </summary>
        [ParameterControl("目标IP",ParameterControlEnum.IP)]
        public string TargetIP { get; set; } = "127.0.0.1";

        /// <summary>
        /// 目标端口
        /// </summary>
        [ParameterControl("目标端口", ParameterControlEnum.INT)]
        public int TargetPort { get; set; } = 5020;

        /// <summary>
        /// 自动重连
        /// </summary>
        [ParameterControl("自动重连", ParameterControlEnum.CheckBox)]
        public bool AutoReconnection { get; set; } = false;

        /// <summary>
        /// 接收结束符
        /// </summary>
        [ParameterControl("接收结束符", ParameterControlEnum.EndChar,endCharProperty: "EndChar")]
        public bool HasEndChar { get; set; } = false;

        /// <summary>
        /// 结束符
        /// </summary>
        public string EndChar { get; set; } = "";

    }
}
