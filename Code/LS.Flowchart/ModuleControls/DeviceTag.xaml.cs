
using LS.Flowchart;
using LS.WPF.Core.MVVM;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.Tools;

namespace LS.Flowchart.ModuleControls
{
    /// <summary>
    /// DeviceTag.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceTag : UserControl, System.ComponentModel.INotifyPropertyChanged
    {
        public DeviceTag()
        {
            InitializeComponent();
            this.Loaded += DeviceTag_Loaded;
            this.MouseRightButtonDown += DeviceTag_MouseRightButtonDown;
        }

        private void DeviceTag_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var control = (DeviceTag)sender;
                ContextMenu menu = new ContextMenu();
                menu.Background = this.FindResource("DefaultContextMenuBackground") as SolidColorBrush;
                menu.Foreground = this.FindResource("DefaultContextMenuForeground") as SolidColorBrush;
                menu.FontSize = 13;
                if (control.IsMouseOver) // 条件判断
                {
                    MenuItem delete = new MenuItem { Header = "删除" };
                    delete.Click += Delete_Click;
                    menu.Items.Add(delete);

                    MenuItem rename = new MenuItem { Header = "重命名" };
                    rename.Click += Rename_Click;
                    menu.Items.Add(rename);
                }
                control.ContextMenu = menu;
            }
            catch(Exception ex)
            {
                LogOperate.Error("DeviceTag_MouseRightButtonDown", ex);
            }
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //弹出小窗口进行重命名
                RenameWindow renameWindow = new RenameWindow("名称修改","设备名称",ModuleValue.Name);
                var res= renameWindow.ShowDialog();
                if (res == true) 
                {
                    ModuleValue.Name = renameWindow.NewContent;
                    OnRefresh?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Rename_Click", ex);
            }
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var index = GlobalData.CurrentProject.DeviceList.FindIndex(x => x.DeviceId == ModuleValue.DeviceId);
            if (index >= 0)
            {
                GlobalData.CurrentProject.DeviceList.RemoveAt(index);
                OnRefresh?.Invoke(true);
            }
        }

        private void DeviceTag_Loaded(object sender, RoutedEventArgs e)
        {
            IsStart = ModuleValue.IsActive;            
        }

        public delegate void DelegateStartChange(object sender, bool isStart);
        public delegate void DelegateRefresh(bool isDelete);
        /// <summary>
        /// 启用属性变更
        /// </summary>
        public event DelegateStartChange OnStartChange;
        /// <summary>
        /// 刷新触发
        /// </summary>
        public event DelegateRefresh OnRefresh;
        /// <summary>
        /// 选中项改变触发
        /// </summary>
        public void SelectChange()
        {
            try 
            {
                if (SelectID == ModuleValue.DeviceId)
                {
                    this.Background = this.FindResource("DefaultSelectBrush") as Brush;
                }
                else
                {
                    this.Background = this.FindResource("TabItem.ActivedBackground") as Brush;
                }
            }
            catch(Exception ex)
            {

            }
        }

        #region 属性

        /// <summary>
        /// 当前模块的属性
        /// </summary>
        public ProjectDevice ModuleValue
        {
            get { return (ProjectDevice)GetValue(ModuleValueProperty); }
            set { SetValue(ModuleValueProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty ModuleValueProperty =
DependencyProperty.RegisterAttached("ModuleValue", typeof(ProjectDevice), typeof(DeviceTag), new PropertyMetadata(null, ModuleValueValueChanged));

        private static void ModuleValueValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(ModuleValueProperty, (ProjectDevice)e.NewValue);
        }


        public string  SelectID
        {
            get { return (string)GetValue(SelectIDProperty); }
            set { SetValue(SelectIDProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty SelectIDProperty =
DependencyProperty.RegisterAttached("SelectID", typeof(string), typeof(DeviceTag), new PropertyMetadata(null, SelectIDValueChanged));

        private static void SelectIDValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(SelectIDProperty, (string)e.NewValue);
            (obj as DeviceTag).SelectChange();
        }


        private bool _isStart = false;
        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsStart
        {
            get { return _isStart; }
            set
            {
                _isStart = value;
                OnPropertyChanged();

                ModuleValue.IsActive = IsStart;
                SetValue(ModuleValueProperty, ModuleValue);

                OnStartChange?.Invoke(this,_isStart);
            }
        }


        #endregion





        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
