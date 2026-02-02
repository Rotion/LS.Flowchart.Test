using System;
using LS.Flowchart.Models.Configs;
using LS.Flowchart.Models.Configs;
using LS.Flowchart.Models.ProjectModel;
using LS.WPF.Core.Library;
using LS.WPF.Core.MVVM;

namespace LS.Flowchart
{
    public static class GlobalData
    {

        /// <summary>
        /// 客户端本地配置信息
        /// </summary>
        public static P_Environment ConfigParams { get; set; } = new P_Environment();
        /// <summary>
        /// 软件版本号
        /// </summary>
        public static string Version { get; private set; } = "V1.0.1.20250716";
        /// <summary>
        /// 设备SN码
        /// </summary>
        public static string SN { get; set; }
        /// <summary>
        /// 授权失败消息
        /// </summary>
        public static string AuthorizationMessage { get; set; }

        /// <summary>
        /// 最近打开的文件
        /// </summary>
        public static P_HistoryFiles HistoryFiles { get; set; } = new P_HistoryFiles();

        /// <summary>
        /// 添加历史文件记录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filePath"></param>
        public static void AddHistoryFile(string name,string filePath)
        {
            try
            {
                var index= GlobalData.HistoryFiles.HistoryFileList.FindIndex(x=>x.FileName==name && x.FilePath == filePath);
                if(index >= 0)
                {
                    //如果已经存在，则更新打开时间
                    GlobalData.HistoryFiles.HistoryFileList[index].OpenTime = DateTime.Now;
                }
                else
                {
                    //如果不存在，则添加新的记录
                    GlobalData.HistoryFiles.HistoryFileList.Add(new HistoryFile()
                    {
                        FileName = name,
                        FilePath = filePath,
                        OpenTime = DateTime.Now
                    });
                }
                //保存到配置文件
                ConfigParamOperation.SaveHistoryFiles(GlobalData.HistoryFiles);
            }
            catch (Exception ex)
            {
                LogOperate.Error("GlobalData.AddHistoryFile", ex);
            }
        }

        /// <summary>
        /// 方案初始化完成标志
        /// </summary>
        public static bool InitProjectFinish { get; set; } = false; 

        /// <summary>
        /// 当前的方案
        /// </summary>
        public static ProjectData CurrentProject { get; set; }

        #region 路径类

        /// <summary>
        /// 日志首目录
        /// </summary>
        public static string Path_Logs = AppDomain.CurrentDomain.BaseDirectory + "Logs\\";
        /// <summary>
        /// 客户端日志路径
        /// </summary>
        public static string Client_Path_Logs = Path_Logs + "ClientLogs\\";


        //
        // 摘要:
        //     配置文件总路径
        public static string Path_Config => AppDomain.CurrentDomain.BaseDirectory + "Config\\";
        /// <summary>
        /// 客户端配置文件路径
        /// </summary>
        public static string Path_Config_Environment = Path_Config + "P_Environment.cfg";
        /// <summary>
        /// 客户端备份的配置文件路径
        /// </summary>
        public static string Path_Config_Environment_Backup = Path_Config + "P_Environment_Backup.cfg";
        /// <summary>
        /// 历史文件记录路径
        /// </summary>
        public static string Path_P_HistoryFiles = Path_Config + "P_HistoryFiles.cfg";
        #endregion

        #region 界面缓存

        /// <summary>
        /// 弹窗对象
        /// </summary>
        public static LSNoBorderMessageBox Win_Popup;

        /// <summary>
        /// 指令的全局对象
        /// True = 为执行  False = 为释放
        /// </summary>
        public static bool InstructionSign { get; set; } = false;

        /// <summary>
        /// 最后一次激活的时间
        /// </summary>
        public static DateTime LastActivationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 初始界面选择的初始项目
        /// </summary>
        public static ProjectType InitProjectType { get; set; } = ProjectType.Blank;

        #endregion

    }
}
