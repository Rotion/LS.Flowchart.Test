using LS.Flowchart.Cameras.CameraProudct;
using LS.Flowchart.Models.ProjectModel;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System.Windows;
using System.Windows.Controls;

namespace LS.Flowchart.Cameras.CameraControls
{
    /// <summary>
    /// LocalImageCameraParamControl.xaml 的交互逻辑
    /// </summary>
    public partial class LocalImageCameraParamControl : UserControl
    {
        public LocalImageCameraParamControl()
        {
            InitializeComponent();
            this.DataContext = new VM_LocalImageCameraParamControl(this);
        }

        public ProjectCamera Project_Camera => (this.DataContext as VM_LocalImageCameraParamControl)?.Project_Camera;

    }

    public class VM_LocalImageCameraParamControl : BaseViewModel
    {
        public VM_LocalImageCameraParamControl(LocalImageCameraParamControl ui) : base(ui) { }


        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
        }

        public override void LoadData()
        {
            try
            {
                base.LoadData();
                Project_Camera = new ProjectCamera();
                Project_Camera.Camera_Type = CameraType.LocalImage;
                Project_Camera.CameraParam = new LocalImageCameraParam();
            }
            catch (Exception ex)
            {
                LogOperate.Error("VM_LocalImageCameraParamControl LoadData", ex);
            }
        }


        public DelegateCommand OpenFolderCommand
        {
            get { return new DelegateCommand(OpenFolder); }
        }
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="obj"></param>
        private void OpenFolder(object obj)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        var folderPath = dialog.SelectedPath;
                        if (!string.IsNullOrEmpty(folderPath))
                        {
                            var param = Project_Camera.CameraParam as LocalImageCameraParam;
                            param.ImageFolder = folderPath;
                            Project_Camera.CameraParam = param;
                            OnPropertyChanged("Project_Camera");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("VM_LocalImageCameraParamControl OpenFolder", ex);
            }
        }





        #region 属性

        private ProjectCamera projectCamera;
        /// <summary>
        /// 相机设备属性
        /// </summary>
        public ProjectCamera Project_Camera
        {
            get { return projectCamera; }
            set { projectCamera = value; OnPropertyChanged(); }
        }


        #endregion


    }
}
