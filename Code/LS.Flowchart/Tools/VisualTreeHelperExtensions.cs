using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LS.Flowchart.ModuleControls;
using LS.Flowchart.UCControls;

namespace LS.Flowchart.Tools
{
    public static class VisualTreeHelperExtensions
    {
        public static T GetVisualAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            if (parent == null)
            {
                return null;
            }
            if (parent is T)
            {
                return parent as T;
            }
            return parent.GetVisualAncestor<T>();
        }

        /// <summary>
        /// 判断当前鼠标位置下是否有ModuleBase控件
        /// </summary>
        /// <param name="canvas">容器控件</param>
        /// <param name="point">鼠标的坐标</param>
        /// <returns></returns>
        public static ModuleBase GetModuleBaseUnderPoint(Canvas canvas, Point point)
        {
            UIElement element = null;
            VisualTreeHelper.HitTest(
                canvas,
                null,
                new HitTestResultCallback((hitResult) =>
                {
                    // 检查命中结果是否为UIElement
                    if (hitResult.VisualHit is UIElement uiElement)
                    {
                        // 我们查找这个UIElement的祖先（或自身）是否是ModuleBase
                        ModuleBase node = uiElement.GetVisualAncestor<ModuleBase>();
                        if (node != null)
                        {
                            element = node;
                            return HitTestResultBehavior.Stop;
                        }
                    }
                    return HitTestResultBehavior.Continue;
                }),
                new PointHitTestParameters(point)
            );
            if (element != null)
            {
                return element as ModuleBase;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 判断当前鼠标位置下是否有ArrowPolyline控件
        /// </summary>
        /// <param name="canvas">容器控件</param>
        /// <param name="point">鼠标的坐标</param>
        /// <returns></returns>
        public static ArrowPolyline GetArrowPolylineUnderPoint(Canvas canvas, Point point)
        {
            UIElement element = null;
            VisualTreeHelper.HitTest(
                canvas,
                null,
                new HitTestResultCallback((hitResult) =>
                {
                    // 检查命中结果是否为UIElement
                    if (hitResult.VisualHit is UIElement uiElement)
                    {
                        // 我们查找这个UIElement的祖先（或自身）是否是ArrowPolyline
                        ArrowPolyline line = uiElement as ArrowPolyline;
                        if (line != null)
                        {
                            element = line;
                            return HitTestResultBehavior.Stop;
                        }
                    }
                    return HitTestResultBehavior.Continue;
                }),
                new PointHitTestParameters(point)
            );
            if (element != null)
            {
                return element as ArrowPolyline;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取容器（Canvas）下所有指定类型的控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null) yield break;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T matchedChild)
                    yield return matchedChild;

                foreach (var nestedChild in FindVisualChildren<T>(child))
                    yield return nestedChild;
            }
        }
    }
}
