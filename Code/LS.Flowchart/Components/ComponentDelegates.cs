namespace LS.Flowchart.Components
{
    public class ComponentDelegates
    {
        /// <summary>
        /// 接收消息的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        public delegate void DelegateOnResponse(object sender, string response);

        /// <summary>
        /// 组件状态变更通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        public delegate void DelegateOnStateChange(object sender, bool state);

    }
}
