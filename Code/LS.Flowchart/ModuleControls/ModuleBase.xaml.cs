using LS.Flowchart.Models.ToolModels;
using LS.Flowchart.ModuleParamView;
using LS.Flowchart.Tools;
using LS.Flowchart.UCControls;
using LS.Flowchart.ViewModels;
using LS.WPF.Core.MVVM;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LS.Flowchart.ModuleControls
{
    /// <summary>
    /// ModuleBase.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleBase : UserControl, System.ComponentModel.INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; this.Uid = _id; this.Name = _id; }
        }

        /// <summary>
        /// 新建一个模块对象
        /// </summary>
        public ModuleBase()
        {
            InitializeComponent();
            ID = IDHelper.GetGuidName();
            InitEvent();
        }
        /// <summary>
        /// 新建一个模块对象
        /// </summary>
        /// <param name="id">传入ID值</param>
        public ModuleBase(string id)
        {
            InitializeComponent();
            Name = ID = id;
            InitEvent();
        }
        /// <summary>
        /// 初始化事件绑定
        /// </summary>
        private void InitEvent()
        {
            this.Loaded += ModuleBase_Loaded;
            this.MouseDoubleClick += ModuleBase_MouseDoubleClick;
            this.MouseLeftButtonDown += ModuleBase_MouseDown;
            this.MouseMove += ModuleBase_MouseMove;
            this.MouseLeftButtonUp += ModuleBase_MouseUp;
            this.PreviewMouseLeftButtonUp += ModuleBase_MouseUp;
            this.MouseRightButtonDown += ModuleBase_MouseRightButtonDown;
            this.PreviewKeyDown += ModuleBase_PreviewKeyDown;
            this.MouseEnter += ModuleBase_MouseEnter;
            this.MouseLeave += ModuleBase_MouseLeave;

            this._up.MouseLeave += Path_MouseLeave;
            this._up.MouseEnter += Path_MouseEnter;
            this._up.MouseDown += Path_MouseDown;
            this._down.MouseLeave += Path_MouseLeave;
            this._down.MouseEnter += Path_MouseEnter;
            this._down.MouseDown += Path_MouseDown;
            this._left.MouseLeave += Path_MouseLeave;
            this._left.MouseEnter += Path_MouseEnter;
            this._left.MouseDown += Path_MouseDown;
            this._right.MouseLeave += Path_MouseLeave;
            this._right.MouseEnter += Path_MouseEnter;
            this._right.MouseDown += Path_MouseDown;


        }



        #region 连接点功能

        public delegate void EnterConnectPointMode(ModuleBase sender, PathLocation location, ModuleItemModel moduleItem);
        public event EnterConnectPointMode OnEnterConnectPointMode;

        // ModuleBase 控件内部的方法
        public Point GetPositionRelativeToCanvas()
        {
            // 获取自身在父容器（Canvas）中的坐标
            double left = Canvas.GetLeft(this); // X 坐标
            double top = Canvas.GetTop(this);   // Y 坐标
            return new Point(left, top);
        }

        /// <summary>
        /// 获取连接点的坐标位置
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Point GetPathLocation(PathLocation location)
        {
            try
            {
                var point = GetPositionRelativeToCanvas();
                var width = this.Width;
                var height = this.Height;

                switch (location)
                {
                    case PathLocation.Top:
                        //上端点，初始点 X方向+width/2
                        point.X = point.X + (width / 2d);
                        break;
                    case PathLocation.Bottom:
                        //下端点  初始点 X方向+width/2 Y方向+height
                        point.X = point.X + (width / 2d);
                        point.Y = point.Y + height;
                        break;
                    case PathLocation.Left:
                        //左端点  初始点 Y方向+height/2
                        point.Y = point.Y + (height / 2d);
                        break;
                    case PathLocation.Right:
                        //右端点 初始点 X方向+width  Y方向+height/2
                        point.X = point.X + width;
                        point.Y = point.Y + (height / 2d);
                        break;
                }
                return point;
            }
            catch (Exception ex)
            {
                LogOperate.Error("GetPathLocation", ex);
                return Mouse.GetPosition((UIElement)this.Parent);
            }
        }

        /// <summary>
        /// 连接点，鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PathLocation location = PathLocation.Bottom;

            var path = (Path)sender;
            if (path.Name == this._up.Name)
            {
                location = PathLocation.Top;
            }
            else if (path.Name == this._down.Name)
            {
                location = PathLocation.Bottom;
            }
            else if (path.Name == this._left.Name)
            {
                location = PathLocation.Left;
            }
            else if (path.Name == this._right.Name)
            {
                location = PathLocation.Right;
            }

            OnEnterConnectPointMode?.Invoke(this, location, ModuleItem);
        }

        /// <summary>
        /// 连接点，鼠标移动到上方
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Cross;
            isOnPath = true;
        }
        /// <summary>
        /// 连接点，鼠标移走
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            isOnPath = false;
        }

        /// <summary>
        /// 鼠标移出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleBase_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsMouseOver)
            {
                this._up.Visibility = Visibility.Collapsed;
                this._down.Visibility = Visibility.Collapsed;
                this._left.Visibility = Visibility.Collapsed;
                this._right.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ModuleBase_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsMouseOver)
            {
                this._up.Visibility = Visibility.Visible;
                this._down.Visibility = Visibility.Visible;
                this._left.Visibility = Visibility.Visible;
                this._right.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 模块响应按键的功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ModuleBase_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    //删除
                    if (this.Parent is Canvas)
                    {
                        (this.Parent as Canvas).Children.Remove(this);
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ModuleBase_PreviewKeyDown", ex);
            }
        }

        private void ModuleBase_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region 右键功能处理

        /// <summary>
        /// 右键点击，弹出菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleBase_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = (ModuleBase)sender;
            ContextMenu menu = new ContextMenu();
            menu.Background = this.FindResource("DefaultContextMenuBackground") as SolidColorBrush;
            menu.Foreground = this.FindResource("DefaultContextMenuForeground") as SolidColorBrush;
            menu.FontSize = 13;
            if (control.IsMouseOver) // 条件判断
            {
                if (ModuleItem.LowerLevelPoints.Count > 0)
                {
                    //断开连接
                    //只能从上级断下级，也就是显示下层关系
                    MenuItem DisconnectPoint = new MenuItem { Header = "断开连接" };
                    var canvas = this.Parent as Canvas;
                    var modules = canvas.FindVisualChildren<ModuleBase>().ToList();
                    foreach (var itemPoint in ModuleItem.LowerLevelPoints)
                    {
                        string id = itemPoint.EndPointID;
                        var end = modules.Find(x => x.ID == id);
                        if (end != null)
                        {
                            MenuItem DisconnectItem = new MenuItem { Header = end.ModuleItem.Name };
                            DisconnectItem.Tag = itemPoint;
                            DisconnectItem.Click += DisconnectItem_Click;
                            DisconnectPoint.Items.Add(DisconnectItem);
                        }
                    }

                    menu.Items.Add(DisconnectPoint);
                }


                MenuItem delete = new MenuItem { Header = "删除" };
                delete.Click += Delete_Click;
                menu.Items.Add(delete);

                MenuItem rename = new MenuItem { Header = "重命名" };
                rename.Click += Rename_Click; ;
                menu.Items.Add(rename);
            }
            control.ContextMenu = menu;
        }

        private void DisconnectItem_Click(object sender, RoutedEventArgs e)
        {
            //断开连接

            try
            {
                var canvas = this.Parent as Canvas;
                var points = canvas.FindVisualChildren<ArrowPolyline>().ToList();
                var itemPoint = (sender as MenuItem).Tag as PointRelationshipModel;
                if (itemPoint != null)
                {
                    string id = $"{itemPoint.StartPointID}{itemPoint.EndPointID}";
                    if (points.Exists(x => x.ID == id))
                    {
                        canvas.Children.Remove(points.Find(x => x.ID == id));
                        ModuleItem.LowerLevelPoints.Remove(itemPoint);
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("断开连接", ex);
            }
        }

        private bool isRename = false;
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isRename = true;
                this._title.IsReadOnly = false;
                this._title.Focus();
                this._title.SelectAll();
            }
            catch (Exception ex)
            {
                LogOperate.Error("Rename_Click", ex);
            }
        }
        /// <summary>
        /// 文本输入，按下回车就赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (isRename)
                {
                    if (e.Key == Key.Enter)
                    {
                        isRename = false;
                        this._title.IsReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Title_PreviewKeyDown", ex);
            }
        }
        /// <summary> 
        /// 失去焦点后，也给名称赋值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Title_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isRename)
                {
                    isRename = false;
                    this._title.IsReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ModuleBase_LostFocus", ex);
            }
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Canvas)
            {
                (this.Parent as Canvas).Children.Remove(this);
            }
        }

        #endregion


        #region 模块移动处理


        public delegate void DelegatePointString(string pointString);
        public event DelegatePointString OnGetPointString;
        bool isMove = false;
        bool isChangePoint = false;
        bool isDoubleClick = false;
        bool isOnPath = false;//是否在连接点上方
        Point InitialPosition;

        /// <summary>
        /// 设置在画板上的位置
        /// </summary>
        /// <param name="point"></param>
        public void SetCanvasPoint(Point point)
        {
            try
            {
                ModuleItem.InCanvasPoint = point;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SetCanvasPoint", ex);
            }
        }

        /// <summary>
        /// 添加上层关系
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        public void AddUpperLevel(string id, PathLocation location, PathLocation endLocation)
        {
            try
            {
                PointRelationshipModel pointRelationship = new PointRelationshipModel();

                pointRelationship.StartPointID = id;
                pointRelationship.StartPointLocation = location;
                pointRelationship.EndPointID = ModuleItem.ID;
                pointRelationship.EndPointLocation = endLocation;

                if (ModuleItem.UpperLevelPoints.Exists(x => x.StartPointID == pointRelationship.StartPointID && x.EndPointID == pointRelationship.EndPointID))
                {
                    ModuleItem.UpperLevelPoints.RemoveAll(x => x.StartPointID == pointRelationship.StartPointID && x.EndPointID == pointRelationship.EndPointID);
                }
                ModuleItem.UpperLevelPoints.Add(pointRelationship);

                UpdateToProcess();
            }
            catch (Exception ex)
            {
                LogOperate.Error("AddUpperLevel", ex);
            }
        }

        /// <summary>
        /// 添加下层关系
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        public void AddLowerLevel(string id, PathLocation location, PathLocation endLocation)
        {
            try
            {
                PointRelationshipModel pointRelationship = new PointRelationshipModel();

                pointRelationship.StartPointID = ModuleItem.ID;
                pointRelationship.StartPointLocation = location;
                pointRelationship.EndPointID = id;
                pointRelationship.EndPointLocation = endLocation;

                if (ModuleItem.LowerLevelPoints.Exists(x => x.StartPointID == pointRelationship.StartPointID && x.EndPointID == pointRelationship.EndPointID))
                {
                    ModuleItem.LowerLevelPoints.RemoveAll(x => x.StartPointID == pointRelationship.StartPointID && x.EndPointID == pointRelationship.EndPointID);
                }
                ModuleItem.LowerLevelPoints.Add(pointRelationship);

                UpdateToProcess();
            }
            catch (Exception ex)
            {
                LogOperate.Error("AddLowerLevel", ex);
            }
        }


        /// <summary>
        /// 模块被鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleBase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isDoubleClick)
            {
                isDoubleClick = false;
                return;
            }
            if (isOnPath)
                return;
            isMove = true;
            isChangePoint = false;
            GlobalData.InstructionSign = true;
            this.CaptureMouse(); // 关键：锁定鼠标事件到当前控件
            // 记录初始位置
            InitialPosition = Mouse.GetPosition((UIElement)this.Parent);
        }

        /// <summary>
        /// 模块鼠标移动变化坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleBase_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if ((isMove && GlobalData.InstructionSign) && (UIElement)this.Parent is Canvas)
                {
                    if (VM_MainWindow.Instance.IsMouseOverControl(this))
                    {
                        return;
                    }
                    // 获取鼠标位置（相对于父容器）
                    Point mousePos = Mouse.GetPosition((UIElement)this.Parent);
                    OnGetPointString?.Invoke($"X:{mousePos.X},Y:{mousePos.Y}");
                    // 计算控件左上角新位置
                    double newX = mousePos.X;
                    double newY = mousePos.Y;
                    if ((isMove && GlobalData.InstructionSign))
                    {
                        // 更新位置（Canvas布局）
                        Canvas.SetLeft(this, newX);
                        Canvas.SetTop(this, newY);
                        isChangePoint = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ModuleBase_MouseMove", ex);
            }
        }
        /// <summary>
        /// 模块鼠标弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleBase_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (isMove && (UIElement)this.Parent is Canvas)
                {
                    if (isChangePoint)
                    {
                        // 获取鼠标位置（相对于父容器）
                        Point mousePos = Mouse.GetPosition((UIElement)this.Parent);

                        double newX = InitialPosition.X;
                        double newY = InitialPosition.Y;
                        if (mousePos.X > 0 && mousePos.Y > 0)
                        {
                            // 计算控件左上角新位置
                            newX = mousePos.X;
                            newY = mousePos.Y;
                        }
                        if (isMove && GlobalData.InstructionSign)
                        {
                            // 更新位置（Canvas布局）
                            Canvas.SetLeft(this, newX);
                            Canvas.SetTop(this, newY);
                            SetCanvasPoint(new Point(newX, newY));
                            UpdateToProcess();

                            VM_MainWindow.Instance.CreatePointRelationshipByProcess(this.Parent as Canvas, this.ModuleItem);
                        }
                    }
                }
                this.ReleaseMouseCapture(); // 释放捕获
                isMove = false;
                GlobalData.InstructionSign = false;
            }
            catch (Exception ex)
            {
                LogOperate.Error("ModuleBase_MouseUp", ex);
            }
            finally
            {
                this.ReleaseMouseCapture(); // 释放捕获
                isMove = false;
                GlobalData.InstructionSign = false;
            }
        }

        /// <summary>
        /// 更新数据到流程方案
        /// </summary>
        private void UpdateToProcess()
        {
            try
            {
                //更新方案中的数据
                var p_id = (this.Parent as Canvas).Name;
                var index = GlobalData.CurrentProject.ProcessList.FindIndex(x => x.ID == p_id);
                if (index >= 0)
                {
                    var m_list = GlobalData.CurrentProject.ProcessList[index].ModuleItems;
                    var m_index = m_list.FindIndex(x => x.ID == this.ModuleItem.ID);
                    if (m_index >= 0)
                    {
                        //更新模块数据
                        GlobalData.CurrentProject.ProcessList[index].ModuleItems[m_index] = this.ModuleItem;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 设置模块数据
        /// </summary>
        /// <param name="item"></param>
        public void SetModuleItem(ModuleItemModel item)
        {
            if (item.IsStartOrEnd)
            {
                Border_CornerRadius = new CornerRadius(20);
                StateBorder_CornerRadius = new CornerRadius(20, 0, 0, 20);
            }
            if (item.ID != ID)
            {
                item.ID = ID;
            }
            SetValue(ModuleItemProperty, item);
        }
        #endregion

        #region 双击打开配置界面

        /// <summary>
        /// 鼠标双击模块，打开配置界面   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDoubleClick = true;
                //显示配置界面
                //获取枚举特性，得到模块参数界面的类型，并显示
                // 获取枚举值 ImageSource 的 FieldInfo
                var fieldInfo = typeof(ModuleTypeEnum).GetField(ModuleItem.ModuleType.ToString(),
                    BindingFlags.Static | BindingFlags.Public);

                if (fieldInfo != null)
                {
                    // 获取 ModuleParamView 特性
                    var paramViewAttr = fieldInfo.GetCustomAttribute<ModuleParamViewAttribute>();

                    if (paramViewAttr != null)
                    {
                        object obj = paramViewAttr.ParamViewType.Assembly.CreateInstance(paramViewAttr.ParamViewType.FullName);
                        if (obj is IModuleParamView)
                        {
                            (obj as IModuleParamView).ShowView();
                        }
                    }
                    else
                    {
                        VM_MainWindow.Popup("模块参数界面未实现，敬请期待");
                    }
                }
                else
                {
                    VM_MainWindow.Popup("模块参数界面未实现，敬请期待");
                }
            }
        }

        #endregion


        /// <summary>
        /// 边框圆角
        /// </summary>
        public CornerRadius Border_CornerRadius { get; set; } = new CornerRadius(5);
        /// <summary>
        /// 状态灯边框圆角
        /// </summary>
        public CornerRadius StateBorder_CornerRadius { get; set; } = new CornerRadius(5, 0, 0, 5);


        #region 当前选择的模块
        /// <summary>
        /// 当前模块的属性
        /// </summary>
        public ModuleItemModel ModuleItem
        {
            get { return (ModuleItemModel)GetValue(ModuleItemProperty); }
            set { SetValue(ModuleItemProperty, value); OnPropertyChanged(); }
        }
        public static readonly DependencyProperty ModuleItemProperty =
DependencyProperty.RegisterAttached("ModuleItem", typeof(ModuleItemModel), typeof(ModuleBase), new PropertyMetadata(null, ModuleItemValueChanged));

        private static void ModuleItemValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(ModuleItemProperty, (ModuleItemModel)e.NewValue);
        }

        /// <summary>
        /// 状态灯
        /// </summary>
        public Brush State_Brush
        {
            get { return (Brush)GetValue(State_BrushProperty); }
            set { SetValue(State_BrushProperty, value); OnPropertyChanged(); }
        }

        public static readonly DependencyProperty State_BrushProperty =
DependencyProperty.RegisterAttached("State_Brush", typeof(Brush), typeof(ModuleBase), new PropertyMetadata(Brushes.White, State_BrushValueChanged));

        private static void State_BrushValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.SetValue(State_BrushProperty, (Brush)e.NewValue);
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
