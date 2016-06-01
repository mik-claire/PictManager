using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictManager.Util
{
    public class PmUtil
    {
        public static void ShowError(Exception ex)
        {
            string message = string.Format("Excelファイルへのアクセスに失敗しました。\n{0}", ex.Message);

#if DEBUG
            message += string.Format("\n" + ex.StackTrace);
#endif
            System.Windows.Forms.MessageBox.Show(message, "Error!!",
               System.Windows.Forms.MessageBoxButtons.OK,
               System.Windows.Forms.MessageBoxIcon.Error);
        }
    }
}
