using LS.Flowchart;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LS.Flowchart.Models.Configs;
using LS.Flowchart.Tools;
using LS.Flowchart.Views.Settings;

namespace LS.Flowchart.ViewModels.Settings
{
    public class VM_SoftSettingWindow:BaseViewModel
    {
        public VM_SoftSettingWindow():base(typeof(SoftSettingWindow))
        {
            (UIElement as SoftSettingWindow).winBar.MouseMove += WinBar_MouseMove;
        }

        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as SoftSettingWindow).DragMove();
            }
        }

        public override void LoadData()
        {
            try
            {
                base.LoadData();

                ShowInitWindow =GlobalData.ConfigParams.ShowInitWindow;
                IsAutoStart = GlobalData.ConfigParams.IsAutoStart;
                DelayStart = GlobalData.ConfigParams.DelayStart;
                IsDirectClose = GlobalData.ConfigParams.CloseType == SoftCloseType.DirectClose;
                IsBackgrounder = GlobalData.ConfigParams.CloseType == SoftCloseType.Backgrounder;
            }
            catch(Exception ex)
            {
                // 处理异常
                LogOperate.Error("加载数据时发生异常", ex);
            }   
        }


        public DelegateCommand CloseCommand
        {
            get { return new DelegateCommand(Close); }
        }

        private void Close(object obj)
        {
            this.Close();
        }


        public DelegateCommand ConfirmCommand
        {
            get { return new DelegateCommand(Confirm); }
        }

        private void Confirm(object obj)
        {
            try 
            {
                //开机自启操作
                if (IsAutoStart)
                {
                    StartupManager startupManager = new StartupManager();
                    if (!startupManager.IsStartupEnabled())
                    {
                        startupManager.EnableStartup();
                    }
                }
                else
                {
                    StartupManager startupManager = new StartupManager();
                    if (startupManager.IsStartupEnabled())
                    {
                        startupManager.DisableStartup();
                    }
                }

                GlobalData.ConfigParams.ShowInitWindow = ShowInitWindow;
                GlobalData.ConfigParams.IsAutoStart = IsAutoStart;
                GlobalData.ConfigParams.DelayStart = DelayStart;
                GlobalData.ConfigParams.CloseType = IsDirectClose ? SoftCloseType.DirectClose : SoftCloseType.Backgrounder;
                // 保存配置
                ConfigParamOperation.SaveConfigParam(GlobalData.ConfigParams);
                // 关闭窗口
                this.Close();
            }
            catch(Exception ex)
            {
                // 处理异常
                LogOperate.Error("确认操作时发生异常", ex);
            }
        }



        #region 属性

        private bool _ShowInitWindow;
        /// <summary>
        /// 是否显示初始界面
        /// </summary>
        public bool ShowInitWindow
        {
            get { return _ShowInitWindow; }
            set
            {
                _ShowInitWindow = value;
                OnPropertyChanged();
            }
        }

        private bool _IsAutoStart;
        /// <summary>
        /// 是否开机自启
        /// </summary>
        public bool IsAutoStart
        {
            get { return _IsAutoStart; }
            set
            {
                _IsAutoStart = value;
                OnPropertyChanged();
            }
        }

        private int _DelayStart;
        /// <summary>
        /// 延时启动
        /// </summary>
        public int DelayStart
        {
            get { return _DelayStart; }
            set
            {
                _DelayStart = value;
                OnPropertyChanged();
            }
        }

        private bool _IsDirectClose;
        private bool _IsBackgrounder;
        /// <summary>
        /// 直接关闭
        /// </summary>
        public bool IsDirectClose
        {
            get { return _IsDirectClose; }
            set
            {
                _IsDirectClose = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 后台运行
        /// </summary>
        public bool IsBackgrounder
        {
            get { return _IsBackgrounder; }
            set
            {
                _IsBackgrounder = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
