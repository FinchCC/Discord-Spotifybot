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
            return "ODg0ODU0Njc1MzYxMzc4MzM0.YTejIg.5aWepCTTmsGkuBs7qiSh8HW3F8s";
        }

        private class Config
        {
            public string token { get; set; }
        }
    }
}
