using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ZooKeeperNet;
using FontFamily = System.Drawing.FontFamily;

namespace ZookeeperMonitor.Model
{
    class MainModel : ConnectionWatcher
    {
        private string _input1;
        private string _input2;
        private string _path;

        private static String GetPath(string znode)
        {
            if (znode == null)
            {
                return "/";
            }

            if (!znode.Equals("/"))
            {
                return "/" + znode;
            }

            return "/";
        }

        /// <summary>
        /// Lists the znodes
        /// </summary>
        public void List()
        {
            _path = GetPath(_input1);

            try
            {
                var children = Zk.GetChildren(_path, false);
                IEnumerable<string> enumerable = children as string[] ?? children.ToArray();

                if (enumerable.IsEmpty())
                {
                    AppendLineShow("No members in znode: " + _input1);
                }
                else
                {
                    AppendLineShow("\n======= Z =======");
                    foreach (var child in enumerable)
                    {
                        Output.Append(child + "   ");
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

        /// <summary>
        /// Creates a znode
        /// </summary>
        /// <param name="eph"> eph stands for Ephemeral</param>
        public void Create(bool eph)
        {

            _path = GetPath(eph ? _input1 + "/" + _input2 : _input1);

            try
            {
                var createdPath = Zk.Create(_path, null, Ids.OPEN_ACL_UNSAFE,
                    eph ? CreateMode.EphemeralSequential : CreateMode.Persistent);
                AppendLineShow("Created: " + createdPath);
            }
            catch (KeeperException e)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
            catch (InvalidOperationException e)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
        }

        /// <summary>
        /// Deletes a znode
        /// </summary>
        public void Delete()
        {
            _path = GetPath(_input1);

            try
            {
                IEnumerable<String> children = Zk.GetChildren(_path, false);
                foreach (var child in children)
                {
                    Zk.Delete(_path + "/" + child, -1);
                }

                Zk.Delete(_path, -1);
                AppendLineShow(String.Format("Znode {0} was deleted successfully\n", _input1));
            }
            catch (KeeperException.NoNodeException e)
            {
                AppendLineShow(String.Format("Znode {0} does not exist!", _input1));
            }
            catch (InvalidOperationException e)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
            catch (Exception e)
            {
                AppendLineShow("Exception " + e);
            }
        }

        /// <summary>
        /// Writes a value in znode
        /// </summary>
        public void Write()
        {
            _path = GetPath(_input1);

            try
            {
                var stat = Zk.Exists(_path, false);
                if (stat == null)
                {
                    Zk.Create(_path, Encoding.Default.GetBytes(_input2), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                    AppendLineShow(String.Format("Znode {0} does not exist! Creating one..", _input1));
                }
                else
                {
                    Zk.SetData(_path, Encoding.Default.GetBytes(_input2), -1);
                    AppendLineShow(String.Format("{0} set value {1}", _input1, _input2));
                }
            }
            catch (KeeperException e)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
            catch (InvalidOperationException e)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
            catch (ArgumentNullException e)
            {
                AppendLineShow("Insert in textboxes!");
            }
        }

        /// <summary>
        /// Read value from znode
        /// </summary>
        public void Read()
        {
            _path = GetPath(_input1);
            try
            {
                var data = Zk.GetData(_path, this, null /*stat*/);
                AppendLineShow(String.Format("Value of {0} is {1}", _input1, Encoding.UTF8.GetString(data)));

            }
            catch (KeeperException.NoNodeException)
            {
                AppendLineShow("Znode does not exist!");
            }
            catch (KeeperException)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
            catch (InvalidOperationException)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
        }

        public void Watch()
        {
            _path = GetPath(_input1);

            try
            {
                Zk.Exists(_path, this);
                AppendLineShow(String.Format("Watch added to {0}", _input1));
            }
            catch (KeeperException)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
            catch (InvalidOperationException e)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
        }

        public void Listen()
        {
            const string pdfPath = "/PDFConverter";
            try
            {
                Zk.GetChildren(pdfPath, true);
                AppendLineShow(string.Format("A watch on {0}'s children has been placed.", pdfPath));
            }
            catch (KeeperException.NoNodeException e)
            {
                AppendLineShow(String.Format("Znode {0} does not exist!", _input1));
            }
            catch (InvalidOperationException e)
            {
                AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
            }
        }

        internal void UploadConfig()
        {
            var keys = ConfigurationManager.AppSettings;
            _path = "/FirmID-ClientNames";

            // Create parent folder if != exist
            if (Zk.Exists(_path, false) == null)
            {
                try
                {
                    var createdPath = Zk.Create(_path, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                    AppendLineShow("Created: " + createdPath);
                }
                catch (KeeperException e)
                {
                    AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
                }
                catch (InvalidOperationException e)
                {
                    AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
                }
            }

            // Upload to zookeeper
            AppendLineShow("\n+++ Write in ZK +++");
            foreach (var key in keys)
            {
                var value = keys.GetValues(key.ToString());
                if (value != null) _input1 = "FirmID-ClientNames/" + (string)value.GetValue(0);
                _input2 = key.ToString();
                Write();
            }


            // Show the config
            AppendLineShow("\n+++ Keys and values in App.config +++");
            foreach (var key in keys)
            {
                var strings = keys.GetValues(key.ToString());
                if (strings == null) continue;

                var value1 = strings.GetValue(0);
                AppendLineShow(key + " - " + value1.ToString());
            }
        }

        internal void UploadFonts()
        {
            _path = "/Fonts";

            // Get fonts from local machine
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            FontFamily[] fontFamilies = installedFontCollection.Families;
            List<string> localFontList = fontFamilies.Select(t => t.Name).ToList();

            if (Zk.Exists(_path, false) == null)
            {
                try
                {
                    var createdPath = Zk.Create(_path, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                    AppendLineShow("Created: " + createdPath);
                }
                catch (KeeperException e)
                {
                    AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
                }
                catch (InvalidOperationException e)
                {
                    AppendLineShow(string.Format("Znode {0} isn't valid!", _input1));
                }
            }

            //Upload to zookeeper
            AppendLineShow("\n+++ Write in ZK +++");
            foreach (string font in localFontList)
            {
                _input1 = "Fonts/" + font;
                Create(false);
            }
        }

        internal void Test()
        {

            string[] files = Directory.GetFiles(@"C:\Windows\Fonts\", "*.ttf");

            foreach (string file in files)
            {

                GlyphTypeface ttf = new GlyphTypeface(new Uri(file));

                foreach (String x in ttf.FamilyNames.Values)
                {
                    Console.WriteLine(x);
                }

                Console.WriteLine(ttf.Version);

            }

            //string[] files = Directory.GetFiles(@"C:\Test\","*.ttf");

            ////////////////////////////////////////////////////////////////////////////

            //System.Drawing.FontFamily[] fontFamilies;
            //InstalledFontCollection installedFontCollection = new InstalledFontCollection();

            //// Get the array of FontFamily objects.
            //fontFamilies = installedFontCollection.Families;


            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\Documents\FontsOnMachine.txt"))
            //    foreach (System.Drawing.FontFamily font in fontFamilies)
            //    {
            //        file.WriteLine(font.Name);
            //    }

            ////////////////////////////////////////////////////////////////////////////

            //try
            //{

            //var bla = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            //AppendLineShow(bla);
            //AppendLineShow(ConfigurationManager.AppSettings["18127"]);

            //var keys = ConfigurationManager.AppSettings;

            //// Show the config
            //AppendLineShow("\n+++ Keys and values in App.config +++");

            //AppendLineShow(keys.GetKey(0));

            //foreach (var key in keys)
            //{
            //    var strings = keys.GetValues(key.ToString());
            //    if (strings == null) continue;

            //    var value1 = strings.GetValue(0);
            //    AppendLineShow(key + " - " + value1.ToString());
            //}
            //}
            //catch (Exception e)
            //{
            //    AppendLineShow(e.ToString());
            //}
        }

        /// <summary>
        /// Getters & Setters for _input1
        /// </summary>
        public string Input1
        {
            get { return _input1; }
            set { _input1 = value; }
        }

        /// <summary>
        /// Getters & Setters for _input2
        /// </summary>
        public string Input2
        {
            get { return _input2; }
            set { _input2 = value; }
        }
    }
}
