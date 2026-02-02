using LS.Flowchart.Models.Configs;
using LS.Flowchart.ViewModels;
using LS.WPF.Core.MVVM;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Brush = System.Windows.Media.Brush;

namespace LS.Flowchart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        private static string W_ID = "359988E0-PCZD-420F-B401-F8A68004E60A";
        private static System.Threading.Mutex mutex = null;

        [System.Runtime.InteropServices.DllImport("User32", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        static extern IntPtr GetDesktopWindow();

        private int curProcressID = -1;
        private string curProcressName = "";
        private string logPath = GlobalData.Client_Path_Logs;
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                bool isStartupBoot = e.Args.Length > 0; // 检查参数

                curProcressID = Process.GetCurrentProcess().Id;
                curProcressName = Process.GetCurrentProcess().ProcessName;
                //初始化日志组件
                LogOperate.InitLog(curProcressID, logPath);
                LogOperate.Start("--------------------------------------------------------------------");
                LogOperate.Start("启动.." + curProcressID);
                bool flag = false;
                mutex = new System.Threading.Mutex(true, W_ID, out flag);

                if (!flag)
                {
                    LogOperate.Start("找到互斥线程。");
                    //等待2秒
                    System.Threading.Thread.Sleep(2000);
                    int isContinue = -1;
                    //寻找客户端进程
                    Process[] app = Process.GetProcessesByName(curProcressName);
                    LogOperate.Start("查询已启动线程：" + curProcressName + "..数量：" + app.Length.ToString());
                    if (app.Length > 0)
                    {
                        //判断是否有窗口打开,如果有进程却无窗口显示，则关闭进程后，继续启动当前程序
                        foreach (var a in app)
                        {
                            if (a.MainWindowHandle == IntPtr.Zero)
                            {
                                int killId = -1;
                                //获取线程ID
                                try
                                {
                                    killId = a.Id;
                                    if (killId == curProcressID)//当前线程是自己，跳过
                                        continue;
                                    LogOperate.Start(string.Format("获取线程[{0}]...启动时间:{1:HH:mm:ss.ffff}", killId, a.StartTime));
                                }
                                catch (Exception ex)
                                {
                                    LogOperate.Start(string.Format("获取线程ID...失败:{0}", ex.Message));
                                }
                                //杀掉线程
                                try
                                {
                                    if (killId > 0)
                                    {
                                        LogOperate.Start(string.Format("杀掉线程[{0}]...", killId));
                                        try { a.CloseMainWindow(); a.Close(); System.Threading.Thread.Sleep(1000); } catch { }
                                        a.Kill();
                                    }
                                }
                                catch (Exception ex) { LogOperate.Start(string.Format("杀掉线程[{0}]失败:{1}", killId, ex.Message)); }
                            }
                            else
                            {
                                isContinue = a.Id;
                            }
                        }
                    }
                    if (isContinue > 0)
                    {
                        LogOperate.Start(string.Format("当前进程有效[{0}]，退出本次启动。", isContinue));
                        LogOperate.Save();
                        Environment.Exit(0);//退出程序  
                        return;
                    }
                }

                //主窗体实例化及窗口显示之前，不能进行弹窗
                //主窗体实例化
                while (GetDesktopWindow() == IntPtr.Zero)
                {
                    LogOperate.Start("未找到桌面句柄，重试...");
                    System.Threading.Thread.Sleep(100);
                }
                System.Threading.Thread.Sleep(100);

                try
                {
                    var res = ConfigParamOperation.ReadConfigParam(out P_Environment p);
                    if (res.IsSucessed)
                    {
                        GlobalData.ConfigParams = p;
                    }
                    else
                    {
                        GlobalData.ConfigParams = ConfigParamOperation.GetDefaultConfig();
                        var cfg = GlobalData.ConfigParams;
                        ConfigParamOperation.SaveConfigParam(cfg);
                    }

                    if (GlobalData.ConfigParams == null)
                    {
                        GlobalData.ConfigParams = ConfigParamOperation.GetDefaultConfig();
                        var cfg = GlobalData.ConfigParams;
                        ConfigParamOperation.SaveConfigParam(cfg);
                    }

                    //ConfigParamOperation.SaveHistoryFiles(new WPFClient.Models.Configs.P_HistoryFiles()
                    //{
                    //    HistoryFileList = new System.Collections.Generic.List<WPFClient.Models.Configs.HistoryFile>()
                    //    {
                    //        new WPFClient.Models.Configs.HistoryFile(){ FileName="测试方案1",FilePath="D:\\测试方案1.prj",OpenTime=Convert.ToDateTime("2025-07-18 11:54:30") },
                    //        new WPFClient.Models.Configs.HistoryFile(){ FileName="测试方案2",FilePath="D:\\测试方案2.prj",OpenTime=Convert.ToDateTime("2025-07-18 10:54:30") },
                    //        new WPFClient.Models.Configs.HistoryFile(){ FileName="测试方案3",FilePath="D:\\测试方案3.prj",OpenTime=Convert.ToDateTime("2025-07-17 11:54:30") },
                    //        new WPFClient.Models.Configs.HistoryFile(){ FileName="测试方案4",FilePath="D:\\测试方案4.prj",OpenTime=Convert.ToDateTime("2025-07-16 11:54:30") },
                    //        new WPFClient.Models.Configs.HistoryFile(){ FileName="测试方案5",FilePath="D:\\测试方案5.prj",OpenTime=Convert.ToDateTime("2025-07-14 11:54:30") },
                    //        new WPFClient.Models.Configs.HistoryFile(){ FileName="测试方案6",FilePath="D:\\测试方案6.prj",OpenTime=Convert.ToDateTime("2025-06-18 11:54:30") },
                    //    }
                    //});
                }
                catch (Exception ex)
                {
                    GlobalData.ConfigParams = ConfigParamOperation.GetDefaultConfig();
                    LogOperate.Error("读取配置文件异常  -- 配置文件丢失", ex);
                }

                if (isStartupBoot)
                {
                    LogOperate.Start("开机自启触发，延时启动");
                    Thread.Sleep(GlobalData.ConfigParams.DelayStart * 1000);
                }

                //InitializeNotifyIcon();
                Task.Run(() =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(GlobalData.ConfigParams.ThemeColor))
                        {
                            Application.Current.Resources["ThemeColor"] = (Brush)(new BrushConverter().ConvertFromString(GlobalData.ConfigParams.ThemeColor));
                        }
                    }
                    catch { }

                    while (true)
                    {
                        if (VM_AuthorizationWindow.Instance != null)
                        {
                            //ViewModels.VM_MainWindow.Instance.Hide();
                            new System.Threading.Thread(() => Operation.ThreadOperate.OnStart()) { IsBackground = true }.Start();
                            break;
                        }
                        Thread.Sleep(500);
                    }
                });
            }
            catch (Exception ex)
            {
                LogOperate.Start("OnStartup_Exception Exit " + ex.ToString());
                LogOperate.Save();
                Environment.Exit(0);
            }
        }

        protected override void OnLoadCompleted(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
            Current.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }


        /// <summary>
        /// UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (e.Exception != null)
                {
                    LogOperate.Error("App_OnDispatcherUnhandledException Exception", e.Exception);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("App_OnDispatcherUnhandledException", ex);
            }
            LogOperate.Save();
        }

        /// <summary>
        /// 非UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    LogOperate.Error("UnhandledExceptionEventArgs Exception", exception);
                }
                else if (e.ExceptionObject != null)
                {
                    LogOperate.Error("UnhandledExceptionEventArgs Exception:" + e.ExceptionObject.ToString());
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("CurrentDomain_UnhandledException", ex);
            }
            finally
            {
                LogOperate.Save();
            }
        }

        /*
        #region 托盘图标
        private NotifyIcon _notifyIcon;

        public void InitializeNotifyIcon()
        {
            try
            {
                _notifyIcon = new NotifyIcon
                {
                    Icon = ConvertBytesToIcon(WPFClient.Properties.Resources.logo),
                    Visible = true,
                    Text = "Vision App",
                    ContextMenu = new ContextMenu(new MenuItem[]
                    {
                        new MenuItem("显示主窗口", (s, e) => ShowWindow()),
                        new MenuItem("退出", (s, e) =>
                        {
                            ThreadOperate.OnExit();
                            Environment.Exit(0);
                        })
                    })

                };
                _notifyIcon.MouseDoubleClick += (s, e) => ShowWindow();
            }
            catch (Exception ex)
            {
                LogOperate.Error("InitializeNotifyIcon Exception", ex);
            }
        }

        public Icon ConvertBytesToIcon(byte[] iconBytes)
        {
            using (MemoryStream ms = new MemoryStream(iconBytes))
            {
                return new Icon(ms); // 直接构造Icon对象
            }
        }

        private void ShowWindow()
        {
            try
            {
                if (VM_MainWindow.Instance?.UIElement != null)
                {
                    if ((VM_MainWindow.Instance?.UIElement as MainWindow).WindowState == WindowState.Minimized)
                    {
                        (VM_MainWindow.Instance?.UIElement as MainWindow).WindowState = WindowState.Maximized;
                        (VM_MainWindow.Instance?.UIElement as MainWindow).Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ShowWindow Exception", ex);
            }
        }

        #endregion
        */
    }

}
