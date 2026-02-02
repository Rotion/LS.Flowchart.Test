using LS.Flowchart.Models.Configs;
using LS.Flowchart.Operation;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LS.Flowchart.ViewModels
{
    public partial class VM_MainWindow
    {
        #region 窗口操作

        /// <summary>
        /// 窗口状态切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win_StateChanged(object sender, EventArgs e)
        {
            //if ((UIElement as MainWindow).WindowState == System.Windows.WindowState.Minimized)
            //{
            //    (UIElement as MainWindow).ShowInTaskbar = false;
            //}
            //else
            //{
            //    (UIElement as MainWindow).ShowInTaskbar = true;
            //}
        }

        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as MainWindow).DragMove();
            }
        }

        private void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(GlobalData.ConfigParams.CloseType == SoftCloseType.DirectClose)
            {
                ThreadOperate.OnExit();
                return;
            }
        }

        private void Win_Closed(object sender, EventArgs e)
        {
            if (GlobalData.ConfigParams.CloseType == SoftCloseType.DirectClose)
            {
                Environment.Exit(0);
                return;
            }
            else
            {
                (VM_MainWindow.Instance.UIElement as MainWindow).WindowState = System.Windows.WindowState.Minimized;
                (UIElement as MainWindow).ShowInTaskbar = false;
            }
        }

        #endregion


        #region 图标方法

        /// <summary>
        /// 图标触发方法
        /// </summary>
        public DelegateCommand NotifyIconCommand
        {
           get { return new DelegateCommand(NotifyIconOperation); }
        }

        private void NotifyIconOperation(object obj)
        {
            try 
            {
                if (obj != null)
                {
                    switch (obj.ToString())
                    {
                        case "ShowWindows":
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
                            break;
                        case "Exit":
                            ThreadOperate.OnExit();
                            Environment.Exit(0);
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                LogOperate.Error("NotifyIconOperation", ex);
            }
        }

        #endregion
    }
}
