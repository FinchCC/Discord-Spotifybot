using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MusicBot
{
    public class Values
    {
        public static string GetToken()
        {
            var content = File.ReadAllText("C:\\Users\\askha\\source\\repos\\MusicBot\\MusicBot\\botcfg.txt");
            var cfg = JsonConvert.DeserializeObject<Config>(content);

            return cfg.token;
        }


        public static string getTokenTemp()
        {
            return "";
        }

        private class Config
        {
            public string token { get; set; }
        }
    }
}
