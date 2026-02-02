using LS.Flowchart.Tools;
using LS.WPF.Core.MVVM;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LS.Flowchart.ViewModels
{
    public partial class VM_MainWindow
    {

        #region 鼠标操作

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GlobalData.LastActivationTime = DateTime.Now;

            try
            {
                if (SelectLine != null)
                {
                    //判断是否在控件上，非起始控件
                    Point mousePos = Mouse.GetPosition(TabSelectItem.Content as Canvas);
                    //获取鼠标下的控件
                    var line = VisualTreeHelperExtensions.GetArrowPolylineUnderPoint(TabSelectItem.Content as Canvas, mousePos);
                    if (line == null)
                    {
                        //不在控件上，取消选中                    
                        SelectLine = null;
                    }
                }
            }
            catch(Exception ex)
            {
                LogOperate.Error("SelectLine 处理", ex);
            }
        }

        private void Window_TouchUp(object sender, TouchEventArgs e)
        {
            GlobalData.InstructionSign = false;
        }

        private void Window_TouchDown(object sender, TouchEventArgs e)
        {
            GlobalData.LastActivationTime = DateTime.Now;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsConnectPointMode)
            {
                //连线模式
                ComfirmConnectPoint();
                IsConnectPointMode = false;
            }
            if (Module != null)
            {
                AddModule();
            }
            GlobalData.InstructionSign = false;
        }

        /// <summary>
        /// 窗口监视鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsConnectPointMode)
            {
                DrawConnectPoint();
            }
            if (Module != null)
            {
                MoveModule();
            }
        }

        #endregion

    }
}
