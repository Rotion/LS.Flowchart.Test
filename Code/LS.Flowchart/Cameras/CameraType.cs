using LS.Standard.Data;
using LS.WPF.Core.MVVM.StandardModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using LS.Flowchart.Cameras.CameraProudct;
using LS.Flowchart.Cameras.Product;
using LS.WPF.Core.MVVM.StandardModel;

namespace LS.Flowchart.Cameras
{
    /// <summary>
    /// 相机类型
    /// </summary>
    public enum CameraType
    {
        [CameraClass(typeof(LocalImageCamera))]
        [EnumName("本地图像")]
        LocalImage = 0,

        [CameraClass(typeof(HikVisonCamera))]
        [EnumName("海康相机")]
        HikVison = 1,
    }


    /// <summary>
    /// 获取实时的图像数据 委托
    /// </summary>
    public delegate void DelegateReceiveImage(Bitmap bmp);

    /// <summary>
    /// 相机类型扩展
    /// </summary>
    public static class CameraTypeExtensions
    {
        public static IEnumerable<DropDownModel> GetCameraTypeInfo(this CameraType cameraType)
        {
            var enumType = typeof(CameraType);
            var values = Enum.GetValues(enumType) as CameraType[];

            foreach (var value in values)
            {
                var field = enumType.GetField(value.ToString());
                if (field == null) continue;

                // 获取特性值
                var enumNameAttr = field.GetCustomAttribute<EnumNameAttribute>();
                yield return new DropDownModel
                {
                    Value = value.ToString(),
                    Name = enumNameAttr?.CNName ?? value.ToString(),
                    Content = value
                };
            }
        }
    }
}
