using LS.Standard.Data;
using System.Collections.Generic;
using LS.Flowchart.Components.ComponentModels;
using LS.Flowchart.Models.ProjectModel;
using static LS.Flowchart.Components.ComponentDelegates;
using static LS.Flowchart.ModuleControls.DeviceTag;

namespace LS.Flowchart.Components
{
    public interface IComponent
    {
        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        string DeviceId { get; }
        /// <summary>
        /// 设备信息
        /// </summary>
        ProjectDevice Device { get; }
        /// <summary>
        /// 设备类型
        /// </summary>
        DeviceEnum DeviceType { get; }
        /// <summary>
        /// 查询是否已启动
        /// </summary>
        /// <returns></returns>
        bool IsSatrt();
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        BaseResult Start();
        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        BaseResult Stop();
        /// <summary>
        /// 清空记录的消息内容
        /// </summary>
        /// <returns></returns>
        BaseResult ClearRecord();
        /// <summary>
        /// 状态变更通知
        /// </summary>
        event DelegateOnStateChange OnStateChange;
        /// <summary>
        /// 接收消息的记录
        /// </summary>
        List<MessageModel> ResponseRecords { get; set; }
        /// <summary>
        /// 接收消息变化时，刷新触发
        /// </summary>
        event DelegateRefresh OnRefresh;
        /// <summary>
        /// 消息回调
        /// </summary>
        event DelegateOnResponse OnResponse;
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="isHEX">是否十六进制发送</param>
        /// <returns></returns>
        BaseResult Send(string content, bool isHEX = false);

    }
}
