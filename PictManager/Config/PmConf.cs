using Newtonsoft.Json;
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

            using (StreamReader sr = new StreamReader(confPath))
            {
                string rte = sr.ReadToEnd().Replace("\\", "\\\\");
                Config = JsonConvert.DeserializeObject<Conf>(rte);
            }
        }

        public class Conf
        {
            public string DirectoryPath;
        }
    }
}
