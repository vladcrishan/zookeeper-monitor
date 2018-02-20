using System.Windows;
using System.Windows.Input;
using ZookeeperMonitor.Command;
using ZookeeperMonitor.Model;

namespace ZookeeperMonitor.ViewModel
{
    // ReSharper disable once InconsistentNaming
    class ZKViewModel
    {
        private MainModel _mainModel;

        public ZKViewModel(MainModel _mainModel)
        {
            this._mainModel = _mainModel;

            ExitCommand = new RelayCommand(ExitAction);
            ListCommand = new RelayCommand(ListAction);
            CreateCommand = new RelayCommand(CreateAction);
            DeleteCommand = new RelayCommand(DeleteAction);
            JoinCommand = new RelayCommand(JoinAction);
            WriteCommand = new RelayCommand(WriteAction);
            ReadCommand = new RelayCommand(ReadAction);
            WatchCommand = new RelayCommand(WatchAction);
            ListenCommand = new RelayCommand(ListenAction);
            UploadConfigCommand = new RelayCommand(UploadConfigAction);
            UploadFontsCommand = new RelayCommand(UploadFontsAction);
            TestCommand = new RelayCommand(TestAction);
        }

       
        public ICommand ExitCommand { get; private set; }
        public ICommand ListCommand { get; private set; }
        public ICommand CreateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand JoinCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand ReadCommand { get; private set; }
        public ICommand WatchCommand { get; private set; }
        public ICommand ListenCommand { get; private set; }
        public ICommand UploadConfigCommand { get; private set; }
        public ICommand UploadFontsCommand { get; private set; }
        public ICommand TestCommand { get; private set; }

        /// <summary>
        /// Actions
        /// </summary>
        public void ExitAction()
        {
            _mainModel.CloseConnection();
            Application.Current.Shutdown();
        }

        public void ListAction()
        {
            _mainModel.List();
        }

        public void CreateAction()
        {
            _mainModel.Create(false);
        }

        public void DeleteAction()
        {
            _mainModel.Delete();
        }

        public void JoinAction()
        {
            _mainModel.Create(true);
        }

        public void WriteAction()
        {
            _mainModel.Write();
        }

        public void ReadAction()
        {
            _mainModel.Read();
        }

        public void WatchAction()
        {
            _mainModel.Watch();
        }

        public void ListenAction()
        {
            _mainModel.Listen();
        }

        public void UploadConfigAction()
        {
            _mainModel.UploadConfig();
        }

        public void UploadFontsAction()
        {
            _mainModel.UploadFonts();
        }

        private void TestAction()
        {
            _mainModel.Test();
        }

        /// <summary>
        /// Getters & Setters _mainModel
        /// </summary>
        public MainModel MainModel
        {
            get { return _mainModel; }
            set { _mainModel = value; }
        }
    }
}
