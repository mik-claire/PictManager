using Newtonsoft.Json;
using PictManager.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictManager.Config
{
    public class PmConf
    {
        public static Conf Config { get; set; }

        public static void LoadConfig()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var confPath = System.IO.Path.Combine(baseDir, "Config", "pm.conf");

            try
            {
                using (StreamReader sr = new StreamReader(confPath))
                {
                    string rte = sr.ReadToEnd().Replace("\\", "\\\\");
                    Config = JsonConvert.DeserializeObject<Conf>(rte);
                }
            }
            catch (Exception ex)
            {
                PmUtil.ShowError(ex);
            }
        }

        public class Conf
        {
            public string DirectoryPath;
            public string SaiProgramPath;
        }
    }
}
