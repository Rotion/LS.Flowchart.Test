using LS.Flowchart.Models.ToolModels;
using LS.Flowchart.ViewModels;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace LS.Flowchart.UCControls
{
    /// <summary>
    /// 工具箱
    /// </summary>
    public partial class ToolsControl : UserControl, System.ComponentModel.INotifyPropertyChanged
    {
        public ToolsControl()
        {
            InitializeComponent();
        }


        public DelegateCommand ItemClickCommand
        {
            get { return new DelegateCommand(ItemClick); }
        }

        /// <summary>
        /// 工具箱，主菜单栏的点击事件
        /// 处理展开和闭合
        /// </summary>
        /// <param name="obj"></param>
        private void ItemClick(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var item = obj as ToolItemModel;
                    if (item != null)
                    {
                        int index = ItemSource.ToList().FindIndex(x => x.Name == item.Name);
                        if (index >= 0)
                        {
                            ItemSource[index].IsExpanded = !ItemSource[index].IsExpanded;
                            this._itemCtrl.Items.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ItemClick(object obj)", ex);
            }
        }

        /// <summary>
        /// 模块主图标，鼠标放上去后弹出工具窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseOn(object sender, MouseEventArgs e)
        {
            try
            {
                Grid grid = (Grid)sender;
                if (grid != null)
                {
                    foreach (var child in grid.Children)
                    {
                        if (child is Popup)
                        {
                            (child as Popup).IsOpen = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Path_MouseOn", ex);
            }
        }

        /// <summary>
        /// 模块主图标，鼠标移走关闭工具窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Grid grid = (Grid)sender;
                if (grid != null)
                {
                    foreach (var child in grid.Children)
                    {
                        if (child is Popup)
                        {
                            DispatcherTimer _timer = new DispatcherTimer();
                            _timer.Interval = TimeSpan.FromMilliseconds(200);
                            _timer.Tick += (s, ee) =>
                            {
                                //增加判断鼠标是否放在弹出窗口上
                                if (!VM_MainWindow.Instance.IsMouseOverControl((child as Popup).Child as Border))
                                {
                                    (child as Popup).IsOpen = false;
                                }
                                _timer.Stop();
                            };
                            _timer.Start();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Path_MouseLeave", ex);
            }
        }

        /// <summary>
        /// 弹出窗口，鼠标移走后关闭弹出窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Popup expandPopup = (Popup)sender;
                expandPopup.IsOpen = false;
            }
            catch (Exception ex)
            {
                LogOperate.Error("Popup_MouseLeave", ex);
            }
        }

        /// <summary>
        /// 模块被鼠标选中，并且按住后给对象赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SetValue(SelectModuleProperty, null);
                var grid = (Grid)sender;
                var data = grid.DataContext;
                if (data is ModuleItemModel)
                {
                    SetValue(SelectModuleProperty, data);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ModuleItem_MouseDown", ex);
            }
        }

        /// <summary>
        /// 模块被鼠标选中后释放，则清空选择对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SetValue(SelectModuleProperty, null);
            }
            catch (Exception ex)
            {
                LogOperate.Error("ModuleItem_MouseUp", ex);
            }
        }

        #region 数据集合
        /// <summary>
        ///展开控件的数据集合
        /// </summary>
        public ObservableCollection<ToolItemModel> ItemSource
        {
            get { return (ObservableCollection<ToolItemModel>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); OnPropertyChanged(); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
DependencyProperty.RegisterAttached("ItemSource", typeof(ObservableCollection<ToolItemModel>), typeof(ToolsControl), new PropertyMetadata(new ObservableCollection<ToolItemModel>(), ItemSourceValueChanged));

        private static void ItemSourceValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(ItemSourceProperty, (ObservableCollection<ToolItemModel>)e.NewValue);
        }

        #endregion

        #region 是否展开
        /// <summary>
        ///是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); OnPropertyChanged(); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(ToolsControl), new PropertyMetadata(true, IsExpandedValueChanged));

        private static void IsExpandedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(IsExpandedProperty, (bool)e.NewValue);
        }

        #endregion

        #region 当前选择的模块

        public ModuleItemModel SelectModule
        {
            get { return (ModuleItemModel)GetValue(SelectModuleProperty); }
            set { SetValue(SelectModuleProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty SelectModuleProperty =
DependencyProperty.RegisterAttached("SelectModule", typeof(ModuleItemModel), typeof(ToolsControl), new PropertyMetadata(null, SelectModuleValueChanged));

        private static void SelectModuleValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(SelectModuleProperty, (ModuleItemModel)e.NewValue);
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
