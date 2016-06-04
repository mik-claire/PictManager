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
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public string Directory { get; set; }
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
                string filePath = Path.Combine(PmConf.Config.DirectoryPath, this.Directory == null ? string.Empty : this.Directory, this.FileName);
                return filePath;
            }
        }

        public Uri ImageUri
        {
            get
            {
                Uri uri = new Uri(this.FilePath);
                return uri;
            }
        }

        public BitmapSource Image
        {
            get
            {
                using (Stream stream = new FileStream(
                    this.FilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete
                ))
                {
                    // ロックしないように指定したstreamを使用する。
                    BitmapDecoder decoder = BitmapDecoder.Create(
                        stream,
                        BitmapCreateOptions.None, // この辺のオプションは適宜
                        BitmapCacheOption.Default // これも
                    );
                    BitmapSource bmp = new WriteableBitmap(decoder.Frames[0]);
                    bmp.Freeze();

                    // xamlでImageを記述 → imgSync
                    return bmp;
                }
            }
        }
    }
}
