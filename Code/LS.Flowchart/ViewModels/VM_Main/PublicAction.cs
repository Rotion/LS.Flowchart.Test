using LS.WPF.Core.Library;
using LS.WPF.Core.MVVM;
using System.Windows;
using System.Windows.Input;

namespace LS.Flowchart.ViewModels
{
    public partial class VM_MainWindow
    {
        #region 公共方法

        private static bool isPopup = false;
        /// <summary>
        /// 弹出单按钮对话框 永远返回true
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Popup(string message = "", string title = "提示", string comfirm = "确认")
        {
            if (isPopup)
                return true;
            isPopup = true;
            bool res = true;
            VM_AuthorizationWindow.Instance.DoMenthodByDispatcher(() =>
            {
                LogOperate.ClickLog(string.Format("弹窗(未点击)-{0}", message));
                LSNoBorderMessageBox box = new LSNoBorderMessageBox(title, message, comfirm, "", 1);
                GlobalData.Win_Popup = box;
                res = box.ShowDialog() == true;
                LogOperate.ClickLog(string.Format("弹窗-{0}-{1}", comfirm, message));
                isPopup = false;
                GlobalData.Win_Popup = null;
            }, false);
            return res;
        }

        private static bool isPopup2 = false;
        /// <summary>
        /// 弹出双按钮对话框
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="title">文本</param>
        /// <param name="comfirm">确认按钮</param>
        /// <param name="cancel">取消按钮</param>
        /// <param name="isOnly">是否独占，独占的话其他同时弹出的都返回False</param>
        /// <returns></returns>
        public static bool Popup2(string message = "", string title = "提示", string comfirm = "确认", string cancel = "取消", bool isOnly = false)
        {
            if (isOnly && isPopup2)
                return false;
            isPopup2 = true;
            bool res = true;
            VM_AuthorizationWindow.Instance.DoMenthodByDispatcher(() =>
            {
                LogOperate.ClickLog(string.Format("弹窗(未点击)-{0}", message));
                LSNoBorderMessageBox box = new LSNoBorderMessageBox(title, message, comfirm, cancel, 2);
                res = box.ShowDialog() == true;
                LogOperate.ClickLog(string.Format("弹窗-{0}-{1}", res ? comfirm : cancel, message));
            }, false);
            isPopup2 = false;
            return res;

        }

        private static bool isPopup3 = false;
        /// <summary>
        /// 弹出三按钮对话框
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="title">文本</param>
        /// <param name="comfirm">确认按钮</param>
        /// <param name="cancel">取消按钮</param>
        /// <param name="isOnly">是否独占，独占的话其他同时弹出的都返回False</param>
        /// <returns></returns>
        public static ThreeResult Popup3(string message = "", string title = "提示", string yes = "是", string no = "否", string cancel = "取消", bool isOnly = false)
        {
            if (isOnly && isPopup3)
                return ThreeResult.Cancel;
            isPopup3 = true;
            ThreeResult res = ThreeResult.Cancel;
            VM_AuthorizationWindow.Instance.DoMenthodByDispatcher(() =>
            {
                LogOperate.ClickLog(string.Format("弹窗(未点击)-{0}", message));
                LSThreeMessageBox box = new LSThreeMessageBox(title, message, yes, no, cancel);
                box.ShowDialog();
                res = box.DialogResult;
                LogOperate.ClickLog(string.Format("弹窗-{0}-{1}", res.ToString(), message));
            }, false);
            isPopup3 = false;
            return res;

        }



        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="type">提示类型</param>
        public static void Tip(string message, TipType type = TipType.Info)
        {
            VM_AuthorizationWindow.Instance.DoMenthodByDispatcher(() =>
            {
                MessageTipWindow win = new MessageTipWindow(message, type);
                //VM_MessagePopupWindow win = new VM_MessagePopupWindow(message, type);
                win.Top = 0;
                win.Left = 710;
                win.Show();
            });
        }


        #region 鼠标与控件

        /// <summary>
        /// 判断鼠标是否在控件上方
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsMouseOverControl(UIElement element)
        {
            Point mousePos = Mouse.GetPosition(UIElement);
            Point controlPos = element.TranslatePoint(new Point(), UIElement);
            Rect controlRect = new Rect(controlPos, element.RenderSize);
            bool isInside = controlRect.Contains(mousePos);
            return isInside;
        }


        #endregion

        #endregion
    }
}
