namespace LS.Flowchart.Models.ProjectModel
{
    public enum ProjectType
    {
        /// <summary>
        /// 空白方案
        /// </summary>
        Blank,
        /// <summary>
        /// 定位测量
        /// </summary>
        Location,
        /// <summary>
        /// 缺陷检测
        /// </summary>
        Defect,
        /// <summary>
        /// 扫码识别
        /// </summary>
        QrCode,
        /// <summary>
        /// 根据方案文件加载
        /// </summary>
        FromFile
    }
}
