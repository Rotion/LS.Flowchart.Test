using LS.WPF.Core.MVVM;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;
using LS.Flowchart.Models.Configs;
using LS.Flowchart.Models.ProjectModel.Parameters;
using LS.Flowchart.Operation;

namespace LS.Flowchart.Operation
{
    public static class ThreadOperate
    {
        /// <summary>
        /// 程序启动时的工作任务
        /// </summary>
        public static void OnStart()
        {
            try
            {
                //权限界面显示
                //VM_MainWindow.Instance.Hide();
                //VM_MainWindow.Instance.DoMenthodByDispatcher(() => 
                //{
                //    AuthorizationWindow authorization = new AuthorizationWindow();
                //    authorization.Show();
                //});

                ConfigParamOperation.ReadHistoryFiles(out P_HistoryFiles files);
                GlobalData.HistoryFiles = files;


                Parameter_DataSource.InitDataSource();
            }
            catch (Exception ex)
            {
                LogOperate.Error("ThreadOperate.OnStart", ex);
            }
        }

        /// <summary>
        /// 程序退出时的工作任务
        /// </summary>
        public static void OnExit()
        {
            try
            {
                //处理一些结束时需要做的任务
                ProjectOperation.StopProject();
            }
            catch (Exception ex)
            {
                LogOperate.Error("ThreadOperate.OnExit", ex);
            }
            finally
            {
                //保存日志
                LogOperate.Start("退出程序【" + Process.GetCurrentProcess().Id + "】");
                LogOperate.Save();
            }
        }

    }
}
