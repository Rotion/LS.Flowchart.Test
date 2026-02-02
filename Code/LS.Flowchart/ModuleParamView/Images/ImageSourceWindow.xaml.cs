using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LS.Flowchart.Views.Settings;

namespace LS.Flowchart.ModuleParamView.Images
{
    /// <summary>
    /// ImageSourceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageSourceWindow : Window,IModuleParamView
    {
        public ImageSourceWindow()
        {
            InitializeComponent();
            this.DataContext = new VM_ImageSourceWindow(this);
        }

        public void ShowView()
        {
            ShowDialog();
        }
    }

    public class VM_ImageSourceWindow : BaseViewModel
    {
        public VM_ImageSourceWindow(ImageSourceWindow win) : base(win)
        {
            (UIElement as ImageSourceWindow).winBar.MouseMove += WinBar_MouseMove;
        }

        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as ImageSourceWindow).DragMove();
            }
        }

        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
        }

        public override void LoadData()
        {
            base.LoadData();
        }

        public DelegateCommand CloseCommand
        {
            get { return new DelegateCommand(Close); }
        }

        private void Close(object obj)
        {
            this.Close();
        }
    }

    /// <summary>
    /// 图像源参数模型
    /// </summary>
    public class ImageSourceParam
    {

    }

}
