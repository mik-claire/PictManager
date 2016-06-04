using PictManager.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictManager.Util
{
    public class PmUtil
    {
        public static readonly string STARTUP_PATH = System.Windows.Forms.Application.StartupPath;
        public static readonly string DB_PATH = Path.Combine(STARTUP_PATH, "Config", "PictData.db");
        public static readonly string CONNECTION_STRING = string.Format("Data Source={0};Version=3;", DB_PATH);

        public static void ShowError(Exception ex)
        {
            string message = string.Format("例外が発生しました。\n{0}", ex.Message);

#if DEBUG
            message += string.Format("\n" + ex.StackTrace);
#endif
            System.Windows.Forms.MessageBox.Show(message, "Error!!",
               System.Windows.Forms.MessageBoxButtons.OK,
               System.Windows.Forms.MessageBoxIcon.Error);
        }

        public static string GetExtention(string fileName)
        {
            string[] ary = fileName.Split('.');
            if (ary.Length < 1)
            {
                return string.Empty;
            }

            string ext = "." + ary[ary.Length - 1];

            return ext;
        }

        public static string RemoveExtention(string fileName)
        {
            string ext = GetExtention(fileName);
            string withoutExt = fileName.Substring(0, fileName.Length - ext.Length);

            return withoutExt;
        }

        public static string GetParentDirectoryName(string filePath)
        {
            string[] ary = filePath.Split('\\');
            if (ary.Length < 2)
            {
                return string.Empty;
            }

            return ary[ary.Length - 2];
        }

        public static string GetPictureDirectory(string FilePath)
        {
            string[] ary = FilePath.Split('\\');
            if (ary.Length < 2)
            {
                return string.Empty;
            }

            string[] pictureDirectoryAry = PmConf.Config.DirectoryPath.Split('\\');
            string dir = string.Empty;
            for (int i = pictureDirectoryAry.Length; i < ary.Length - 1; i++)
            {
                dir = Path.Combine(dir, ary[i]);
            }

            return dir;
        }
    }
}
