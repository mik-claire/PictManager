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
using PictManager.View;
using System.Diagnostics;

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
            var dbData = loadDB(PmUtil.DB_PATH, string.Empty);

            if (!Directory.Exists(PmConf.Config.DirectoryPath))
            {
                Directory.CreateDirectory(PmConf.Config.DirectoryPath);
            }

            List<PictureInfo> infoList = new List<PictureInfo>();

            DirectoryInfo di = new DirectoryInfo(PmConf.Config.DirectoryPath);
            load2(di, infoList, dbData);

            insert(infoList);
            this.DataContext = infoList;
        }

        private void load2(DirectoryInfo di, List<PictureInfo> infoList, List<PictureInfo> dbData)
        {
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                load2(subDi, infoList, dbData);
            }

            foreach (FileInfo fi in di.GetFiles())
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
                PictureInfo hitRec = new PictureInfo();
                foreach (var rec in dbData)
                {
                    if (rec.FileName != fi.Name ||
                        rec.Directory != fi.FullName.Substring(PmConf.Config.DirectoryPath.Length + 1, fi.FullName.Length - PmConf.Config.DirectoryPath.Length - fi.Name.Length - 1))
                    {
                        continue;
                    }

                    isNewPicture = false;
                    hitRec = rec;
                    break;
                }
                if (!isNewPicture)
                {
                    infoList.Add(hitRec);
                    continue;
                }

                // load
                PictureInfo pi = new PictureInfo();
                pi.FileName = fi.Name;
                pi.DisplayName = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                pi.Directory = fi.FullName.Substring(PmConf.Config.DirectoryPath.Length + 1, fi.FullName.Length - PmConf.Config.DirectoryPath.Length - fi.Name.Length - 1);
                pi.IsNewPicture = true;
                infoList.Add(pi);
            }
        }

        private void search(string[] keywords)
        {
            if (keywords.Length == 1 &&
                string.IsNullOrEmpty(keywords[0]))
            {
                // Reload
                load();
                return;
            }

            string cond = string.Empty;
            string column = string.Empty;
            if ((bool)this.radioButton_FileName.IsChecked)
            {
                column = "fileName";
            }
            else if ((bool)this.radioButton_DisplayName.IsChecked)
            {
                column = "displayName";
            }
            else // if ((bool)this.radioButton_Tags.IsCheched)
            {
                column = "tags";
            }

            foreach (string word in keywords)
            {
                if (cond != string.Empty)
                {
                    cond += " or ";
                }
                cond = string.Format("{0} like '%{1}%'", column, word);
            }

            var dbData = loadDB(PmUtil.DB_PATH, cond);

            if (!Directory.Exists(PmConf.Config.DirectoryPath))
            {
                Directory.CreateDirectory(PmConf.Config.DirectoryPath);
            }

            this.DataContext = dbData;
        }

        private void opnSai(string saiPath, string openFilePath)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = saiPath;
            info.Arguments = openFilePath;

            using (Process p = new Process())
            {
                p.StartInfo = info;
                p.Start();
            }
        }

        #region Event Handler

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// [ Rename ]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Rename_Click(object sender, RoutedEventArgs e)
        {
            PictureInfo pi = this.listView_Picts.SelectedItem as PictureInfo;
            if (pi == null)
            {
                return;
            }

            string newDisplayName = this.textBox_FileName.Text.Trim();
            string newFileName = newDisplayName + PmUtil.GetExtention(pi.FileName);

            string message = @"Rename '{0}' -> '{1}' ?";
            message = string.Format(message,
                pi.DisplayName,
                newDisplayName);

            MessageBoxResult mbr = MessageBox.Show(message,
                "Confirm.",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (mbr != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                string filePathDest = System.IO.Path.Combine(PmConf.Config.DirectoryPath, pi.Directory, newFileName);
                File.Move(pi.FilePath, filePathDest);
                updateFileName(pi.Id, newFileName, newDisplayName);
            }
            catch (Exception ex)
            {
                PmUtil.ShowError(ex);
            }

            MessageBox.Show("Renamed.",
                "Information.",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            string[] keywords = this.textBox_Search.Text.Trim().Split(' ');
            search(keywords);
        }

        /// <summary>
        /// [ Tag Set ]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_TagSet_Click(object sender, RoutedEventArgs e)
        {
            var piObj = this.listView_Picts.SelectedItem;
            PictureInfo pi = piObj as PictureInfo;
            if (pi == null)
            {
                return;
            }

            List<string> tags = this.textBox_Tags.Text.Trim().Split(' ').ToList();
            pi.Tags = tags;

            updateTags(pi.Id, string.Join(" ", tags));

            string[] keywords = this.textBox_Search.Text.Trim().Split(' ');
            search(keywords);
        }

        /// <summary>
        /// [ Search ]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Search_Click(object sender, RoutedEventArgs e)
        {
            string[] keywords = this.textBox_Search.Text.Trim().Split(' ');
            search(keywords);
        }

        /// <summary>
        /// ListView selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_Picts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var piObj = this.listView_Picts.SelectedItem;
            PictureInfo pi = piObj as PictureInfo;
            if (pi == null)
            {
                return;
            }

            string fileName = PmUtil.RemoveExtention(pi.FileName);
            this.textBox_FileName.Text = fileName;

            string tags = string.Join(" ", pi.Tags);
            this.textBox_Tags.Text = tags;
        }

		private void textBox_Search_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				button_Search_Click(new object(), new RoutedEventArgs());
				return;
			}
		}

        private void textBox_FileName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // button_Rename_Click(new object(), new RoutedEventArgs());
                return;
            }
        }

		private void textBox_Tags_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				button_TagSet_Click(new object(), new RoutedEventArgs());
				return;
			}
		}

        private void listView_Picts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.listView_Picts.SelectedItems.Count != 1)
            {
                return;
            }

            PictureInfo pi = this.listView_Picts.SelectedItem as PictureInfo;
            if (pi == null)
            {
                return;
            }

            string ext = pi.FileName.Substring(pi.FileName.Length - 4, 4).ToUpper();
            if (ext == ".SAI" &&
                !string.IsNullOrEmpty(PmConf.Config.SaiProgramPath))
            {
                // Open sai
                opnSai(PmConf.Config.SaiProgramPath, pi.FilePath);
                return;
            }

            PreviewWindow w = new PreviewWindow(pi);
            w.Show();
        }

        #endregion

        #region DB Control

        private List<PictureInfo> loadDB(string filePath, string cond)
        {
            string sql = @"
select
  *
from
  data";

            List<PictureInfo> infoList = new List<PictureInfo>();

            SQLiteConnection cn = null;
            SQLiteCommand cmd = null;
            SQLiteDataReader reader = null;

            try
            {
                cn = new SQLiteConnection(PmUtil.CONNECTION_STRING);
                cn.Open();

                if (cond != string.Empty)
                {
                    sql += @"
where
  {0}
";
                    sql = string.Format(sql,
                        cond);
                }

                cmd = new SQLiteCommand(sql + ";", cn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PictureInfo pi = new PictureInfo();
                    pi.Id = int.Parse(reader["id"].ToString());
                    pi.FileName = reader["fileName"].ToString();
                    pi.Directory = reader["directory"].ToString();
                    pi.DisplayName = reader["displayName"].ToString();
                    string[] tags = reader["tags"].ToString().Split(' ');
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

        private void insert(List<PictureInfo> piList)
        {
            SQLiteConnection cn = null;
            SQLiteCommand cmd = null;

            try
            {
                cn = new SQLiteConnection(PmUtil.CONNECTION_STRING);
                cn.Open();

                foreach (PictureInfo pi in piList)
                {
                    if (!pi.IsNewPicture)
                    {
                        continue;
                    }

                    string sql = @"
insert into data (
  fileName,
  displayName,
  directory,
  tags
)
values (
  '{0}',
  '{1}',
  '{2}',
  '{3}'
);";

                    string tags = string.Join(" ", pi.Tags);
                    sql = string.Format(sql,
                        pi.FileName,
                        pi.DisplayName,
                        pi.Directory,
                        tags);

                    cmd = new SQLiteCommand(sql, cn);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }

                if (cn != null)
                {
                    cn.Close();
                }
            }
        }

        private void updateFileName(int index, string fileName, string displayName)
        {
            string sql = @"
update data
set
  fileName = '{0}',  
  displayName = '{1}'
where id = {2};";

            SQLiteConnection cn = null;
            SQLiteCommand cmd = null;

            try
            {
                cn = new SQLiteConnection(PmUtil.CONNECTION_STRING);
                cn.Open();

                sql = string.Format(sql,
                    fileName,
                    displayName,
                    index);
                cmd = new SQLiteCommand(sql, cn);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }

                if (cn != null)
                {
                    cn.Close();
                }
            }
        }

        private void updateTags(int index, string tags)
        {
            string sql =@"
update data
  set tags = '{0}'
where id = {1};";

            SQLiteConnection cn = null;
            SQLiteCommand cmd = null;

            try
            {
                cn = new SQLiteConnection(PmUtil.CONNECTION_STRING);
                cn.Open();

                sql = string.Format(sql,
                    tags,
                    index);
                cmd = new SQLiteCommand(sql, cn);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }

                if (cn != null)
                {
                    cn.Close();
                }
            }
        }

        #endregion

    }
}
