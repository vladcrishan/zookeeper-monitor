using System.Windows;
using ZookeeperMonitor.ViewModel;

namespace ZookeeperMonitor.View
{
    /// <summary>
    /// Interaction logic for ConnectView.xaml
    /// </summary>
    public partial class ConnectView : Window
    {
        public ConnectView()
        {
            InitializeComponent();
            var connectViewModel = new ConnectViewModel();

            DataContext = connectViewModel;
            connectViewModel.ButtonClicked += this.Hide;
        }
    }
}
