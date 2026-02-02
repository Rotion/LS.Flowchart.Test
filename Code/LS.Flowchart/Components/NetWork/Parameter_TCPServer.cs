using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Flowchart.Models.ProjectModel.Parameters;

namespace LS.Flowchart.Components.NetWork
{
    /// <summary>
    /// TCP服务端的参数
    /// </summary>
    public class Parameter_TCPServer
    {
        /// <summary>
        /// 本机IP
        /// </summary>
        [ParameterControl("本机IP", ParameterControlEnum.IP)]
        public string LocalIP { get; set; } = "127.0.0.1";

        /// <summary>
        /// 本机端口
        /// </summary>
        [ParameterControl("本机端口", ParameterControlEnum.INT)]
        public int LocalPort { get; set; } = 5050;


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
