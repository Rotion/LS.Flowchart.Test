using LS.WPF.Core.MVVM;
using Microsoft.Win32;
using System;
using System.IO;
using LS.Flowchart.Operation;

namespace LS.Flowchart.ViewModels
{
    public partial class VM_MainWindow
    {
        #region 方案处理

        /// <summary>
        /// 保存方案
        /// </summary>
        private void SaveProject()
        {
            try
            {
                if (GlobalData.CurrentProject == null)
                {
                    VM_MainWindow.Popup("当前没有打开的方案", "提示");
                    return;
                }
                //保存方案
                if (string.IsNullOrEmpty(GlobalData.CurrentProject.FilePath))
                {
                    SaveFileDialog dialog = new SaveFileDialog();

                    dialog.Title = "保存方案";
                    dialog.Filter = "方案文件|*.pczd";
                    dialog.DefaultExt = "pczd";

                    if (dialog.ShowDialog() == true)
                    {
                        string path = dialog.FileName;
                        string name = Path.GetFileName(path);
                        GlobalData.CurrentProject.Name = name;
                        GlobalData.CurrentProject.FilePath = path;
                    }

                }
                if (!string.IsNullOrEmpty(GlobalData.CurrentProject.FilePath))
                {
                    var res = ProjectFileOperation.SaveProject(GlobalData.CurrentProject.FilePath);
                    if (res)
                    {
                        VM_MainWindow.Tip("方案保存成功");
                        GlobalData.AddHistoryFile(GlobalData.CurrentProject.Name, GlobalData.CurrentProject.FilePath);
                    }
                    else
                    {
                        VM_MainWindow.Popup($"方案保存失败,{res.Message}", "提示");
                    }
                }
                else
                {
                    VM_MainWindow.Popup("方案保存失败,未选择文件路径", "提示");
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("SaveProject", ex);
            }
        }


        /// <summary>
        /// 方案另存为
        /// </summary>
        private void SaveAs()
        {
            try
            {
                if (GlobalData.CurrentProject == null)
                {
                    VM_MainWindow.Popup("当前没有打开的方案", "提示");
                    return;
                }
                //另存为
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "另存为";
                dialog.Filter = "方案文件|*.pczd";
                dialog.DefaultExt = "pczd";
                if (dialog.ShowDialog() == true)
                {
                    string path = dialog.FileName;
                    string name = Path.GetFileName(path);
                    GlobalData.CurrentProject.Name = name;
                    GlobalData.CurrentProject.FilePath = path;
                    var res = ProjectFileOperation.SaveProject(GlobalData.CurrentProject.FilePath);
                    if (res)
                    {
                        VM_MainWindow.Tip("方案另存为成功");
                        GlobalData.AddHistoryFile(GlobalData.CurrentProject.Name, GlobalData.CurrentProject.FilePath);
                    }
                    else
                    {
                        VM_MainWindow.Popup($"方案另存为失败,{res.Message}", "提示");
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("SaveAs", ex);
            }
        }

        /// <summary>
        /// 打开方案文件
        /// </summary>
        private void OpenProject()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "打开方案";
                dialog.Filter = "方案文件|*.pczd";
                if (dialog.ShowDialog() == true)
                {
                    string path = dialog.FileName;
                    string name = Path.GetFileName(path);
                    GlobalData.CurrentProject.Name = name;
                    GlobalData.CurrentProject.FilePath = path;
                    var res = ProjectFileOperation.OpenProject(GlobalData.CurrentProject.FilePath);
                    if (res)
                    {
                        VM_MainWindow.Tip("方案打开成功");
                        GlobalData.AddHistoryFile(GlobalData.CurrentProject.Name, GlobalData.CurrentProject.FilePath);
                    }
                    else
                    {
                        VM_MainWindow.Popup($"方案打开失败,{res.Message}", "提示");
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("OpenProject", ex);
            }
        }

        #endregion
    }
}
