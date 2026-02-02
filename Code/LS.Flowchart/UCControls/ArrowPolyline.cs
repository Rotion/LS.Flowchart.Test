using LS.WPF.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LS.Flowchart.UCControls
{
    public class ArrowPolyline : Shape
    {
        public ArrowPolyline()
        {
            this.MouseLeftButtonDown += ArrowPolyline_MouseLeftButtonDown;
            this.Stroke = Default_Stroke;
            this.StrokeThickness = 3;
            this.ArrowSize = 11;
        }

        private void ArrowPolyline_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnSelectChanged?.Invoke(this);
            this.Stroke = Select_Stroke;
        }

        public void LoseSelect()
        {
            this.Stroke = Default_Stroke;
        }

        public delegate void OnSelectChangedHandler(ArrowPolyline sender);
        public event OnSelectChangedHandler OnSelectChanged;

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID { get; set; }        
        /// <summary>
        /// 起始点的模块ID
        /// </summary>
        public  string StartID { get; set; }
        /// <summary>
        /// 结束点的模块ID
        /// </summary>
        public string EndID { get; set; }
        public Brush Default_Stroke { get; set; } = Brushes.Orange;
        public Brush Select_Stroke { get; set; } = Brushes.Green;

        // 依赖属性定义（新增箭头方向属性）
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowSizeProperty =
            DependencyProperty.Register("ArrowSize", typeof(double), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowDirectionProperty =
            DependencyProperty.Register("ArrowDirection", typeof(Direction), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(Direction.None, FrameworkPropertyMetadataOptions.AffectsRender));

        // 折点集合
        private List<Point> _breakPoints = new List<Point>();
        public List<Point> BreakPoints
        {
            get => _breakPoints;
            set
            {
                _breakPoints = value ?? new List<Point>();
                UpdateBreakPoints();
            }
        }

        // 属性封装
        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        public Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public double ArrowSize
        {
            get => (double)GetValue(ArrowSizeProperty);
            set => SetValue(ArrowSizeProperty, value);
        }

        public Direction ArrowDirection
        {
            get => (Direction)GetValue(ArrowDirectionProperty);
            set => SetValue(ArrowDirectionProperty, value);
        }

        /// <summary>
        /// 核心：动态生成几何路径
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                PathGeometry geometry = new PathGeometry();
                try
                {
                    PathFigure lineFigure = new PathFigure
                    {
                        StartPoint = StartPoint,
                        IsClosed = false
                    };

                    // 添加所有折点
                    foreach (Point point in BreakPoints)
                    {
                        lineFigure.Segments.Add(new LineSegment(point, true));
                    }
                    // 添加终点
                    var endPoint = EndPoint;
                    switch (ArrowDirection)
                    {
                        case Direction.None:
                            lineFigure.Segments.Add(new LineSegment(endPoint, true));
                            break;
                        case Direction.Up:
                            endPoint.Y += ArrowSize;
                            lineFigure.Segments.Add(new LineSegment(endPoint, true));
                            break;
                        case Direction.Down:
                            endPoint.Y -= ArrowSize;
                            lineFigure.Segments.Add(new LineSegment(endPoint, true));
                            break;
                        case Direction.Left:
                            endPoint.X += ArrowSize;
                            lineFigure.Segments.Add(new LineSegment(endPoint, true));
                            break;
                        case Direction.Right:
                            endPoint.X -= ArrowSize;
                            lineFigure.Segments.Add(new LineSegment(endPoint, true));
                            break;
                    }
                   
                    geometry.Figures.Add(lineFigure);


                    // 2. 添加智能箭头
                    if (ArrowDirection != Direction.None && StartPoint != EndPoint)
                    {
                        CreateSmartArrowHead(geometry);
                    }
                }
                catch (Exception ex)
                {
                    // 日志处理
                    LogOperate.Error("DefiningGeometry", ex);
                }

                geometry.Freeze();
                return geometry;
            }
        }

        /// <summary>
        /// 智能箭头生成（支持四方向）
        /// </summary>
        private void CreateSmartArrowHead(PathGeometry geometry)
        {
            Point arrowTip = EndPoint;
            Vector direction = CalculateArrowDirection();

            PathFigure arrowFigure = new PathFigure
            {
                StartPoint = arrowTip,
                IsClosed = true
            };

            // 计算箭头三顶点
            Point CalculatePoint(double angle, double distance)
            {
                double rad = angle * Math.PI / 180;
                return new Point(
                    arrowTip.X + distance * (direction.X * Math.Cos(rad) - direction.Y * Math.Sin(rad)),
                    arrowTip.Y + distance * (direction.X * Math.Sin(rad) + direction.Y * Math.Cos(rad))
                );
            }

            // 根据方向生成箭头
            arrowFigure.Segments.Add(new LineSegment(CalculatePoint(210, ArrowSize), true)); // 左侧翼点
            arrowFigure.Segments.Add(new LineSegment(CalculatePoint(150, ArrowSize), true)); // 右侧翼点
            arrowFigure.Segments.Add(new LineSegment(arrowTip, true)); // 闭合

            geometry.Figures.Add(arrowFigure);
        }

        /// <summary>
        /// 根据方向计算箭头向量
        /// </summary>
        private Vector CalculateArrowDirection()
        {
            switch (ArrowDirection)
            {
                case Direction.Up:
                    return new Vector(0, -1);    // 上
                case Direction.Down:
                    return new Vector(0, 1);   // 下
                case Direction.Left:
                    return new Vector(-1, 0);  // 左
                case Direction.Right:
                    return new Vector(1, 0);  // 右
                default:
                    return (EndPoint - StartPoint);// 默认使用线段方向
            }
        }

        /// <summary>
        /// 自动避让转折点生成
        /// </summary>
        private void UpdateBreakPoints()
        {
            if (_breakPoints.Count == 0) return;

            // 1. 起点到第一个转折点
            Vector startVector = _breakPoints[0] - StartPoint;

            // 2. 末端转折优化
            Point lastBreak = _breakPoints[_breakPoints.Count - 1];
            Vector endVector = EndPoint - lastBreak;

            //// 水平方向连接时添加垂直避让
            //if (Math.Abs(endVector.X) > Math.Abs(endVector.Y))
            //{
            //    double offsetY = ArrowSize * (endVector.Y > 0 ? 1 : -1);
            //    _breakPoints.Add(new Point(EndPoint.X, lastBreak.Y + offsetY));
            //}
            //// 垂直方向连接时添加水平避让
            //else
            //{
            //    double offsetX = ArrowSize * (endVector.X > 0 ? 1 : -1);
            //    _breakPoints.Add(new Point(lastBreak.X + offsetX, EndPoint.Y));
            //}
        }
    }

    /// <summary>
    /// 箭头方向枚举
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 不显示箭头
        /// </summary>
        None,
        Up,
        Down,
        Left,
        Right,
        Auto
    }

    /*
     * 版本 20250729 16:00:00
    public class ArrowPolyline : Shape
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID { get; set; }

        // 依赖属性定义（新增箭头方向属性）
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowSizeProperty =
            DependencyProperty.Register("ArrowSize", typeof(double), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowDirectionProperty =
            DependencyProperty.Register("ArrowDirection", typeof(Direction), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(Direction.None, FrameworkPropertyMetadataOptions.AffectsRender));

        // 折点集合
        private List<Point> _breakPoints = new List<Point>();
        public List<Point> BreakPoints
        {
            get => _breakPoints;
            set
            {
                _breakPoints = value ?? new List<Point>();
                UpdateBreakPoints();
            }
        }

        // 属性封装
        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        public Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public double ArrowSize
        {
            get => (double)GetValue(ArrowSizeProperty);
            set => SetValue(ArrowSizeProperty, value);
        }

        public Direction ArrowDirection
        {
            get => (Direction)GetValue(ArrowDirectionProperty);
            set => SetValue(ArrowDirectionProperty, value);
        }

        /// <summary>
        /// 核心：动态生成几何路径
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                PathGeometry geometry = new PathGeometry();
                try
                {
                    // 1. 绘制主折线
                    PathFigure lineFigure = new PathFigure
                    {
                        StartPoint = StartPoint,
                        IsClosed = false
                    };

                    foreach (Point point in _breakPoints)
                    {
                        lineFigure.Segments.Add(new LineSegment(point, true));
                    }
                    lineFigure.Segments.Add(new LineSegment(EndPoint, true));
                    geometry.Figures.Add(lineFigure);

                    // 2. 添加智能箭头
                    if (ArrowDirection!=Direction.None && StartPoint != EndPoint)
                    {
                        CreateSmartArrowHead(geometry);
                    }
                }
                catch (Exception ex)
                {
                    // 日志处理
                    LogOperate.Error("DefiningGeometry", ex);
                }

                geometry.Freeze();
                return geometry;
            }
        }

        /// <summary>
        /// 智能箭头生成（支持四方向）
        /// </summary>
        private void CreateSmartArrowHead(PathGeometry geometry)
        {
            Point arrowTip = EndPoint;
            Vector direction = CalculateArrowDirection();

            PathFigure arrowFigure = new PathFigure
            {
                StartPoint = arrowTip,
                IsClosed = true
            };

            // 计算箭头三顶点
            Point CalculatePoint(double angle, double distance)
            {
                double rad = angle * Math.PI / 180;
                return new Point(
                    arrowTip.X + distance * (direction.X * Math.Cos(rad) - direction.Y * Math.Sin(rad)),
                    arrowTip.Y + distance * (direction.X * Math.Sin(rad) + direction.Y * Math.Cos(rad))
                );
            }

            // 根据方向生成箭头
            arrowFigure.Segments.Add(new LineSegment(CalculatePoint(210, ArrowSize), true)); // 左侧翼点
            arrowFigure.Segments.Add(new LineSegment(CalculatePoint(150, ArrowSize), true)); // 右侧翼点
            arrowFigure.Segments.Add(new LineSegment(arrowTip, true)); // 闭合

            geometry.Figures.Add(arrowFigure);
        }

        /// <summary>
        /// 根据方向计算箭头向量
        /// </summary>
        private Vector CalculateArrowDirection()
        {
            switch (ArrowDirection)
            {
                case Direction.Up:
                    return new Vector(0, -1);    // 上
                case Direction.Down:
                    return new Vector(0, 1);   // 下
                case Direction.Left:
                    return new Vector(-1, 0);  // 左
                case Direction.Right:
                    return new Vector(1, 0);  // 右
                default:
                    return (EndPoint - StartPoint);// 默认使用线段方向
            }
        }

        /// <summary>
        /// 自动避让转折点生成
        /// </summary>
        private void UpdateBreakPoints()
        {
            if (_breakPoints.Count == 0) return;

            // 1. 起点到第一个转折点
            Vector startVector = _breakPoints[0] - StartPoint;

            // 2. 末端转折优化
            Point lastBreak = _breakPoints[_breakPoints.Count - 1];
            Vector endVector = EndPoint - lastBreak;

            // 水平方向连接时添加垂直避让
            if (Math.Abs(endVector.X) > Math.Abs(endVector.Y))
            {
                double offsetY = ArrowSize * (endVector.Y > 0 ? 1 : -1);
                _breakPoints.Add(new Point(EndPoint.X, lastBreak.Y + offsetY));
            }
            // 垂直方向连接时添加水平避让
            else
            {
                double offsetX = ArrowSize * (endVector.X > 0 ? 1 : -1);
                _breakPoints.Add(new Point(lastBreak.X + offsetX, EndPoint.Y));
            }
        }
    }

    /// <summary>
    /// 箭头方向枚举
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 不显示箭头
        /// </summary>
        None,
        Up,
        Down,
        Left,
        Right,
        Auto
    }

    */

    /*
     *  版本  2025-07-27 14:00:00
    public class ArrowPolyline : Shape
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string ID {  get; set; }

        // 依赖属性定义
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(default(Point), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowSizeProperty =
            DependencyProperty.Register("ArrowSize", typeof(double), typeof(ArrowPolyline),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

        // 折点集合（非依赖属性，通过INotifyPropertyChanged通知更新）
        private List<Point> _breakPoints = new List<Point>();
        public List<Point> BreakPoints
        {
            get => _breakPoints;
            set
            {
                _breakPoints = value ?? new List<Point>();
                InvalidateVisual(); // 重绘
            }
        }

        // 属性封装
        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }

        public Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public double ArrowSize
        {
            get => (double)GetValue(ArrowSizeProperty);
            set => SetValue(ArrowSizeProperty, value);
        }

        /// <summary>
        /// 核心：动态生成几何路径
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                
                PathGeometry geometry = new PathGeometry();
                try
                {
                    PathFigure lineFigure = new PathFigure
                    {
                        StartPoint = StartPoint,
                        IsClosed = false
                    };

                    // 添加所有折点
                    foreach (Point point in BreakPoints)
                    {
                        lineFigure.Segments.Add(new LineSegment(point, true));
                    }
                    // 添加终点
                    lineFigure.Segments.Add(new LineSegment(EndPoint, true));
                    geometry.Figures.Add(lineFigure);

                    // 添加箭头（仅当线段长度>0时）
                    if (StartPoint != EndPoint)
                    {
                        Point arrowBase = BreakPoints.Count > 0 ? BreakPoints[1] : StartPoint;
                        Vector direction = (EndPoint - arrowBase);
                        if (direction.Length > 0) // 避免零向量
                        {
                            direction.Normalize();
                            CreateArrowHead(geometry, direction);
                        }
                    }
                }
                catch(Exception ex)
                {
                    LogOperate.Error("DefiningGeometry", ex);
                }

                geometry.Freeze(); // 冻结几何对象提升性能
                return geometry;
            }
        }

        private void CreateArrowHead(PathGeometry geometry, Vector direction)
        {
            const double arrowAngle = 150; // 箭头张角150°

            // 确保方向向量是单位向量（长度为1）
            direction.Normalize();

            // 反转方向向量（关键修正）
            Vector reverseDir = new Vector(-direction.X, -direction.Y);

            // 计算旋转后的箭头两翼点（使用三角函数）
            Point CalculateWingPoint(double angle)
            {
                double radians = angle * Math.PI / 180;
                return new Point(
                    EndPoint.X + ArrowSize * (reverseDir.X * Math.Cos(radians) - reverseDir.Y * Math.Sin(radians)),
                    EndPoint.Y + ArrowSize * (reverseDir.X * Math.Sin(radians) + reverseDir.Y * Math.Cos(radians))
                );
            }

            // 计算左右翼点（角度符号交换）
            Point arrowLeft = CalculateWingPoint(-arrowAngle / 2);  // 左翼（-75°）
            Point arrowRight = CalculateWingPoint(arrowAngle / 2);   // 右翼（75°）

            // 构建箭头三角形路径
            PathFigure arrowFigure = new PathFigure
            {
                StartPoint = EndPoint,
                IsClosed = true
            };
            arrowFigure.Segments.Add(new LineSegment(arrowLeft, true));
            arrowFigure.Segments.Add(new LineSegment(arrowRight, true));
            arrowFigure.Segments.Add(new LineSegment(EndPoint, true)); // 闭合路径

            geometry.Figures.Add(arrowFigure);
        }
    }

    */
}

