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
    }
}
