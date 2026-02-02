using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LS.Flowchart.Models.ToolModels;
using LS.Flowchart.ModuleControls;
using LS.Flowchart.Tools;
using LS.Flowchart.UCControls;

namespace LS.Flowchart.ViewModels
{
    public partial class VM_MainWindow
    {
        #region 工具箱操作方法

        /// <summary>
        /// 初始化工具箱
        /// 添加模块到工具箱里边
        /// </summary>
        private void InitTools()
        {
            try
            {
                ObservableCollection<ToolItemModel> tools = new ObservableCollection<ToolItemModel>();

                ToolItemModel images = new ToolItemModel();
                images.Name = "采集";
                images.Icon = UIElement.FindResource("相机") as Geometry;
                images.IsExpanded = false;
                images.Sort = 0;
                images.ModuleItems = new ObservableCollection<ModuleItemModel>()
                {
                    new ModuleItemModel(){ Name="图像源",ModuleType=ModuleTypeEnum.ImageSource,Sort=0,IsStartOrEnd=true, Icon=UIElement.FindResource("图像源") as Geometry  },
                    new ModuleItemModel(){ Name="多图采集",ModuleType=ModuleTypeEnum.MultiImage,Sort=1,IsStartOrEnd=true,Icon=UIElement.FindResource("多图采集") as Geometry  },
                    new ModuleItemModel(){ Name="输出图像",ModuleType=ModuleTypeEnum.ImageOutput,Sort=2,Icon=UIElement.FindResource("图像输出") as Geometry  },
                    new ModuleItemModel(){ Name="光源",ModuleType=ModuleTypeEnum.LightSource,Sort=3,Icon=UIElement.FindResource("光源") as Geometry  },
                };
                images.ModuleItems.ToList().Sort((x, y) => x.Sort.CompareTo(y.Sort));
                tools.Add(images);

                ToolItemModel recognize = new ToolItemModel();
                recognize.Name = "识别";
                recognize.Icon = UIElement.FindResource("扫码识别") as Geometry;
                recognize.IsExpanded = false;
                recognize.Sort = 1;
                recognize.ModuleItems = new ObservableCollection<ModuleItemModel>()
                {
                    new ModuleItemModel(){ Name="二维码识别",ModuleType=ModuleTypeEnum.QR_Recognize,Sort=0,Icon=UIElement.FindResource("二维码识别") as Geometry  },
                    new ModuleItemModel(){ Name="条码识别",ModuleType=ModuleTypeEnum.BarCode_Recognize,Sort=1,Icon=UIElement.FindResource("一维码识别") as Geometry  },
                    new ModuleItemModel(){ Name="字符识别",ModuleType=ModuleTypeEnum.Char_Recognize,Sort=2,Icon=UIElement.FindResource("字符识别") as Geometry  },
                };
                recognize.ModuleItems.ToList().Sort((x, y) => x.Sort.CompareTo(y.Sort));
                tools.Add(recognize);


                tools.ToList().Sort((x, y) => x.Sort.CompareTo(y.Sort));
                ToolItems = tools;
            }
            catch (Exception ex)
            {
                LogOperate.Error("初始化工具箱", ex);
            }
        }

        /// <summary>
        /// 初始化流程
        /// 空白项目默认添加一个流程
        /// </summary>
        public void InitProcess()
        {
            try
            {
                //根据方案创建界面
                CreateByProject();
            }
            catch (Exception ex)
            {
                LogOperate.Error("InitProcess", ex);
            }
        }

        public DelegateCommand ToolOpCommand
        {
            get { return new DelegateCommand(ToolOp); }
        }

        private void ToolOp(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    switch (obj.ToString())
                    {
                        case "ToolExpandClick":
                            Tool_IsExpanded = !Tool_IsExpanded;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ToolOp(Object obj)", ex);
            }
        }

        #region 控件拖动处理方法

        private ModuleBase Module;
        /// <summary>
        /// 在鼠标位置添加模块控件
        /// </summary>
        /// <param name="moduleItem"></param>
        private void DrawModule(ModuleItemModel moduleItem)
        {
            try
            {
                if (Module != null)
                {
                    return;
                }
                int index = 1;
                foreach (var item in (TabSelectItem.Content as Canvas).Children)
                {
                    if (item != null && item is ModuleBase)
                    {
                        if ((item as ModuleBase).ModuleItem.ModuleType == moduleItem.ModuleType)
                        {
                            index++;
                        }
                    }
                }
                string Name = $"{moduleItem.Name}{index}";
                moduleItem.ProcessID = (TabSelectItem.Content as Canvas).Name;
                ModuleItemModel module = ModuleItemModel.Copy(moduleItem);
                module.Name = Name;
                Module = new ModuleBase(module.ID);
                Module.SetModuleItem(module);
                Module.Opacity = 0.6;
                Module.OnGetPointString += (p) => { PointString = p; };
                Module.OnEnterConnectPointMode += Module_OnEnterConnectPointMode;


                // 获取鼠标位置（相对于父容器）
                Point mousePos = Mouse.GetPosition(TabSelectItem.Content as Canvas);

                // 设置初始位置（需Canvas布局）
                Canvas.SetLeft(Module, mousePos.X);
                Canvas.SetTop(Module, mousePos.Y);
                Module.SetCanvasPoint(mousePos);
                // 添加到父容器（如Canvas）
                (TabSelectItem.Content as Canvas).Children.Add(Module);
            }
            catch (Exception ex)
            {
                LogOperate.Error("DrawModule", ex);
            }
        }

        /// <summary>
        /// 鼠标弹起时，将控件添加到画板中
        /// </summary>
        private void AddModule()
        {
            try
            {
                if (Module != null)
                {
                    //判断控件当前是否在画板的范围内
                    var canvas = TabSelectItem.Content as Canvas;
                    Point mousePos = Mouse.GetPosition(TabSelectItem.Content as Canvas);
                    if (mousePos.X > 0 && mousePos.Y > 0)
                    {
                        Module.Opacity = 1;
                        Module.SetCanvasPoint(mousePos);

                        var index = GlobalData.CurrentProject.ProcessList.FindIndex(x => x.ID == Module.ModuleItem.ProcessID);
                        if (index >= 0)
                        {
                            GlobalData.CurrentProject.ProcessList[index].ModuleItems.Add(Module.ModuleItem);
                        }

                        Module = null;
                    }
                    else
                    {
                        canvas.Children.Remove(Module);
                        Module = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("AddModule", ex);
            }
        }

        /// <summary>
        /// 移动模块
        /// </summary>
        private void MoveModule()
        {
            try
            {
                if (Module != null)
                {
                    //更新模块的位置

                    // 获取鼠标位置（相对于父容器）
                    Point mousePos = Mouse.GetPosition(TabSelectItem.Content as Canvas);

                    // 计算控件左上角新位置
                    double newX = mousePos.X;
                    double newY = mousePos.Y;

                    // 更新位置（Canvas布局）
                    Canvas.SetLeft(Module, newX);
                    Canvas.SetTop(Module, newY);
                    Module.SetCanvasPoint(mousePos);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("AddModule", ex);
            }
        }

        #region 连线模式

        /// <summary>
        /// 是否进入了连线模式
        /// </summary>
        private bool IsConnectPointMode = false;
        /// <summary>
        /// 当前最新的连接关系
        /// </summary>
        private PointRelationshipModel PointRelationship = null;
        /// <summary>
        /// 连线的开始控件
        /// </summary>
        private ModuleBase Point_StartModule = null;
        /// <summary>
        /// 当前绘制的箭头线
        /// </summary>
        private ArrowPolyline _currentArrow = null;

        /// <summary>
        /// 进入连线模式
        /// </summary>
        /// <param name="location"></param>
        /// <param name="moduleItem"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Module_OnEnterConnectPointMode(ModuleBase sender, PathLocation location, ModuleItemModel moduleItem)
        {
            try
            {
                if (IsConnectPointMode)
                    return;
                IsConnectPointMode = true;
                PointRelationship = new PointRelationshipModel();

                PointRelationship.StartPointLocation = location;
                PointRelationship.StartPointID = sender.ID;
                Point_StartModule = sender;
                // 获取鼠标位置（相对于父容器）
                Point mousePos = Mouse.GetPosition(TabSelectItem.Content as Canvas);
                _currentArrow = new ArrowPolyline();
                _currentArrow.StartID = sender.ID;
                _currentArrow.StartPoint = mousePos;
                _currentArrow.EndPoint = mousePos;
                (TabSelectItem.Content as Canvas).Children.Add(_currentArrow);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 绘制移动时的连接线
        /// </summary>
        private void DrawConnectPoint()
        {
            try
            {
                if (IsConnectPointMode)
                {
                    if (_currentArrow != null)
                    {
                        // 获取鼠标位置（相对于父容器）
                        Point mousePos = Mouse.GetPosition(TabSelectItem.Content as Canvas);
                        _currentArrow.EndPoint = mousePos;

                        // 当XY方向均有偏移时计算折点（HVH模式）
                        if (ShouldAddBreakPoints(_currentArrow.StartPoint, mousePos))
                        {
                            _currentArrow.BreakPoints = CalculateBreakPointsVHV(_currentArrow.StartPoint, mousePos);
                        }
                        else
                        {
                            _currentArrow.BreakPoints.Clear(); // 直线模式
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("DrawConnectPoint", ex);
            }
        }

        /// <summary>
        /// 确认连接线
        /// </summary>
        private void ComfirmConnectPoint()
        {
            try
            {
                if (IsConnectPointMode)
                {
                    (TabSelectItem.Content as Canvas).Children.Remove(_currentArrow);

                    //判断是否在控件上，非起始控件
                    Point mousePos = Mouse.GetPosition(TabSelectItem.Content as Canvas);
                    //获取鼠标下的控件
                    var endModule = VisualTreeHelperExtensions.GetModuleBaseUnderPoint(TabSelectItem.Content as Canvas, mousePos);
                    if (endModule != null)
                    {
                        if (endModule.ID != PointRelationship.StartPointID)
                        {
                            PointRelationship.EndPointID = endModule.ID;
                            //定位鼠标在控件的哪个区域，分上下左右
                            // 在 endModule 判断区域
                            Point mouseRelativePos = Mouse.GetPosition(endModule);
                            double width = endModule.ActualWidth;
                            double height = endModule.ActualHeight;
                            var location = PathLocation.Top;
                            // 判断水平方向（左/右）
                            if (mouseRelativePos.X < width * 0.3)
                                location = PathLocation.Left;
                            else if (mouseRelativePos.X > width * 0.7)
                                location = PathLocation.Right;
                            else
                            {
                                if (mouseRelativePos.Y <= height * 0.5)
                                {
                                    location = PathLocation.Top;
                                }
                                else
                                {
                                    location = PathLocation.Bottom;
                                }
                            }
                            PointRelationship.EndPointLocation = location;

                            if (Point_StartModule != null)
                            {
                                Point_StartModule.AddLowerLevel(PointRelationship.EndPointID, PointRelationship.StartPointLocation, PointRelationship.EndPointLocation);
                            }

                            endModule.AddUpperLevel(PointRelationship.StartPointID, PointRelationship.StartPointLocation, PointRelationship.EndPointLocation);
                            //绘画连接线
                            CreatePointRelationshipByProcess(TabSelectItem.Content as Canvas, Point_StartModule.ModuleItem);
                        }
                    }
                    _currentArrow = null;
                    Point_StartModule = null;
                    PointRelationship = null;
                    IsConnectPointMode = false;
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("ComfirmConnectPoint", ex);
            }
        }


        #region 折点计算核心逻辑
        // 判断是否需要添加折点（XY方向均偏移超过阈值）
        private bool ShouldAddBreakPoints(Point start, Point end) =>
            Math.Abs(end.X - start.X) > 10 && Math.Abs(end.Y - start.Y) > 10;

        /// <summary>
        /// HVH折线模式：水平→垂直→水平
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private List<Point> CalculateBreakPointsHVH(Point start, Point end)
        {
            var points = new List<Point>();
            double midX = (start.X + end.X) / 2;

            // 第一段水平线终点
            points.Add(new Point(midX, start.Y));
            // 第二段垂直线终点
            points.Add(new Point(midX, end.Y));

            return points;
        }

        /// <summary>
        /// VHV模式：垂直 → 水平 → 垂直
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private List<Point> CalculateBreakPointsVHV(Point start, Point end)
        {
            var points = new List<Point>();
            double midY = (start.Y + end.Y) / 2; // Y轴中点

            // 第一段垂直线终点：从起点垂直移动到midY
            points.Add(new Point(start.X, midY));
            // 第二段水平线终点：从垂直点水平移动到终点X位置
            points.Add(new Point(end.X, midY));

            return points;
        }



        #endregion

        #endregion

        #endregion

        #region 工具箱属性

        private ObservableCollection<ToolItemModel> _toolItems = new ObservableCollection<ToolItemModel>();
        /// <summary>
        /// 工具箱数据集合
        /// </summary>
        public ObservableCollection<ToolItemModel> ToolItems
        {
            get { return _toolItems; }
            set { _toolItems = value; OnPropertyChanged(); }
        }

        private bool _Tool_IsExpanded = true;
        /// <summary>
        /// 工具箱的是否折叠隐藏
        /// </summary>
        public bool Tool_IsExpanded
        {
            get { return _Tool_IsExpanded; }
            set { _Tool_IsExpanded = value; OnPropertyChanged(); }
        }

        private ModuleItemModel _ModuleItem;
        /// <summary>
        /// 当前选择的模块
        /// </summary>
        public ModuleItemModel ModuleItem
        {
            get { return _ModuleItem; }
            set
            {
                _ModuleItem = value;
                OnPropertyChanged();

                if (_ModuleItem != null)
                {
                    DrawModule(_ModuleItem);
                }
                else
                {
                    AddModule();
                }

            }
        }

        private string _PointString;
        /// <summary>
        /// 移动点位信息
        /// </summary>
        public string PointString
        {
            get { return _PointString; }
            set { _PointString = value; OnPropertyChanged(); }
        }



        #endregion

        #region 流程管理


        /// <summary>
        /// 根据方案数据创建界面
        /// </summary>
        /// <param name="process"></param>
        private void CreateByProject()
        {
            try
            {
                if (GlobalData.CurrentProject == null || GlobalData.CurrentProject.ProcessList == null)
                {
                    return;
                }
                var tab = (UIElement as MainWindow).canvasTab;
                tab.Items.Clear();

                foreach (var item in GlobalData.CurrentProject.ProcessList)
                {
                    Canvas canvas = new Canvas();
                    canvas.Name = item.ID;
                    canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
                    canvas.VerticalAlignment = VerticalAlignment.Stretch;
                    canvas.Background = UIElement.FindResource("DefaultCanvasBackground") as SolidColorBrush;
                    canvas.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
                    canvas.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);

                    foreach (var module in item.ModuleItems)
                    {
                        //添加模块组件
                        CreateModuleBaseByProcess(canvas, module);
                    }
                    foreach (var module in item.ModuleItems)
                    {
                        //添加模块连线
                        CreatePointRelationshipByProcess(canvas, module);
                    }


                    TabItem tabItem = new TabItem();
                    tabItem.Header = item.Name;
                    tabItem.Content = canvas;
                    tab.Items.Add(tabItem);
                }
                tab.SelectedIndex=0;


            }
            catch (Exception ex)
            {
                LogOperate.Error("CreateByProject", ex);
            }
        }

        /// <summary>
        /// 创建 模块控件
        /// </summary>
        /// <param name="moduleItem"></param>
        public void CreateModuleBaseByProcess(Canvas canvas, ModuleItemModel moduleItem)
        {
            try
            {
                ModuleBase Module = new ModuleBase();
                Module.ID = moduleItem.ID;
                Module.SetModuleItem(moduleItem);
                Module.OnGetPointString += (p) => { PointString = p; };
                Module.OnEnterConnectPointMode += Module_OnEnterConnectPointMode;

                canvas.Children.Add(Module);
                Canvas.SetLeft(Module, moduleItem.InCanvasPoint.X);
                Canvas.SetTop(Module, moduleItem.InCanvasPoint.Y);
            }
            catch (Exception ex)
            {
                LogOperate.Error("CreateModuleBaseByProcess", ex);
                VM_MainWindow.Popup("加载方案发生错误", "错误");
            }
        }

        private ArrowPolyline _selectLine;
        /// <summary>
        /// 当前选择的线
        /// </summary>
        public ArrowPolyline SelectLine
        {
            get { return _selectLine; }
            set 
            {
                if (_selectLine != null)
                {
                    _selectLine.LoseSelect();
                }
                _selectLine = value;  
            }
        }
        /// <summary>
        /// 创建 连接线
        /// </summary>
        /// <param name="moduleItem"></param>
        public void CreatePointRelationshipByProcess(Canvas canvas, ModuleItemModel moduleItem)
        {
            try
            {
                //绘画连接线，取上下层关系进行绘画
                var modules = canvas.FindVisualChildren<ModuleBase>().ToList();
                var points = canvas.FindVisualChildren<ArrowPolyline>().ToList();


                foreach (var point in moduleItem.LowerLevelPoints)
                {

                    var start = modules.Find(x => x.ID == point.StartPointID);
                    var end = modules.Find(x => x.ID == point.EndPointID);
                    //获取初始点和结束点
                    Point startPos = start.GetPathLocation(point.StartPointLocation);
                    Point endPos = end.GetPathLocation(point.EndPointLocation);
                    var Arrow = new ArrowPolyline();
                    //初始点ID+结束点ID 生成线的ID
                    string id = $"{point.StartPointID}{point.EndPointID}";
                    if (points.Exists(x => x.ID == id))
                    {
                        canvas.Children.Remove(points.Find(x => x.ID == id));
                    }
                    Arrow.ID = id;
                    Arrow.StartID = start.ID;
                    Arrow.EndID = end.ID;
                    Arrow.StartPoint = startPos;
                    Arrow.EndPoint = endPos;
                    Arrow.ArrowDirection = GetArrowDirection(point.EndPointLocation);
                    Arrow.OnSelectChanged += ((line) => { SelectLine = line; });
                    // 当XY方向均有偏移时计算折点（VHV模式）
                    if (ShouldAddBreakPoints(startPos, endPos))
                    {
                        Arrow.BreakPoints = CalculateBreakPointsVHV(startPos, endPos);
                    }
                    else
                    {
                        Arrow.BreakPoints.Clear(); // 直线模式
                    }

                    canvas.Children.Add(Arrow);
                }

                foreach (var point in moduleItem.UpperLevelPoints)
                {
                    var start = modules.Find(x => x.ID == point.StartPointID);
                    var end = modules.Find(x => x.ID == point.EndPointID);
                    //获取初始点和结束点
                    Point startPos = start.GetPathLocation(point.StartPointLocation);
                    Point endPos = end.GetPathLocation(point.EndPointLocation);
                    var Arrow = new ArrowPolyline();
                    //初始点ID+结束点ID 生成线的ID
                    string id = $"{point.StartPointID}{point.EndPointID}";
                    if (points.Exists(x => x.ID == id))
                    {
                        canvas.Children.Remove(points.Find(x => x.ID == id));
                    }
                    Arrow.ID = id;
                    Arrow.StartID = start.ID;
                    Arrow.EndID = end.ID;
                    Arrow.StartPoint = startPos;
                    Arrow.EndPoint = endPos;
                    Arrow.ArrowDirection = GetArrowDirection(point.EndPointLocation);
                    Arrow.OnSelectChanged += ((line) => { SelectLine = line; });
                    // 当XY方向均有偏移时计算折点（VHV模式）
                    if (ShouldAddBreakPoints(startPos, endPos))
                    {
                        Arrow.BreakPoints = CalculateBreakPointsVHV(startPos, endPos);
                    }
                    else
                    {
                        Arrow.BreakPoints.Clear(); // 直线模式
                    }

                    canvas.Children.Add(Arrow);
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("CreatePointRelationshipByProcess", ex);
                VM_MainWindow.Popup("加载方案发生错误（模块关系部分）", "错误");
            }
        }

        /// <summary>
        /// 获取箭头方向
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private Direction GetArrowDirection(PathLocation location)
        {
            switch (location)
            {
                case PathLocation.Left:
                    return Direction.Right;
                case PathLocation.Right:
                    return Direction.Left;
                case PathLocation.Top:
                    return Direction.Down;
                case PathLocation.Bottom:
                    return Direction.Up;
                default:
                    return Direction.None;
            }
        }


        #region 流程属性

        private int _TabIndex = 0;
        /// <summary>
        /// 画板Tab的选择索引
        /// </summary>
        public int TabIndex
        {
            get { return _TabIndex; }
            set { _TabIndex = value; OnPropertyChanged(); }
        }

        private TabItem _TabSelectItem;
        /// <summary>
        /// 当前选择的Tab子项
        /// </summary>
        public TabItem TabSelectItem
        {
            get { return _TabSelectItem; }
            set { _TabSelectItem = value; OnPropertyChanged(); }
        }

        #endregion

        #endregion

        #endregion
    }
}
