using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System;
using System.Windows;
using LS.Flowchart.Models.Configs;
using LS.Flowchart.ViewModels.Settings;

namespace LS.Flowchart.ViewModels
{
    public partial class VM_MainWindow : BaseViewModel
    {
        public static VM_MainWindow Instance { get; private set; }
        public VM_MainWindow(MainWindow win) : base(win)
        {

#if DEBUG
            win.Topmost = false;
#endif
            win.Closing += Win_Closing;
            win.Closed += Win_Closed;
            win.MouseMove += Window_MouseMove;
            win.MouseDown += Window_MouseDown;
            win.MouseUp += Window_MouseUp;
            win.TouchDown += Window_TouchDown;
            win.TouchUp += Window_TouchUp;
            win.winBar.MouseMove += WinBar_MouseMove;
            win.StateChanged += Win_StateChanged; ;
            Instance = this;
        }



        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
#if DEBUG
            (UIElement as MainWindow).Topmost = false;
#endif

        }

        public override void LoadData()
        {
            base.LoadData();
            InitTools();
            InitProcess();
        }

        /// <summary>
        /// 操作方法
        /// </summary>
        public DelegateCommand OperationCommand
        {
            get { return new DelegateCommand(Operation); }
        }
        /// <summary>
        /// 操作方法
        /// </summary>
        /// <param name="obj"></param>
        private void Operation(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    LogOperate.ClickLog("点击了-操作方法【" + obj.ToString() + "】");
                    switch (obj.ToString())
                    {
                        case "MinWindow":
                            (UIElement as MainWindow).WindowState = WindowState.Minimized;
                            break;
                        case "MaxWindow":
                            if ((UIElement as MainWindow).WindowState == WindowState.Maximized)
                                (UIElement as MainWindow).WindowState = WindowState.Normal;
                            else
                                (UIElement as MainWindow).WindowState = WindowState.Maximized;
                            break;
                        case "CloseWindow":

                            if (GlobalData.ConfigParams.CloseType == SoftCloseType.DirectClose)
                            {
                                var res3 = Popup3(message: "方案未保存，是否进行保存后退出？", isOnly: true);
                                switch (res3)
                                {
                                    case WPF.Core.Library.ThreeResult.Cancel:
                                        return;
                                    case WPF.Core.Library.ThreeResult.Yes:
                                        VM_MainWindow.Instance.SaveProject();
                                        break;
                                }                                
                                (UIElement as MainWindow).Close();
                                VM_AuthorizationWindow.Instance.Close();
                                return;
                            }
                            else
                            {
                                (UIElement as MainWindow).WindowState = WindowState.Minimized;
                            }

                            break;
                        case "SaveProject":
                            SaveProject();
                            break;
                        case "SaveAs":
                            SaveAs();
                            break;
                        case "OpenProject":
                            OpenProject();
                            break;
                        case "SoftSetting":
                            //软件设置
                            VM_SoftSettingWindow softSetting = new VM_SoftSettingWindow();
                            softSetting.ShowDialog();
                            break;
                        case "Communication":
                            //通信管理
                            VM_CommunicationWindow communicationWindow = new VM_CommunicationWindow();
                            communicationWindow.ShowDialog();
                            break;
                        case "GlobalObject":
                            //全局变量
                            VM_GlobalObjectWindow globalObjectWindow = new VM_GlobalObjectWindow();
                            globalObjectWindow.ShowDialog();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Operation", ex);
            }
        }



        #region 属性






        #endregion

    }
}
