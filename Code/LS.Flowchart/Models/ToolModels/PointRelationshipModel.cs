namespace LS.Flowchart.Models.ToolModels
{
    /// <summary>
    /// 连接点关系数据结构
    /// </summary>
    public class PointRelationshipModel
    {
        /// <summary>
        ///起始点 模块，或控件的ID
        /// </summary>
        public string StartPointID { get; set; }
        /// <summary>
        /// 起始点的位置
        /// </summary>
        public PathLocation StartPointLocation { get; set; }

        /// <summary>
        ///结束点 模块，或控件的ID
        /// </summary>
        public string EndPointID { get; set; }
        /// <summary>
        /// 结束点的位置
        /// </summary>
        public PathLocation EndPointLocation { get; set; }

    }
}
