using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using ZooKeeperNet;

namespace ZookeeperMonitor.Model
{
    class ConnectionWatcher : IWatcher, INotifyPropertyChanged
    {
        private readonly TimeSpan _sessionTimeout = new TimeSpan(0, 0, 5);
        protected ZooKeeper Zk;
        private String _status;
        private readonly CountdownEvent _connectedSignal = new CountdownEvent(1);



        public ConnectionWatcher()
        {
            Output = new StringBuilder();
            _status = "Status";
        }

        public void Connect(string host)
        {
            Zk = new ZooKeeper(host, _sessionTimeout, this);
            Status = "Connecting...";

            // If no zookeeper server active
            var timeOutThread = new Thread(() =>
            {
                // Wait 3 sec for zookeeper to connect
                Thread.Sleep(3000);
                if (_connectedSignal.CurrentCount != 0)
                {
                    _connectedSignal.Signal();
                }
            });
            timeOutThread.Start();

            _status = "Connection failed";
            _connectedSignal.Wait();
        }

        public void CloseConnection()
        {
            if (Zk == null) return;
            Status = "Connection Closed";
            Zk.Dispose();
        }

        public void Process(WatchedEvent @event)
        {
            if (@event.State == KeeperState.SyncConnected && _connectedSignal.CurrentCount != 0)
            {
                _connectedSignal.Signal();
                Status = "Connection Established!";
            }

            AppendLineShow(String.Format("Just hand an event: {0} ", @event.Type));

            if (@event.Type == EventType.NodeChildrenChanged)
            {
                const string pdfPath = "/PDFConverter";

                // Place the watcher again
                Zk.GetChildren(pdfPath, true);

                // List znodes + values
                try
                {
                    var children = Zk.GetChildren(pdfPath, false);
                    IEnumerable<string> enumerable = children as string[] ?? children.ToArray();

                    if (enumerable.IsEmpty())
                    {
                        AppendLineShow("No members in znode: " + pdfPath);
                    }
                    else
                    {
                        AppendLineShow("\n======= Z =======");
                        foreach (var child in enumerable)
                        {
                            var path = pdfPath + "/" + child;
                            var data = Zk.GetData(path, this, null /*stat*/);
                            AppendLineShow(child + "  -  " + (data != null ? Encoding.UTF8.GetString(data) : ""));
                        }
                        AppendLineShow("\n");
                    }
                }
                catch (KeeperException.NoNodeException e)
                {
                    AppendLineShow("Znode does not exist!");
                }
                catch (KeeperException.SessionExpiredException e)
                {
                    AppendLineShow("The session expired!");
                }
                catch (InvalidOperationException e)
                {
                    AppendLineShow("Too many '/'!");
                }
            }
        }

        /// <summary>
        /// Getters & Setters for _status
        /// </summary>
        public String Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        /// <summary>
        /// Getter & Setters for _output
        /// </summary>
        public StringBuilder Output { get; set; }

        public void AppendLineShow(string s)
        {
            Output.AppendLine(s);
            OnPropertyChanged("Output");
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
