using PictManager.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PictManager
{
    public class PictureInfo
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public List<string> Tags { get; set; }

        public bool IsNewPicture { get; set; }

        public PictureInfo()
        {
            this.Tags = new List<string>();
        }

        public string FilePath
        {
            get
            {
                string filePath = Path.Combine(PmConf.Config.DirectoryPath, this.FileName);
                return filePath;
            }
        }
    }
}
