using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LS.Flowchart.Tools
{
    public static class IPHelper
    {


        /// <summary>
        /// 用NetworkInterface获取IP地址
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static string GetLocalIPAddressWithNetworkInterface(NetworkInterfaceType _type)
        {
            string ip = "";
            var ips = NetworkInterface.GetAllNetworkInterfaces();
            if (ips != null && ips.Length > 0)
            {
                foreach (var item in ips)
                {
                    if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation uip in item.GetIPProperties().UnicastAddresses)
                        {
                            if (uip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ip = uip.Address.ToString();
                                break;
                            }
                        }
                    }
                }
            }
            return ip;
        }

        /// <summary>
        /// 取本机主机ip
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {

                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ip = "";
                        ip = IpEntry.AddressList[i].ToString();
                        return ip;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /// <summary>
        /// IP地址转换为数字
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <returns></returns>
        public static long IpToLong(string ip)
        {
            long IntIp = 0;
            string[] ips = ip.Split('.');
            IntIp = long.Parse(ips[0]) << 0x18 | long.Parse(ips[1]) << 0x10 | long.Parse(ips[2]) << 0x8 | long.Parse(ips[3]);
            return IntIp;

        }

        /// <summary>
        /// 数字(long)转换为IP地址
        /// </summary>
        /// <param name="num">数字</param>
        /// <returns></returns>
        public static string LongToIp(long num)
        {
            long IntIp = num;
            StringBuilder sb = new StringBuilder();
            sb.Append(IntIp >> 0x18 & 0xff).Append(".");
            sb.Append(IntIp >> 0x10 & 0xff).Append(".");
            sb.Append(IntIp >> 0x8 & 0xff).Append(".");
            sb.Append(IntIp & 0xff);
            return sb.ToString();
        }
    }
}
