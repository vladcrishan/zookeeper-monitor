using System.Windows.Input;
using ZookeeperMonitor.Command;
using ZookeeperMonitor.Model;
using ZookeeperMonitor.View;

namespace ZookeeperMonitor.ViewModel
{
    class ConnectViewModel
    {
        private MainModel _mainModel;
        private ZKViewModel _zkViewModel;

        public delegate void ButtonClickEvent();
        public event ButtonClickEvent ButtonClicked = delegate { }; 

        public ConnectViewModel()
        {
            _mainModel = new MainModel();

            //HostName = "10.10.10.99"; // local zookeeper
            //HostName = "cargo301,cargo302,cargo303";
            ConnectCommand = new RelayCommand(ConnectAction);
        }

        public string HostName { get; set; }

        /// <summary>
        /// Commands
        /// </summary>
        public ICommand ConnectCommand { get; private set; }

        /// <summary>
        /// Actions
        /// </summary>
        public void ConnectAction()
        {
            // Event
            ButtonClicked();

            //Connect
            _mainModel.Connect(HostName);

            // View + ViewModel of ZooKeeper main window.
            _zkViewModel = new ZKViewModel(_mainModel);
            var zkView = new ZooKeeperView()
            {
                DataContext = _zkViewModel
            };
            zkView.ShowDialog();
        }

    }
}
