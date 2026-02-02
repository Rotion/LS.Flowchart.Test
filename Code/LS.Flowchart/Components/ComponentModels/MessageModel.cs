using System;

namespace LS.Flowchart.Components.ComponentModels
{
    /// <summary>
    /// 消息模型
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// 索引号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
    }
}
