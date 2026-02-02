using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.Models.ToolModels;
using LS.Standard.Data;
using LS.Standard.Helper;
using LS.WPF.Core.MVVM;
using Newtonsoft.Json;
using System.IO;

namespace LS.Flowchart.Operation
{
    /// <summary>
    /// 方案文件操作
    /// </summary>
    public static class ProjectFileOperation
    {
        /// <summary>
        /// 加密KEY
        /// </summary>
        private static AES_KEY _KEY = AESEncript.DefaultKey1;

        /// <summary>
        /// 初始化时，创建方案的类型
        /// </summary>
        /// <param name="projectType"></param>
        /// <returns></returns>
        public static BaseResult InitCreateProject(ProjectType projectType)
        {
            try
            {
                switch (projectType)
                {
                    case ProjectType.Blank:
                        AddDefaultProcess();
                        break;
                    case ProjectType.Location:
                        break;
                    case ProjectType.Defect:
                        break;
                    case ProjectType.QrCode:
                        break;
                }
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("InitCreateProject", ex);
                return new BaseResult(false, $"创建方案失败,{ex.Message}");
            }
        }

        /// <summary>
        /// 添加默认的流程
        /// 创建空白的流程
        /// </summary>
        private static void AddDefaultProcess()
        {
            try
            {
                if (GlobalData.InitProjectFinish)
                {
                    ProjectOperation.StopProject();
                }

                GlobalData.CurrentProject = new ProjectData();
                GlobalData.CurrentProject.Name = "";
                GlobalData.CurrentProject.FilePath = "";

                GlobalData.CurrentProject.ProcessList = new System.Collections.Generic.List<ProcessData>();

                ProcessData process = new ProcessData();
                process.Index = 1;
                process.Name = $"流程{process.Index}";
                process.ModuleItems = new System.Collections.Generic.List<ModuleItemModel>();
                GlobalData.CurrentProject.ProcessList.Add(process);
                //启动方案
                ProjectOperation.StartProject();
            }
            catch (Exception ex)
            {
                LogOperate.Error("AddDefaultProcess", ex);
            }
        }


        /// <summary>
        /// 打开方案文件
        /// 并将数据加载到 GlobalData.CurrentProject
        /// </summary>
        /// <param name="file">方案文件路径</param>
        public static BaseResult OpenProject(string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return new BaseResult(false, "文件不存在");
                }

                //读取加密内容
                var data = File.ReadAllText(file);
                //解密
                var content = AESEncript.Decrypt(data, _KEY.Key, _KEY.Iv);
                var project = JsonConvert.DeserializeObject<ProjectData>(content);

                if (GlobalData.InitProjectFinish)
                {
                    ProjectOperation.StopProject();
                }
                //更新方案信息
                GlobalData.CurrentProject = project;
                ProjectOperation.StartProject();
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("OpenProject", ex);
                return new BaseResult(false, $"打开方案失败,{ex.Message}");
            }
        }

        /// <summary>
        /// 保存方案
        /// </summary>
        /// <param name="file">方案文件的保存路径</param>
        /// <returns></returns>
        public static BaseResult SaveProject(string file)
        {
            try
            {
                var project = GlobalData.CurrentProject;
                if (project.FilePath != file)
                {
                    project.FilePath = file;
                }
                //序列化
                string content = JsonConvert.SerializeObject(project);
                //内容加密
                string data = AESEncript.Encrypt(content, _KEY.Key, _KEY.Iv);
                //写入文件
                File.WriteAllText(file, data);
                return BaseResult.Successed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("SaveProject", ex);
                return new BaseResult(false, $"保存方案失败,{ex.Message}");
            }
        }
    }
}
