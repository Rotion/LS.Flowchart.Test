using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LS.Flowchart.Models.ProjectModel;
using LS.Flowchart.ViewModels.Settings;

namespace LS.Flowchart.Views.Settings
{
    /// <summary>
    /// CommunicationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationWindow : Window
    {
        public CommunicationWindow()
        {
            InitializeComponent();
        }

        private void DeviceTag_OnStartChange(object sender, bool isStart)
        {
            (this.DataContext as VM_CommunicationWindow).OnStartChange(sender, isStart);
        }

        private void DeviceTag_OnRefresh(bool isDelete)
        {
            (this.DataContext as VM_CommunicationWindow).OnRefresh(isDelete);
        }
    }
}
