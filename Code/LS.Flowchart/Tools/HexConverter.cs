using LS.Standard.Data;
using LS.WPF.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LS.Flowchart.Tools
{
    public static class HexConverter
    {
        /// <summary>
        /// 字符串转十六进制（每两位用空格分隔）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="encoding">编码格式（默认ASCII）</param>
        public static BaseResult StringToHex(string input, Encoding encoding = null)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                    return new BaseResult(true, "", input);

                // 默认编码处理
                if (encoding == null)
                    encoding = Encoding.ASCII;

                byte[] bytes = encoding.GetBytes(input);
                StringBuilder hex = new StringBuilder(bytes.Length * 3); // 预留空格位置

                for (int i = 0; i < bytes.Length; i++)
                {
                    // 非首字节时添加空格分隔
                    if (i > 0)
                        hex.Append(' ');

                    // X2格式：两位大写十六进制，不足补零
                    hex.AppendFormat("{0:X2}", bytes[i]);
                }
                return new BaseResult(true, "", hex.ToString());
            }
            catch(Exception ex)
            {
                LogOperate.Error("StringToHex", ex);
                return new BaseResult(false, ex.Message);
            }
        }


        /// <summary>
        /// 十六进制转字符串（UTF-8编码）
        /// </summary>
        public static BaseResult HexToString(string hexInput, Encoding encoding = null)
        {
            try
            {
                if (string.IsNullOrEmpty(hexInput))
                    return new BaseResult(true, "", hexInput);

                // 默认编码处理
                if (encoding == null)
                    encoding = Encoding.ASCII;

                hexInput = hexInput.Replace(" ", "");

                if (!IsHexString(hexInput))
                    return new BaseResult(false, "请输入 0-9 a-f A-F范围内的字符");

                if (hexInput.Length % 2 != 0)
                    return new BaseResult(false, "十六进制字符串长度必须为偶数,可以在数值前补0");

                byte[] bytes = new byte[hexInput.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    // 截取每两个字符并转换为字节
                    string hexByte = hexInput.Substring(i * 2, 2);
                    bytes[i] = Convert.ToByte(hexByte, 16); // 16表示十六进制基数
                }
                string result= encoding.GetString(bytes);
                return new BaseResult(true, "", result);
            }
            catch(Exception ex)
            {
                LogOperate.Error("HexToString", ex);
                return new BaseResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 验证字符串是否符合十六进制字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsHexString(string input)
        {
            // 正则：仅允许0-9、A-F、a-f，且整个字符串由这些字符组成
            return Regex.IsMatch(input, @"^[0-9A-Fa-f]+$");
        }
    }



    }
