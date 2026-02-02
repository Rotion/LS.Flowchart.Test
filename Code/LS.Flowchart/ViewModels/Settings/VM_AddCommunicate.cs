using LS.Flowchart.Components.NetWork;
using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.Models.ProjectModel.Parameters;
using LS.Flowchart.ModuleControls;
using LS.Flowchart.Operation;
using LS.Flowchart.Tools;
using LS.Flowchart.Views.Settings;
using LS.Standard.Helper;
using LS.WPF.Core.Library;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using LS.WPF.Core.MVVM.StandardModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LS.Flowchart.ViewModels.Settings
{
    /// <summary>
    /// 添加通讯设备
    /// </summary>
    public class VM_AddCommunicate : BaseViewModel
    {

        public VM_AddCommunicate() : base(typeof(AddCommunicate))
        {
            (UIElement as AddCommunicate).winBar.MouseMove += WinBar_MouseMove;
        }

        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
            Task.Run(() => { Parameter_DataSource.GetCOM(); });
        }

        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as AddCommunicate).DragMove();
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

        public override void LoadData()
        {
            try
            {
                base.LoadData();

                //获取设备类型列表
                //反射枚举DeviceEnum获取属性
                DeviceTypeList.Clear();
                var deviceTypes = Enum.GetValues(typeof(Models.ProjectModel.DeviceEnum)).Cast<Models.ProjectModel.DeviceEnum>();
                foreach (var deviceType in deviceTypes)
                {
                    var enumName = EnumHelper.GetName(deviceType);
                    DeviceTypeList.Add(new DropDownModel
                    {
                        Name = enumName,
                        Value = ((int)deviceType).ToString(),
                        Content = deviceType
                    });
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("LoadData", ex);
            }

        }

        private int GetIndex()
        {
            if (GlobalData.CurrentProject.DeviceList.Count > 0)
            {
                return GlobalData.CurrentProject.DeviceList.Max(x => x.Index) + 1;
            }
            else { return 1; }
        }

        /// <summary>
        /// 根据选择的设备类型创建参数控件
        /// </summary>
        public void CreateParamer()
        {
            try
            {
                if (SelectedDeviceType == null)
                {
                    return;
                }

                DeviceName = $"{SelectedDeviceType.Name}{GetIndex()}";

                AddCommunicate win = UIElement as AddCommunicate;
                win.paramerListBox.Items.Clear();

                // 根据选择的设备类型创建参数控件
                var selectedDeviceType = SelectedDeviceType.Content as DeviceEnum?;
                if (selectedDeviceType == null)
                {
                    return;
                }

                switch (selectedDeviceType.Value)
                {
                    case DeviceEnum.TCP_Client:
                        DeviceParamer = new Parameter_TCPClient();
                        break;
                    case DeviceEnum.TCP_Server:
                        DeviceParamer = new Parameter_TCPServer();
                        break;
                    case DeviceEnum.COM:
                        DeviceParamer = new Parameter_COM();
                        break;
                    case DeviceEnum.UDP:
                        DeviceParamer = new Parameter_UDP();
                        break;
                }
                foreach (var property in DeviceParamer.GetType().GetProperties())
                {
                    if (property.IsDefined(typeof(ParameterControlAttribute)))//如果属性上有定义该属性,此步没有构造出实例
                    {
                        var attribute = property.GetCustomAttributes(typeof(ParameterControlAttribute))?.FirstOrDefault();
                        if (attribute != null)
                        {
                            var cfgPC = attribute as ParameterControlAttribute;

                            StackPanel stackPanel = new StackPanel();
                            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                            stackPanel.MinHeight = 50;
                            stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                            TextBlock textBlock = new TextBlock();
                            textBlock.Width = 130;
                            textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            textBlock.FontSize = 20;
                            textBlock.Foreground = (Brush)(new BrushConverter().ConvertFromString("#191970"));
                            textBlock.Text = cfgPC.Name;
                            stackPanel.Children.Add(textBlock);

                            string b_name = $"{nameof(DeviceParamer)}.{property.Name}";
                            //不同类型生成不同的控件
                            switch (cfgPC.ControlEnum)
                            {
                                case ParameterControlEnum.TextBox:
                                    TextBox textBox = new TextBox();
                                    textBox.Width = 230;
                                    textBox.Height = 40;
                                    textBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    textBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                    textBox.FontSize = 20;
                                    textBox.Foreground = (Brush)(new BrushConverter().ConvertFromString("#191970"));
                                    textBox.SetBinding(TextBlock.TextProperty, new Binding(b_name) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                    stackPanel.Children.Add(textBox);
                                    break;
                                case ParameterControlEnum.ComboBox:
                                    CommonCombox comboBox = new CommonCombox();
                                    comboBox.Width = 230;
                                    comboBox.Height = 40;
                                    comboBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    comboBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                    comboBox.CornerRadius = new CornerRadius(3);
                                    comboBox.FontSize = 20;
                                    comboBox.DisplayMemberPath = "Name";
                                    comboBox.SelectedValuePath = "Content";
                                    comboBox.Foreground = (Brush)(new BrushConverter().ConvertFromString("#191970"));
                                    comboBox.MoveBackground = Brushes.WhiteSmoke;
                                    comboBox.ItemsSource = Parameter_DataSource.DropDown_DataSource[cfgPC.DropDownSource];
                                    comboBox.SetBinding(CommonCombox.SelectedValueProperty, new Binding(b_name) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                    stackPanel.Children.Add(comboBox);
                                    break;
                                case ParameterControlEnum.CheckBox:
                                    SquareCheckBox checkBox = new SquareCheckBox();
                                    checkBox.TrueColor = UIElement.FindResource("OrangeBrush") as Brush;
                                    checkBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    checkBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                    checkBox.SetBinding(SquareCheckBox.IsCheckedProperty, new Binding(b_name) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                    stackPanel.Children.Add(checkBox);
                                    break;
                                case ParameterControlEnum.INT:
                                case ParameterControlEnum.Long:
                                case ParameterControlEnum.Double:
                                    LSNumber intNum = new LSNumber();
                                    intNum.Width = 230;
                                    intNum.Height = 40;
                                    intNum.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    intNum.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                    intNum.FontSize = 20;
                                    intNum.Foreground = (Brush)(new BrushConverter().ConvertFromString("#191970"));
                                    intNum.Precision = cfgPC.Precision;
                                    intNum.SetBinding(LSNumber.ValueProperty, new Binding(b_name) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                    stackPanel.Children.Add(intNum);
                                    break;
                                case ParameterControlEnum.IP:
                                    IPControl iPControl = new IPControl();
                                    iPControl.Width = 230;
                                    iPControl.Height = 40;
                                    iPControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    iPControl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                    iPControl.FontSize = 20d;
                                    iPControl.Foreground = (Brush)(new BrushConverter().ConvertFromString("#191970"));
                                    iPControl.SetBinding(IPControl.IPProperty, new Binding(b_name) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                    stackPanel.Children.Add(iPControl);
                                    break;
                                case ParameterControlEnum.EndChar:
                                    //结束符
                                    string endCharName = $"{nameof(DeviceParamer)}.{cfgPC.EndCharProperty}";
                                    EndCharControl endCharControl = new EndCharControl();
                                    endCharControl.Width = 230;
                                    endCharControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    endCharControl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                    endCharControl.FontSize = 20d;
                                    endCharControl.SetBinding(EndCharControl.IsStartProperty, new Binding(b_name) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                    endCharControl.SetBinding(EndCharControl.EndCharProperty, new Binding(endCharName) { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                                    stackPanel.Children.Add(endCharControl);
                                    break;
                            }

                            win.paramerListBox.Items.Add(stackPanel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("CreateParamer", ex);
            }
        }


        public DelegateCommand CreateCommand
        {
            get { return new DelegateCommand(Create); }
        }

        private void Create(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(DeviceName))
                {
                    VM_MainWindow.Popup("设备名称不能为空", "提示");
                    return;
                }
                if (SelectedDeviceType == null)
                {
                    VM_MainWindow.Popup("请选择设备类型", "提示");
                    return;
                }
                //创建设备
                var device = new ProjectDevice();
                device.DeviceId = IDHelper.GetGuidId();
                device.Index = GetIndex();
                device.DeviceType = (DeviceEnum)SelectedDeviceType.Content;
                device.IsActive = false;
                device.Name = DeviceName;
                device.DeviceParameter = DeviceParamer;

                var res = ProjectOperation.AddComponent(device);
                if (res)
                {
                    GlobalData.CurrentProject.DeviceList.Add(device);
                    this.Close();
                }
                else
                {
                    VM_MainWindow.Popup(res.Message);
                }

            }
            catch (Exception ex)
            {
                LogOperate.Error("Create", ex);
            }
        }

        #region 属性

        private ObservableCollection<DropDownModel> _deviceTypeList = new ObservableCollection<DropDownModel>();
        private DropDownModel _selectedDeviceType;
        /// <summary>
        /// 设备类型列表
        /// </summary>
        public ObservableCollection<DropDownModel> DeviceTypeList
        {
            get { return _deviceTypeList; }
            set { _deviceTypeList = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 选中的设备类型
        /// </summary>
        public DropDownModel SelectedDeviceType
        {
            get { return _selectedDeviceType; }
            set { _selectedDeviceType = value; OnPropertyChanged(); CreateParamer(); }
        }

        private string _deviceName;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; OnPropertyChanged(); }
        }

        private object _deviceParamer;

        /// <summary>
        /// 设备参数
        /// 按设备类型来赋值
        /// </summary>
        public object DeviceParamer
        {
            get { return _deviceParamer; }
            set { _deviceParamer = value; OnPropertyChanged(); }
        }
        #endregion
    }
}
