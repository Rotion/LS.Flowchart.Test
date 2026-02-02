using LS.Flowchart;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.Models.ProjectModel.Parameters;
using LS.Flowchart.Views.Settings;

namespace LS.Flowchart.ViewModels.Settings
{
    public class VM_CameraWindow:BaseViewModel
    {
        public VM_CameraWindow() : base(typeof(CameraWindow))
        {
            (UIElement as CameraWindow).winBar.MouseMove += WinBar_MouseMove;
        }
        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as CameraWindow).DragMove();
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
        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
        }

        public override void LoadData()
        {            
            try
            {
                base.LoadData();
                CmaeraList.Clear();
                if (GlobalData.CurrentProject.CameraList != null && GlobalData.CurrentProject.CameraList.Count > 0)
                {
                    CmaeraList = new ObservableCollection<ProjectCamera>(GlobalData.CurrentProject.CameraList);
                    if (SelectedCamera == null)
                    {
                        SelectedCamera = CmaeraList[0];
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("LoadData", ex);
            }
        }



        public DelegateCommand AddCameraCommand
        {
            get { return new DelegateCommand(AddCamera); }
        }
        /// <summary>
        /// 添加相机
        /// </summary>
        /// <param name="obj"></param>
        private void AddCamera(object obj)
        {
            try 
            {
                VM_AddCameraWindow vm = new VM_AddCameraWindow();
                vm.ShowDialog();
            }
            catch(Exception ex)
            {
                LogOperate.Error("AddCamera", ex);
            }
        }





        private ObservableCollection<ProjectCamera> _CmaeraList = new ObservableCollection<ProjectCamera>();
        private ProjectCamera _SelectedCamera;
        /// <summary>
        /// 相机列表
        /// </summary>
        public ObservableCollection<ProjectCamera> CmaeraList
        {
            get { return _CmaeraList; }
            set { _CmaeraList = value;OnPropertyChanged(); }
        }
        /// <summary>
        /// 当前选择的相机
        /// </summary>
        public ProjectCamera SelectedCamera
        {
            get { return _SelectedCamera; }
            set { _SelectedCamera = value; OnPropertyChanged(); }   
        }

    }
}
