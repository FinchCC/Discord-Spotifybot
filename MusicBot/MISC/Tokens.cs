using Newtonsoft.Json;
using System.IO;


namespace MusicBot.MISC
{
    public class Tokens
    {

        private static Config cfg;
        public static string GetToken()
        {
            checkcfg();
            return cfg.token;
        }

        public static string GetSpotifyToken()
        {
            checkcfg();
            return cfg.spotifytoken;
        }

        private static void checkcfg()
        {
            if (cfg == null)
            {
                var content = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "botcfg.txt"));
                cfg = JsonConvert.DeserializeObject<Config>(content);
            }
        }

        private class Config
        {
            public string token { get; set; }
            public string spotifytoken { get; set; }
        }
    }
}
