using PictManager.Config;
using PictManager.Model;
using PictManager.Util;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;

namespace PictManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void load()
        {
            var dbData = loadDB(PmUtil.DB_PATH);

            if (!Directory.Exists(PmConf.Config.DirectoryPath))
            {
                Directory.CreateDirectory(PmConf.Config.DirectoryPath);
            }

            List<PictureInfo> infoList = new List<PictureInfo>();

            DirectoryInfo di = new DirectoryInfo(PmConf.Config.DirectoryPath);
            foreach (var fi in di.GetFiles())
            {
                if (!File.Exists(fi.FullName))
                {
                    continue;
                }
                if (fi.Extension.ToUpper() != ".PNG" &&
                    fi.Extension.ToUpper() != ".JPG" &&
                    fi.Extension.ToUpper() != ".SAI")
                {
                    continue;
                }

                bool isNewPicture = true;
                foreach (var rec in dbData)
                {
                    if (rec.FileName != fi.Name)
                    {
                        continue;
                    }

                    isNewPicture = false;
                    break;
                }
                if (!isNewPicture)
                {
                    continue;
                }

                // load
                PictureInfo pi = new PictureInfo();
                pi.FileName = fi.Name;
                pi.DisplayName = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                pi.IsNewPicture = true;
                infoList.Add(pi);
            }

            this.DataContext = infoList;
        }

        private List<PictureInfo> loadDB(string filePath)
        {
            List<PictureInfo> infoList = new List<PictureInfo>();

            SQLiteConnection cn = null;
            SQLiteCommand cmd = null;
            SQLiteDataReader reader = null;

            try
            {
                cn = new SQLiteConnection(PmUtil.CONNECTION_STRING);
                cn.Open();

                cmd = new SQLiteCommand(string.Format(
@"select * from data", ""),
                    cn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PictureInfo pi = new PictureInfo();
                    pi.Id = int.Parse(reader["id"].ToString());
                    pi.FileName = reader["fileName"].ToString();
                    pi.DisplayName = reader["displayName"].ToString();
                    string[] tags = reader["tags"].ToString().Split(',');
                    pi.Tags = tags.ToList();
                    pi.IsNewPicture = false;

                    infoList.Add(pi);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                if (cmd != null)
                {
                    cmd.Dispose();
                }

                if (cn != null)
                {
                    cn.Close();
                }
            }

            return infoList;
        }

        #region Event Handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PmConf.LoadConfig();

                load();
            }
            catch (Exception ex)
            {
                PmUtil.ShowError(ex);
            }

        }

        #endregion
    }
}
