using LS.Flowchart;
using LS.Flowchart.ViewModels;
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
using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.Operation;
using LS.Flowchart.Views.Settings;

namespace LS.Flowchart.ViewModels.Settings
{
    public class VM_GlobalObjectWindow:BaseViewModel
    {
        public VM_GlobalObjectWindow():base(typeof(GlobalObjectWindow))
        {
            (UIElement as GlobalObjectWindow).winBar.MouseMove += WinBar_MouseMove;
        }

        private void WinBar_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                (UIElement as GlobalObjectWindow).DragMove();
            }
        }

        protected override void Page_Loaded(object sender, RoutedEventArgs e)
        {
            base.Page_Loaded(sender, e);
        }

        public override void LoadData()
        {
            base.LoadData();
            DataList=new ObservableCollection<ProjectGlobalObject>(GlobalData.CurrentProject.GlobalObjects);
        }

        public DelegateCommand CloseCommand
        {
            get { return new DelegateCommand(Close); }
        }

        private void Close(object obj)
        {
            this.Close();
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
                        case "Add":
                            isEdit = false;
                            Data = new ProjectGlobalObject();
                            ViewVis = Visibility.Visible;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Op(object obj)", ex);
            }
        }

        public DelegateCommand EditCommand
        {
            get { return new DelegateCommand(Edit); }
        }

        private void Edit(object obj)
        {
            try
            {
                if (obj != null && obj is ProjectGlobalObject)
                {
                    isEdit = true;
                    Data = (obj as ProjectGlobalObject);
                    ViewVis = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Edit(object obj)", ex);
            }
        }   

        public DelegateCommand DeleteCommand
        {
            get { return new DelegateCommand(Delete); }
        }

        private void Delete(object obj)
        {
            try
            {
                if (obj != null && obj is ProjectGlobalObject)
                {
                    var item = (obj as ProjectGlobalObject);
                    if(!VM_MainWindow.Popup2("确认删除全局变量 " + item.Name + " ？"))
                    { 
                        return;
                    }
                    var res= ProjectOperation.DeleteGlobalObject(item);
                    if (res)
                    {
                        if (DataList.Contains(item))
                        {
                            DataList.Remove(item);
                        }
                    }
                    else
                    {
                        VM_MainWindow.Popup(res.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Delete(object obj)", ex);
            }
        }

        public DelegateCommand SaveCommand
        {
            get { return new DelegateCommand(Save); }
        }

        private void Save(object obj)
        {
            try
            {
                if (Data != null)
                {
                    if (string.IsNullOrWhiteSpace(Data.Name))
                    {
                        VM_MainWindow.Popup("变量名称不能为空");
                        return;
                    }
                    if (isEdit)
                    {
                        var res = ProjectOperation.EditGlobalObject(Data);
                        if (res)
                        {
                            ViewVis = Visibility.Collapsed;
                        }
                        else
                        {
                            VM_MainWindow.Popup(res.Message);
                        }
                    }
                    else
                    {
                        var res = ProjectOperation.AddGlobalObject(Data);
                        if (res)
                        {
                            DataList.Add(Data);
                            ViewVis = Visibility.Collapsed;
                        }
                        else
                        {
                            VM_MainWindow.Popup(res.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("Save(object obj)", ex);
            }
        }

        public DelegateCommand CancelCommand
        {
            get { return new DelegateCommand(Cancel); }
        }

        private void Cancel(object obj)
        {
            try
            {
                ViewVis = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LogOperate.Error("Cancel(object obj)", ex);
            }
        }

        private bool isEdit = false;
        private Visibility _viewVis = Visibility.Collapsed;
        private ProjectGlobalObject _data = new ProjectGlobalObject();
        private ObservableCollection<ProjectGlobalObject> _dataList = new ObservableCollection<ProjectGlobalObject>();
        /// <summary>
        /// 全局变量数据列表
        /// </summary>
        public ObservableCollection<ProjectGlobalObject> DataList
        {
            get { return _dataList; }
            set { _dataList = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 编辑或新增的数据对象
        /// </summary>
        public ProjectGlobalObject Data
        {
            get { return _data; }
            set { _data = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 新增或编辑界面的可视状态
        /// </summary>
        public Visibility ViewVis
        {
            get { return _viewVis; }
            set { _viewVis = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DropDownModel> _objectTypes=new ObservableCollection<DropDownModel>()
        {
            new DropDownModel(){ Name=ObjectType.String,Value=ObjectType.String},
            new DropDownModel(){ Name=ObjectType.Int,Value=ObjectType.Int},
            new DropDownModel(){ Name=ObjectType.Double,Value=ObjectType.Double},
            new DropDownModel(){ Name=ObjectType.Float,Value=ObjectType.Float},
            new DropDownModel(){ Name=ObjectType.Bool,Value=ObjectType.Bool},
        };
        private DropDownModel _selectType;
        /// <summary>
        /// 变量类型
        /// </summary>
        public ObservableCollection<DropDownModel> ObjectTypes
        {
            get { return _objectTypes; }
            set { _objectTypes = value; OnPropertyChanged(); }
        }
        public DropDownModel SelectType
        {
            get { return _selectType; }
            set
            {
                _selectType = value;
                if (Data != null && _selectType != null)
                {
                    switch (_selectType.Name)
                    {
                        case ObjectType.String:
                            Data.DefaultValue = default(string);
                            break;
                         case ObjectType.Int:
                            Data.DefaultValue = default(int);
                            break;
                        case ObjectType.Double:
                            Data.DefaultValue = default(double);
                            break;
                        case ObjectType.Float:
                            Data.DefaultValue = default(float);
                            break;
                        case ObjectType.Bool:
                            Data.DefaultValue = default(bool);
                            break;
                    }
                    OnPropertyChanged(nameof(Data));
                }
                OnPropertyChanged();
            }
        }
    }
}
