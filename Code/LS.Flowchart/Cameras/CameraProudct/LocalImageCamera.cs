using LS.Standard.Data;
using LS.WPF.Core.MVVM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Flowchart.Models.ProjectModel;

namespace LS.Flowchart.Cameras.CameraProudct
{
    public class LocalImageCamera:ICamera
    {
        ProjectCamera _projectCamera;

        public LocalImageCamera(ProjectCamera projectCamera)
        {
            _projectCamera = projectCamera;
        }

        public string ID => _projectCamera?.ID;

        public ProjectCamera Project_Camera => _projectCamera;
        /// <summary>
        /// 相机参数
        /// </summary>
        public dynamic CameraParam { get; set; }

        public BaseResult SetExposure(uint exposure)
        {
            return BaseResult.Successed;
        }

        public BaseResult SetGain(uint gain)
        {
            return BaseResult.Successed;
        }

        public BaseResult SetParam(dynamic cameraParam)
        {
            try
            {
                if (cameraParam != null && cameraParam is LocalImageCameraParam)
                {
                    CameraParam = cameraParam;
                    var param = cameraParam as LocalImageCameraParam;
                    ImageFolder = param.ImageFolder;
                    IsLoop = param.IsLoop;
                    ImageIndex = -1;

                    //寻找文件夹内的图片，加载到缓存中
                    //只查找第一层的文件
                    //支持的格式：jpg,png,bmp
                    var ImgFormat = new List<string>() { ".png", ".bmp", ".jpg", ".jpeg" };
                    Images.Clear();
                    if (Directory.Exists(ImageFolder))
                    {
                        var files = Directory.GetFiles(ImageFolder);
                        if (files != null && files.Length > 0)
                        {
                            for (int i = 0; i < files.Length; i++)
                            {
                                var file = files[i];
                                var ext = Path.GetExtension(file);
                                if (ImgFormat.Contains(ext))
                                {
                                    var image = Image.FromFile(file);
                                    Images.Add(image);
                                }
                            }
                        }
                    }
                }
                else
                {
                    return new BaseResult(false, "参数为空");
                }
                return BaseResult.Successed;
            }
            catch(Exception ex)
            {
                LogOperate.Error("LocalImageCamera SetParam", ex);
                return new BaseResult(false,ex.Message);
            }
        }

        public BaseResult Triggertrap()
        {
            if (Images.Count <= 0)
            {
                return new BaseResult(false, "无图片数据");
            }

            ImageIndex++;

            if(ImageIndex>= Images.Count)
            {
                if (IsLoop)
                {
                    ImageIndex = 0;
                }
                else
                {
                    ImageIndex = Images.Count - 1;
                    return new BaseResult(false, "已到最后一张图片");
                }
            }

            Image image = Images[ImageIndex];
            Bitmap bmp=new Bitmap(image);
            OnReceiveImage?.Invoke(bmp);            
            return BaseResult.Successed;
        }

        /// <summary>
        /// 获取实时的图像数据 委托方法
        /// </summary>
        public event DelegateReceiveImage OnReceiveImage;

        /// <summary>
        /// 图片文件夹路径
        /// </summary>
        public string ImageFolder = "";

        /// <summary>
        /// 文件夹内目录的图片
        /// </summary>
        public List<Image> Images = new List<Image>();

        /// <summary>
        /// 当前输出的图像索引
        /// </summary>
        public int ImageIndex = -1;

        /// <summary>
        /// 是否循环播放
        /// 当显示最后一张图片时，是否从头开始
        /// </summary>
        public bool IsLoop = true;
    }

    /// <summary>
    /// 本地图像参数
    /// </summary>
    public class LocalImageCameraParam
    {
        /// <summary>
        /// 图片文件夹路径
        /// </summary>
        public string ImageFolder { get; set; } = "";

        /// <summary>
        /// 是否循环播放
        /// 当显示最后一张图片时，是否从头开始
        /// </summary>
        public bool IsLoop { get; set; } = true;
    }


}
