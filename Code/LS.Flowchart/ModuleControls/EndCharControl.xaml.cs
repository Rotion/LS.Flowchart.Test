using System.Windows;
using System.Windows.Controls;

namespace LS.Flowchart.ModuleControls
{
    /// <summary>
    /// EndCharControl.xaml 的交互逻辑
    /// </summary>
    public partial class EndCharControl : UserControl
    {
        public EndCharControl()
        {
            InitializeComponent();
           
            this.Loaded += EndCharControl_Loaded;
        }

        private void EndCharControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsStart)
            {
                EndCharVis = Visibility.Visible;
            }
            else
            {
                EndCharVis = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }
        public static readonly DependencyProperty IsStartProperty =
DependencyProperty.RegisterAttached("IsStart", typeof(bool), typeof(EndCharControl), new PropertyMetadata(false, IsStartValueChanged));

        private static void IsStartValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(IsStartProperty, (bool)e.NewValue);
        }


        /// <summary>
        /// 结束符
        /// </summary>
        public string EndChar
        {
            get { return (string)GetValue(EndCharProperty); }
            set { SetValue(EndCharProperty, value); }
        }
        public static readonly DependencyProperty EndCharProperty =
DependencyProperty.RegisterAttached("EndChar", typeof(string), typeof(EndCharControl), new PropertyMetadata("", EndCharValueChanged));

        private static void EndCharValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(EndCharProperty, (string)e.NewValue);
        }


        public Visibility EndCharVis
        {
            get { return (Visibility)GetValue(EndCharVisProperty); }
            set { SetValue(EndCharVisProperty, value); }
        }
        public static readonly DependencyProperty EndCharVisProperty =
DependencyProperty.RegisterAttached("EndCharVis", typeof(Visibility), typeof(EndCharControl), new PropertyMetadata(Visibility.Collapsed, EndCharVisValueChanged));

        private static void EndCharVisValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(EndCharVisProperty, (Visibility)e.NewValue);
        }

        private void R_Click(object sender, RoutedEventArgs e)
        {
            EndChar = @"\r";
        }

        private void N_Click(object sender, RoutedEventArgs e)
        {
            EndChar = @"\n";
        }

        private void RN_Click(object sender, RoutedEventArgs e)
        {
            EndChar = @"\r\n";
        }

        private void SquareCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EndCharVis = Visibility.Visible;
            this.Height = 130;
        }

        private void SquareCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EndCharVis = Visibility.Collapsed;
            this.Height = 40;
        }
    }
}
