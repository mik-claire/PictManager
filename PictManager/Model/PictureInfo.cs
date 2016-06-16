using PictManager.Config;
using PictManager.Controls;
using PictManager.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PictManager.Model
{
    public class PictureInfo
    {
        public PictureInfo()
        {
            this.Tags = new List<string>();
        }

        #region DB Data
        
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public string Directory { get; set; }
        public List<string> Tags { get; set; }
        
        #endregion

        public DateTime Modified { get; set; }

        public bool IsNewPicture { get; set; }

        public string FilePath
        {
            get
            {
                string filePath = Path.Combine(
                    PmConf.Config.DirectoryPath,
                    this.Directory == null ? string.Empty : this.Directory, this.FileName);

                return filePath;
            }
        }

        public string Extention
        {
            get
            {
                string ext = PmUtil.GetExtention(this.FilePath);
                return ext;
            }
        }

        public BitmapSource Image
        {
            get
            {
                using (Stream stream = new FileStream(this.FilePath, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(stream,
                        BitmapCreateOptions.None,
                        BitmapCacheOption.Default);

                    BitmapSource bmp = new WriteableBitmap(decoder.Frames[0]);
                    bmp.Freeze();

                    return bmp;
                }
            }
        }
    }
}
