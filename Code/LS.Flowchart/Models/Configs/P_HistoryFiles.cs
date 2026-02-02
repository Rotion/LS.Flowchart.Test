using System;
using System.Collections.Generic;

namespace LS.Flowchart.Models.Configs
{
    [Serializable]
    public class P_HistoryFiles
    {
        /// <summary>
        /// 打开的历史文件
        /// </summary>
        public List<HistoryFile> HistoryFileList { get; set; } = new List<HistoryFile>();
    }

    /// <summary>
    /// 历史打开文件
    /// </summary>
    public class HistoryFile
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 最后一次打开的时间
        /// </summary>
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 时间描述
        /// </summary>
        public string TimeDescription { get; set; }
    }
}
