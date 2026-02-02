using LS.Standard.Data;
using LS.Standard.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LS.Flowchart.Tools
{
    /// <summary>
    /// 授权操作
    /// </summary>
    public static class AuthorizationHelper
    {

        #region 授权加密

        [Serializable]
        public class LicenseModel
        {
            /// <summary>
            /// SN加密后的数据
            /// </summary>
            public string Device { get; set; }

            /// <summary>
            /// 过期时间
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// 防篡改签名
            /// </summary>
            public string Signature { get; set; }
        }

        private static string KEY = "BBA89F3CFAED43C608AF1";
        private static string IV = "A8C05383CADC0";
        private static string S_KEY = "6BD9DD56707144DEA";
        private static string S_IV = "BBFDA398122DEE3";
        private static string NONCE = "pczd$%#198";
        private static string USER = "Rotion";

        /// <summary>
        /// 生成用户名加密
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenerateUser(string name)
        {
            string user = AESEncript.Encrypt(name, KEY, IV);
            return user;
        }

        /// <summary>
        /// 生成加密文件数据
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static LicenseModel GenerateLicense(string user, string sn, DateTime expiry)
        {
            string name = AESEncript.Decrypt(user, KEY, IV);
            if (name != USER)
            {
                return null;
            }

            string device = AESEncript.Encrypt(sn, KEY, IV);
            LicenseModel model = new LicenseModel();
            model.Device = device;
            model.Date = expiry;
            model.Signature = GenerateSignature(device, expiry);
            return model;
        }

        /// <summary>
        /// 解析授权
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        public static BaseResult DecodeLicense(string sn, LicenseModel license)
        {
            string device = AESEncript.Encrypt(sn, KEY, IV);
            if (device != license.Device)
            {
                return new BaseResult(false, "设备与授权码不匹配，请联系管理员");
            }

            string sign = GenerateSignature(license.Device, license.Date);
            if (license.Signature == sign)
            {
                if (license.Date < DateTime.Now)
                {
                    return new BaseResult(false, "授权过期");
                }
                return BaseResult.Successed;
            }
            else
            {
                return new BaseResult(false, "授权验证失败");
            }
        }

        /// <summary>
        /// 生成放混淆签名
        /// </summary>
        /// <param name="device"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        private static string GenerateSignature(string device, DateTime expiry)
        {
            string time = expiry.ToString("yyyyMMddHHmmssfff");
            string content = device + NONCE + time;
            string sgin = AESEncript.Encrypt(content, S_KEY, S_IV);
            return sgin;
        }


        #endregion



        #region 获取SN码相关

        public static string GetHardwareId()
        {
            var sb = new StringBuilder();
            sb.Append(GetHardwareInfo("Win32_Processor", "ProcessorId")); // CPU
            sb.Append(GetHardwareInfo("Win32_BaseBoard", "SerialNumber")); // 主板
            sb.Append(GetHardwareInfo("Win32_BIOS", "SerialNumber")); // BIOS
            sb.Append(GetMacAddress()); // MAC
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public static string GetCPUSN()
        {
            var sb = new StringBuilder();
            sb.Append(GetHardwareInfo("Win32_Processor", "ProcessorId")); // CPU
            return sb.ToString();
        }

        public static string GetBoardSN()
        {
            var sb = new StringBuilder();
            sb.Append(GetHardwareInfo("Win32_BaseBoard", "SerialNumber")); // 主板
            return sb.ToString();
        }



        private static string GetHardwareInfo(string path, string key)
        {
            var searcher = new ManagementObjectSearcher($"SELECT {key} FROM {path}");
            foreach (ManagementObject obj in searcher.Get())
                return obj[key]?.ToString() ?? "";
            return "";
        }

        private static string GetMacAddress()
        {
            var network = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up);
            return network?.GetPhysicalAddress().ToString() ?? "";
        }

        #endregion


    }
}
