using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using LS.WPF.Core.MVVM.StandardModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LS.Flowchart.Cameras;
using LS.Flowchart.Cameras.CameraControls;
using LS.Flowchart.Models.ProjectModel.Parameters;
using LS.Flowchart.Views.Settings;

namespace LS.Flowchart.ViewModels.Settings
{
    internal class VM_AddCameraWindow:BaseViewModel
    {
        public VM_AddCameraWindow() : base(typeof(AddCameraWindow))
        {
            (UIElement as AddCameraWindow).winBar.MouseMove += WinBar_MouseMove;
        }

        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
        }

        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as AddCameraWindow).DragMove();
            }
        }

        public override void LoadData()
        {
            try
            {
                base.LoadData();

                CameraTypeList.Clear();
                var cameraTypes = CameraType.LocalImage.GetCameraTypeInfo();
                if (cameraTypes != null && cameraTypes.Any())
                {
                    CameraTypeList = new ObservableCollection<DropDownModel>(cameraTypes);
                    if(SelectedCameraType==null)
                    {
                        SelectedCameraType = CameraTypeList.FirstOrDefault();
                    }
                }
            }
            catch(Exception ex)
            {
                LogOperate.Error("VM_AddCameraWindow LoadData", ex);
            }
        }

        public DelegateCommand CloseCommand
        {
            get { return new DelegateCommand(Close); }
        }

        private void Close(object obj)
        {
            this.Close();
        }

        /// <summary>
        /// 选择相机类型 改变时触发
        /// </summary>
        private void SelectedCameraType_OnChange()
        {
            try 
            {
                if (SelectedCameraType==null)
                {
                    return;
                }

                (UIElement as AddCameraWindow)._cameraParmGrid.Children.Clear();
                switch ((CameraType)SelectedCameraType.Content)
                {
                    case CameraType.LocalImage:                        
                        (UIElement as AddCameraWindow)._cameraParmGrid.Children.Add(new LocalImageCameraParamControl());
                        break;
                }
            }
            catch(Exception ex)
            {
                LogOperate.Error("SelectedCameraType_OnChange", ex);
            }
        }


        #region 属性

        private string _CameraName;
        /// <summary>
        /// 相机名称
        /// </summary>
        public string CameraName
        {
            get { return _CameraName; }
            set
            {
                _CameraName = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DropDownModel> _CameraTypeList = new ObservableCollection<DropDownModel>();
        private DropDownModel _SelectedCameraType;
        /// <summary>
        /// 相机类型列表
        /// </summary>
        public ObservableCollection<DropDownModel> CameraTypeList
        {
            get { return _CameraTypeList; }
            set
            {
                _CameraTypeList = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 选择的相机类型
        /// </summary>
        public DropDownModel SelectedCameraType
        {
            get { return _SelectedCameraType; }
            set
            {
                _SelectedCameraType = value;
                OnPropertyChanged();
                SelectedCameraType_OnChange();
            }
        }



        #endregion
    }
}
