using LS.Flowchart.Models.Configs;

namespace LS.Flowchart.Models.Configs
{
    public class P_Environment
    {
        /// <summary>
        /// 是否显示初始界面
        /// </summary>
        public bool ShowInitWindow { get; set; } = true;

        /// <summary>
        /// 主题色  十六进制
        /// </summary>
        public string ThemeColor { get; set; }

        /// <summary>
        /// 是否开机自启
        /// </summary>
        public bool IsAutoStart { get; set; } = false;

        /// <summary>
        /// 延时启动
        /// </summary>
        public int DelayStart { get; set; } = 0;

        /// <summary>
        /// 软件关闭操作
        /// </summary>
        public SoftCloseType CloseType { get; set; } = SoftCloseType.DirectClose;   
    }
}
