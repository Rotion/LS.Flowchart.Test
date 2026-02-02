using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.StandardModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace LS.Flowchart.Models.ProjectModel.Parameters
{
    public class Parameter_DataSource
    {
        /// <summary>
        /// 参数控件的下拉框数据源
        /// </summary>
        public static Dictionary<string, List<DropDownModel>> DropDown_DataSource = new Dictionary<string, List<DropDownModel>>();
        /// <summary>
        /// 数据源的Key
        /// </summary>
        public static class DataSourceKey
        {
            /// <summary>
            /// 串口号
            /// </summary>
            public const string COM = "COM";
            /// <summary>
            /// 波特率
            /// </summary>
            public const string BaudRate = "BaudRate";
            /// <summary>
            /// 数据位
            /// </summary>
            public const string DataBits = "DataBits";
            /// <summary>
            /// 停止位
            /// </summary>
            public const string StopBits = "StopBits";
            /// <summary>
            /// 校验位
            /// </summary>
            public const string Parity = "Parity";

            /// <summary>
            /// 全局变量 数据类型
            /// </summary>
            public const string ObjectType = "ObjectType";

        }

        /// <summary>
        /// 初始化下拉框数据源
        /// </summary>
        public static void InitDataSource()
        {
            try
            {
                // 初始化下拉框数据源
                DropDown_DataSource.Clear();


                //串口号
                GetCOM();
                //波特率
                GetBaudRate();
                //数据位
                GetDataBits();
                //停止位
                GetStopBits();
                //校验位
                GetParity();

                //全局变量 数据类型
                GetObjectType();
            }
            catch (Exception ex)
            {
                LogOperate.Error("Parameter_DataSource.InitDataSource", ex);
            }
        }

        /// <summary>
        /// 获取串口号源
        /// </summary>
        public static void GetCOM()
        {
            try
            {
                string key = DataSourceKey.COM;
                if (!DropDown_DataSource.ContainsKey(key))
                {
                    DropDown_DataSource.Add(key, new List<DropDownModel>());
                }
                DropDown_DataSource[key].Clear();
                //获取所有串口号
                var coms = System.IO.Ports.SerialPort.GetPortNames();
                if (coms != null && coms.Length > 0)
                {
                    foreach (var com in coms)
                    {
                        DropDown_DataSource[key].Add(new DropDownModel() { Name = com, Code = com, Value = com, Content = com });
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetCOM", ex);
            }
        }

        /// <summary>
        /// 获取波特率源
        /// </summary>
        /// <param name="key"></param>
        private static void GetBaudRate()
        {
            try
            {
                string key = DataSourceKey.BaudRate;
                if (!DropDown_DataSource.ContainsKey(key))
                {
                    DropDown_DataSource.Add(key, new List<DropDownModel>());
                }
                DropDown_DataSource[key].Clear();
                //获取所有波特率
                var dataList = new int[] { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 128000, 256000 };
                if (dataList != null && dataList.Length > 0)
                {
                    foreach (var data in dataList)
                    {
                        DropDown_DataSource[key].Add(new DropDownModel() { Name = data.ToString(), Code = data.ToString(), Value = data.ToString(), Content = data });
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetBaudRate", ex);
            }
        }

        /// <summary>
        /// 获取数据位源
        /// </summary>
        private static void GetDataBits()
        {
            try
            {
                string key = DataSourceKey.DataBits;
                if (!DropDown_DataSource.ContainsKey(key))
                {
                    DropDown_DataSource.Add(key, new List<DropDownModel>());
                }
                DropDown_DataSource[key].Clear();

                var dataList = new int[] { 5, 6, 7, 8 };
                if (dataList != null && dataList.Length > 0)
                {
                    foreach (var data in dataList)
                    {
                        DropDown_DataSource[key].Add(new DropDownModel() { Name = data.ToString(), Code = data.ToString(), Value = data.ToString(), Content = data });
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetDataBits", ex);
            }
        }

        /// <summary>
        /// 获取停止位源
        /// </summary>
        private static void GetStopBits()
        {
            try
            {
                string key = DataSourceKey.StopBits;
                if (!DropDown_DataSource.ContainsKey(key))
                {
                    DropDown_DataSource.Add(key, new List<DropDownModel>());
                }
                DropDown_DataSource[key].Clear();
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "无", Code = "None", Value = "None", Content = StopBits.None });
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "1", Code = "One", Value = "One", Content = StopBits.One });
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "1.5", Code = "OnePointFive", Value = "OnePointFive", Content = StopBits.OnePointFive });
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "2", Code = "Two", Value = "Two", Content = StopBits.Two });

            }
            catch (Exception ex)
            {
                LogOperate.Error("GetStopBits", ex);
            }
        }

        /// <summary>
        /// 获取校验位源
        /// </summary>
        private static void GetParity()
        {
            try
            {
                string key = DataSourceKey.Parity;
                if (!DropDown_DataSource.ContainsKey(key))
                {
                    DropDown_DataSource.Add(key, new List<DropDownModel>());
                }
                DropDown_DataSource[key].Clear();
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "无", Code = "None", Value = "None", Content = Parity.None });
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "奇校验 Odd", Code = "Odd", Value = "Odd", Content = Parity.Odd });
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "偶校验 Even", Code = "Even", Value = "Even", Content = Parity.Even });
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "标志位 Mark", Code = "Mark", Value = "Mark", Content = Parity.Mark });
                DropDown_DataSource[key].Add(new DropDownModel() { Name = "空格位 Space", Code = "Space", Value = "Space", Content = Parity.Space });
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetParity", ex);
            }
        }

        /// <summary>
        /// 全局变量 数据类型
        /// </summary>
        private static void GetObjectType()
        {
            try
            {
                string key = DataSourceKey.ObjectType;
                if (!DropDown_DataSource.ContainsKey(key))
                {
                    DropDown_DataSource.Add(key, new List<DropDownModel>());
                }
                DropDown_DataSource[key].Clear();
                var typeList = new string[] { "int", "float", "string", "bool" };
                if (typeList != null && typeList.Length > 0)
                {
                    foreach (var type in typeList)
                    {
                        DropDown_DataSource[key].Add(new DropDownModel() { Name = type, Code = type, Value = type, Content = type });
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetObjectType", ex);
            }
        }
    }
}
