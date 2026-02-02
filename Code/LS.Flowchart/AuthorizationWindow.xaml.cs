using LS.Flowchart.Models.Configs;
using LS.Flowchart.Tools;
using LS.Flowchart.ViewModels;
using LS.Flowchart.Views;
using LS.WPF.Core.MVVM;
using LS.WPF.Core.MVVM.Command;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace LS.Flowchart
{
    /// <summary>
    /// AuthorizationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
            this.DataContext = new VM_AuthorizationWindow(this);
            this.Closed += AuthorizationWindow_Closed;
        }

        /// <summary>
        /// 授权窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthorizationWindow_Closed(object sender, EventArgs e)
        {
            if (GlobalData.ConfigParams.CloseType == SoftCloseType.DirectClose)
            {
                Environment.Exit(0);
                return;
            }
        }
    }

    public class VM_AuthorizationWindow : BaseViewModel
    {
        public static VM_AuthorizationWindow Instance { get; private set; }

        public VM_AuthorizationWindow(AuthorizationWindow ui) : base(ui)
        {
            Instance = this;
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
                GlobalData.SN =  AuthorizationHelper.GetCPUSN();
                SN = GlobalData.SN;

                Task.Run(() =>
                {
                    Thread.Sleep(2000);
                    CheckAuthorization();
                });
            }
            catch (Exception ex)
            {
                LogOperate.Error("LoadData", ex);
            }
        }


        public void CheckAuthorization()
        {
            //判断授权
            try
            {

                string file = AppDomain.CurrentDomain.BaseDirectory + "license.lic";
                if (!File.Exists(file))
                {
                    //授权文件不存在
                    GlobalData.AuthorizationMessage = "未获取授权文件，请联系开发商获取授权文件";
                    VM_MainWindow.Popup(GlobalData.AuthorizationMessage);
                    ShowSN = Visibility.Visible;
                    return;
                }
                else
                {
                    var content = File.ReadAllText(file);
                    AuthorizationHelper.LicenseModel license = null;
                    try
                    {
                        license = JsonConvert.DeserializeObject<AuthorizationHelper.LicenseModel>(content);
                    }
                    catch (Exception ex)
                    {
                        LogOperate.Error("授权文件解析异常", ex);
                        GlobalData.AuthorizationMessage = "授权文件解析异常，请联系开发商重新获取授权文件";
                        VM_MainWindow.Popup(GlobalData.AuthorizationMessage);
                        ShowSN = Visibility.Visible;
                        return;
                    }

                    var res = AuthorizationHelper.DecodeLicense(GlobalData.SN, license);
                    if (!res.IsSucessed)
                    {
                        GlobalData.AuthorizationMessage = $"授权文件无效，{res.Message},请联系开发商获取授权文件";
                        VM_MainWindow.Popup(GlobalData.AuthorizationMessage);
                        ShowSN = Visibility.Visible;
                        return;
                    }

                    this.Hide();
                    if (GlobalData.ConfigParams.ShowInitWindow)
                    {
                        //显示初始界面
                        VM_AuthorizationWindow.Instance.DoMenthodByDispatcher(() =>
                        {
                            InitWindow init = new InitWindow();
                            init.Show();
                        });
                    }
                    else
                    {
                        //显示主界面
                        MainWindow main = new MainWindow();
                        VM_MainWindow.Instance.Show();
                        //VM_MainWindow.Instance.LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperate.Error("判断授权异常", ex);
                GlobalData.AuthorizationMessage = "未获取授权文件，请联系开发商获取授权文件";
                VM_MainWindow.Popup(GlobalData.AuthorizationMessage);
                return;
            }
        }

        public DelegateCommand CloseCommand
        {
            get { return new DelegateCommand(Close); }
        }

        private void Close(object obj)
        {
            try
            {
                Instance.Close();
            }
            catch (Exception ex)
            {
                LogOperate.Error("Close ", ex);
            }
        }

        private Visibility _showSN = Visibility.Hidden;

        public Visibility ShowSN
        {
            get { return _showSN; }
            set { _showSN = value; OnPropertyChanged(); }
        }

        private string _sn;

        public string SN
        {
            get { return _sn; }
            set { _sn = value; OnPropertyChanged(); }
        }

    }
}
