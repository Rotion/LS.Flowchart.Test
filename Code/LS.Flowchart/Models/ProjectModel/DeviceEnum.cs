using LS.Standard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Flowchart.Models.ProjectModel
{
    /// <summary>
    /// 设备的类型
    /// 协议类型
    /// </summary>
    public enum DeviceEnum
    {
        /// <summary>
        /// TCP 客户端
        /// </summary>
        [EnumName("TCP 客户端")]
        TCP_Client = 10,

        /// <summary>
        /// TCP 服务端
        /// </summary>
        [EnumName("TCP 服务端")]
        TCP_Server = 11,

        /// <summary>
        /// UDP
        /// </summary>
        [EnumName("UDP")]
        UDP = 12,

        /// <summary>
        /// 串口
        /// </summary>
        [EnumName("串口")]
        COM = 20,

    }

}
