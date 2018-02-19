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
            // Position window in the center of the screen
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            InitializeComponent();
            var connectViewModel = new ConnectViewModel();

            // Data binding
            DataContext = connectViewModel;

            // Hide window after pass
            connectViewModel.ButtonClicked += this.Hide;
        }

    }
}
