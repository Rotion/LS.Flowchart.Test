using LS.Flowchart;
using LS.Flowchart.ViewModels;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LS.Flowchart.Models.Configs;
using LS.Flowchart.Operation;

namespace LS.Flowchart.Views
{
    /// <summary>
    /// InitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InitWindow : Window
    {
        public InitWindow()
        {
            InitializeComponent();
            this.DataContext = new VM_InitWindow(this);
        }
    }

    public class VM_InitWindow : BaseViewModel
    {
        public VM_InitWindow(InitWindow ui) : base(ui) { }

        public override void LoadData()
        {
            try
            {
                base.LoadData();

                NoShow = !GlobalData.ConfigParams.ShowInitWindow;

                // 正则表达式：匹配前三个版本号字段
                string pattern = @"(?i)^V(?<importantVersion>\d+\.\d+\.\d+)\.";

                Match match = Regex.Match(GlobalData.Version, pattern);
                if (match.Success)
                {
                    // 提取命名分组中的值
                    Version = "V" + match.Groups["importantVersion"].Value;
                }
                else
                {
                    Version = GlobalData.Version;
                }

                if (GlobalData.HistoryFiles != null && GlobalData.HistoryFiles.HistoryFileList != null && GlobalData.HistoryFiles.HistoryFileList.Count > 0)
                {
                    //降序排序
                    GlobalData.HistoryFiles.HistoryFileList.Sort((x, y) => { return -1 * x.OpenTime.CompareTo(y.OpenTime); });

                    File1 = GlobalData.HistoryFiles.HistoryFileList.Count > 0 ? GlobalData.HistoryFiles.HistoryFileList[0] : new HistoryFile();
                    File2 = GlobalData.HistoryFiles.HistoryFileList.Count > 1 ? GlobalData.HistoryFiles.HistoryFileList[1] : new HistoryFile();
                    File3 = GlobalData.HistoryFiles.HistoryFileList.Count > 2 ? GlobalData.HistoryFiles.HistoryFileList[2] : new HistoryFile();
                    File4 = GlobalData.HistoryFiles.HistoryFileList.Count > 3 ? GlobalData.HistoryFiles.HistoryFileList[3] : new HistoryFile();
                    File5 = GlobalData.HistoryFiles.HistoryFileList.Count > 4 ? GlobalData.HistoryFiles.HistoryFileList[4] : new HistoryFile();
                    File6 = GlobalData.HistoryFiles.HistoryFileList.Count > 5 ? GlobalData.HistoryFiles.HistoryFileList[5] : new HistoryFile();

                    File1 = GetTitleName(File1);
                    File2 = GetTitleName(File2);
                    File3 = GetTitleName(File3);
                    File4 = GetTitleName(File4);
                    File5 = GetTitleName(File5);
                    File6 = GetTitleName(File6);

                }

            }
            catch (Exception ex)
            {
                LogOperate.Error("VM_InitWindow_LoadData", ex);
            }

        }

        /// <summary>
        /// 计算文件的时间描述
        /// </summary>
        /// <param name="file"></param>
        private HistoryFile GetTitleName(HistoryFile file)
        {
            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                var ts = DateTime.Now.Subtract(file.OpenTime);


                if (ts.TotalDays > 1)
                {
                    if (ts.TotalDays > 30)
                    {
                        file.TimeDescription = "1个月前";
                    }
                    else
                    {
                        file.TimeDescription = $"{(int)ts.TotalDays}天前";
                    }
                }
                else if (ts.TotalHours > 1)
                {
                    file.TimeDescription = $"{(int)ts.TotalHours}小时前";
                }
                else if(ts.TotalMinutes > 0)
                {
                    if (ts.TotalMinutes < 30)
                    {
                        file.TimeDescription = "刚刚";
                    }
                    else
                    {
                        file.TimeDescription = $"{(int)ts.TotalMinutes}分前";
                    }
                }
            }
            return file;
        }

        protected override void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            base.Page_Unloaded(sender, e);

            GlobalData.ConfigParams.ShowInitWindow = !NoShow;
            ConfigParamOperation.SaveConfigParam(GlobalData.ConfigParams);

            MainWindow main = new MainWindow();
            VM_MainWindow.Instance.Show();
            //VM_MainWindow.Instance.LoadData();
        }


        public DelegateCommand OpCommand
        {
            get { return new DelegateCommand(Op); }
        }

        private void Op(object obj)
        {
            try
            {
                if (obj != null)
                {
                    switch (obj.ToString())
                    {
                        case "Close":
                            //默认创建空白方案
                            var res = ProjectFileOperation.InitCreateProject(Models.ProjectModel.ProjectType.Blank);
                            if (!res.IsSucessed)
                            {
                                VM_MainWindow.Popup(res.Message);
                            }
                            else
                            {
                                this.Close();
                            }
                            break;
                        case "Blank":
                            var res1 = ProjectFileOperation.InitCreateProject(Models.ProjectModel.ProjectType.Blank);
                            if (!res1.IsSucessed)
                            {
                                VM_MainWindow.Popup(res1.Message);
                            }
                            else
                            {
                                this.Close();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Op(object obj)", ex);
            }
        }

        public DelegateCommand FileOpenCommand
        {
            get { return new DelegateCommand(FileOpen); }
        }

        private void FileOpen(object obj)
        {
            try
            {
                if (obj != null)
                {
                    HistoryFile file = obj as HistoryFile;
                    var res = ProjectFileOperation.OpenProject(file.FilePath);
                    if (res)
                    {
                        this.Close();
                    }
                    else
                    {
                        VM_MainWindow.Popup(res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("FileOpen", ex);
            }
        }


        private string _version;
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; OnPropertyChanged(); }
        }

        private bool _noShow = false;
        /// <summary>
        /// 不再提示
        /// </summary>
        public bool NoShow
        {
            get { return _noShow; }
            set
            {
                _noShow = value;
                OnPropertyChanged();
            }
        }


        private HistoryFile _file1 = new HistoryFile();
        private HistoryFile _file2 = new HistoryFile();
        private HistoryFile _file3 = new HistoryFile();
        private HistoryFile _file4 = new HistoryFile();
        private HistoryFile _file5 = new HistoryFile();
        private HistoryFile _file6 = new HistoryFile();

        public HistoryFile File1
        {
            get { return _file1; }
            set { _file1 = value; OnPropertyChanged(); }
        }
        public HistoryFile File2
        {
            get { return _file2; }
            set { _file2 = value; OnPropertyChanged(); }
        }
        public HistoryFile File3
        {
            get { return _file3; }
            set { _file3 = value; OnPropertyChanged(); }
        }
        public HistoryFile File4
        {
            get { return _file4; }
            set { _file4 = value; OnPropertyChanged(); }
        }
        public HistoryFile File5
        {
            get { return _file5; }
            set { _file5 = value; OnPropertyChanged(); }
        }
        public HistoryFile File6
        {
            get { return _file6; }
            set { _file6 = value; OnPropertyChanged(); }
        }

    }
}
