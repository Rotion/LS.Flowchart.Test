using LS.Flowchart.Components.NetWork;
using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.Models.ProjectModel.Parameters;
using LS.Flowchart.ModuleControls;
using LS.Flowchart.Tools;
using LS.Flowchart.Views.Settings;
using LS.WPF.Core.Library;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LS.Flowchart.ViewModels.Settings
{
    public class VM_CommunicationWindow : BaseViewModel
    {

        public VM_CommunicationWindow() : base(typeof(CommunicationWindow))
        {
            (UIElement as CommunicationWindow).winBar.MouseMove += WinBar_MouseMove;
        }

        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as CommunicationWindow).DragMove();
            }
        }

        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
        }

        protected override void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            base.Page_Unloaded(sender, e);
            UnBindComponent();
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
                GetDeviceList();
                if (SelectedDevice == null)
                {
                    if (DeviceList != null && DeviceList.Count > 0)
                    {
                        SelectedDevice = DeviceList[0];
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("LoadData", ex);
            }
        }





        #region 设备管理

        /// <summary>
        /// 获取设备列表
        /// </summary>
        private void GetDeviceList()
        {
            try
            {
                DeviceList.Clear();
                var devices = GlobalData.CurrentProject.DeviceList;
                if (devices != null && devices.Count > 0)
                {
                    DeviceList = new ObservableCollection<ProjectDevice>(devices);
                }
                if (SelectedDevice == null)
                {
                    ShowDeviceParamer();
                }
                UnBindComponent();
                BindComponent();
            }
            catch (Exception ex)
            {
                LogOperate.Error("VM_CommunicationWindow.GetDeviceList", ex);
            }
        }

        /// <summary>
        /// 显示设备的参数
        /// </summary>
        private void ShowDeviceParamer()
        {
            try
            {
                if (SelectedDevice != null)
                {
                    object param = SelectedDevice.DeviceParameter;
                    string strParam = JsonConvert.SerializeObject(param);
                    switch (SelectedDevice.DeviceType)
                    {
                        case DeviceEnum.TCP_Client:
                            param = JsonConvert.DeserializeObject<Parameter_TCPClient>(strParam);
                            break;
                        case DeviceEnum.TCP_Server:
                            param = JsonConvert.DeserializeObject<Parameter_TCPServer>(strParam);
                            break;
                        case DeviceEnum.COM:
                            param = JsonConvert.DeserializeObject<Parameter_COM>(strParam);
                            break;
                        case DeviceEnum.UDP:
                            param = JsonConvert.DeserializeObject<Parameter_UDP>(strParam);
                            break;
                    }
                    //反向赋值 否则默认为JToken类型，会导致绑定有问题
                    SelectedDevice.DeviceParameter = param;
                    WrapPanel wrapPanel = (UIElement as CommunicationWindow).device_param;
                    wrapPanel.Children.Clear();
                    foreach (var property in param.GetType().GetProperties())
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
                                stackPanel.Width = 400;
                                stackPanel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                stackPanel.Margin = new Thickness(5);


                                TextBlock textBlock = new TextBlock();
                                textBlock.Width = 130;
                                textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                textBlock.FontSize = 20;
                                textBlock.Foreground = (Brush)(new BrushConverter().ConvertFromString("#191970"));
                                textBlock.Text = cfgPC.Name;
                                stackPanel.Children.Add(textBlock);

                                string b_name = $"{nameof(SelectedDevice)}.{nameof(SelectedDevice.DeviceParameter)}.{property.Name}";
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
                                        comboBox.MoveBackground = Brushes.LightBlue;
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
                                        string endCharName = $"{nameof(SelectedDevice)}.{nameof(SelectedDevice.DeviceParameter)}.{cfgPC.EndCharProperty}";
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

                                wrapPanel.Children.Add(stackPanel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ShowDeviceParamer", ex);
            }
        }

        /// <summary>
        /// 启用的状态变换
        /// </summary>
        /// <param name="sender">DeviceTag 对象</param>
        /// <param name="isStart">是否启动</param>
        public void OnStartChange(object sender, bool isStart)
        {
            try
            {
                if (SelectedDevice != null)
                {
                    DeviceTag deviceTag = (DeviceTag)sender;
                    if (deviceTag != null)
                    {
                        var module = deviceTag.ModuleValue;
                        if (module != null)
                        {
                            var comp = ProjectRunTime.Components.Find(x => x.DeviceId == module.DeviceId);
                            if (isStart)
                            {
                                if (!comp.IsSatrt())
                                {
                                    var res = comp.Start();
                                    if (!res)
                                    {
                                        VM_MainWindow.Popup(res.Message);
                                    }
                                }
                            }
                            else
                            {
                                if (comp.IsSatrt())
                                {
                                    var res = comp.Stop();
                                    if (!res)
                                    {
                                        VM_MainWindow.Popup(res.Message);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("OnStartChange", ex);
            }
        }
        /// <summary>
        /// 刷新触发 
        /// 当组件列表右键删除时/或重命名时
        /// </summary>
        public void OnRefresh(bool isDelete)
        {
            if (isDelete)
            {
                GetDeviceList();
            }
            else
            {
                (UIElement as CommunicationWindow)._deviceList.Items.Refresh();
            }
        }

        /// <summary>
        /// 绑定组件的通知消息
        /// </summary>
        private void BindComponent()
        {
            try
            {
                foreach (var comp in ProjectRunTime.Components)
                {
                    comp.OnRefresh += Comp_OnRefresh;
                    comp.OnStateChange += Comp_OnStateChange;
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("BindComponent", ex);
            }
        }

        /// <summary>
        /// 解绑组件的通知消息
        /// </summary>
        private void UnBindComponent()
        {
            try
            {
                foreach (var comp in ProjectRunTime.Components)
                {
                    comp.OnRefresh -= Comp_OnRefresh;
                    comp.OnStateChange -= Comp_OnStateChange;
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("BindComponent", ex);
            }
        }

        /// <summary>
        /// 组件设备状态变更时，刷新状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        private void Comp_OnStateChange(object sender, bool state)
        {
            DoMenthodByDispatcher(() =>
            {
                var devices = GlobalData.CurrentProject.DeviceList;
                if (devices != null && devices.Count > 0)
                {
                    DeviceList = new ObservableCollection<ProjectDevice>(devices);
                }
                (UIElement as CommunicationWindow)._deviceList.Items.Refresh();
            });
        }
        /// <summary>
        /// 刷新接收消息
        /// </summary>
        /// <param name="isDelete"></param>
        private void Comp_OnRefresh(bool isDelete)
        {
            RefreshReceive();
        }

        /// <summary>
        /// 消息发送接收操作
        /// </summary>
        public DelegateCommand MessageOperationCommand
        {
            get { return new DelegateCommand(MessageOperation); }
        }
        /// <summary>
        /// 消息发送接收操作
        /// </summary>
        private void MessageOperation(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var comp = ProjectRunTime.Components.Find(x => x.DeviceId == SelectedDevice?.DeviceId);
                    switch (obj.ToString())
                    {
                        case "ReceiveClear":
                            //清空消息接收
                            comp?.ClearRecord();
                            ReceiveMessages = "";
                            break;
                        case "SendClear":
                            //清空发送消息
                            SendMessages = "";
                            break;
                        case "Send":
                            //发送消息
                            var res = comp.Send(SendMessages, IsHEX_Send);
                            if (!res)
                            {
                                VM_MainWindow.Popup(res.Message);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("MessageOperation", ex);
            }
        }

        public DelegateCommand AddDeviceCommand
        {
            get { return new DelegateCommand(AddDevice); }
        }

        private void AddDevice(object obj)
        {
            try
            {
                VM_AddCommunicate vm = new VM_AddCommunicate();
                vm.ShowDialog();
                GetDeviceList();
            }
            catch (Exception ex)
            {
                LogOperate.Error("VM_CommunicationWindow.AddDevice", ex);
            }
        }

        /// <summary>
        /// 刷新接收数据
        /// </summary>
        private void RefreshReceive()
        {
            try
            {
                if (SelectedDevice != null)
                {
                    var comp = ProjectRunTime.Components.Find(x => x.DeviceId == SelectedDevice.DeviceId);
                    var dataList = comp.ResponseRecords;
                    StringBuilder builder = new StringBuilder();
                    foreach (var data in dataList)
                    {
                        string msg = $"{data.Index}# {(!string.IsNullOrEmpty(data.Source) ? $"[{data.Source}]" : "")}[{data.Time.ToString("yyyy-MM-dd HH:mm:ss.fff")}] ";
                        if (IsHEX_Receive)
                        {
                            var res = HexConverter.StringToHex(data.Message);
                            if (res)
                            {
                                msg += res.Tag.ToString();
                            }
                            else
                            {
                                msg += data.Message;
                            }
                        }
                        else
                        {
                            msg += data.Message;
                        }
                        builder.AppendLine(msg);
                    }
                    ReceiveMessages = builder.ToString();
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("RefreshReceive", ex);
            }
        }

        /// <summary>
        /// 刷新发送数据
        /// </summary>
        private void RefreshSend()
        {
            try
            {
                if (IsHEX_Send)
                {
                    var res = HexConverter.StringToHex(SendMessages);
                    if (res)
                    {
                        SendMessages = res.Tag.ToString();
                    }
                    else
                    {
                        VM_MainWindow.Popup(res.Message);
                    }
                }
                else
                {
                    var res = HexConverter.HexToString(SendMessages);
                    if (res)
                    {
                        SendMessages = res.Tag.ToString();
                    }
                    else
                    {
                        VM_MainWindow.Popup(res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("RefreshSend", ex);
            }
        }

        private ObservableCollection<ProjectDevice> _deviceList = new ObservableCollection<ProjectDevice>();
        private ProjectDevice _selectedDevice;
        private string _selectId;
        /// <summary>
        /// 设备列表
        /// </summary>
        public ObservableCollection<ProjectDevice> DeviceList
        {
            get { return _deviceList; }
            set { _deviceList = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 选择的设备
        /// </summary>
        public ProjectDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; OnPropertyChanged(); SelectId = _selectedDevice?.DeviceId; ShowDeviceParamer(); RefreshReceive(); }
        }
        /// <summary>
        /// 选择的设备的属性ID
        /// </summary>
        public string SelectId
        {
            get { return _selectId; }
            set { _selectId = value; OnPropertyChanged(); }
        }


        private string _receiveMessages;
        private string _sendMessages = "Test";
        /// <summary>
        /// 接收消息
        /// </summary>
        public string ReceiveMessages
        {
            get { return _receiveMessages; }
            set { _receiveMessages = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        public string SendMessages
        {
            get { return _sendMessages; }
            set { _sendMessages = value; OnPropertyChanged(); }
        }

        private bool _isHEX_Receive = false;
        private bool _isHEX_Send = false;
        /// <summary>
        /// 是否十六进制接收
        /// </summary>
        public bool IsHEX_Receive
        {
            get { return _isHEX_Receive; }
            set { _isHEX_Receive = value; OnPropertyChanged(); RefreshReceive(); }
        }
        /// <summary>
        /// 是否十六进制发送
        /// </summary>
        public bool IsHEX_Send
        {
            get { return _isHEX_Send; }
            set { _isHEX_Send = value; OnPropertyChanged(); RefreshSend(); }
        }


        #endregion
    }
}
