using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

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

        public static async Task<string> GetSpotifyToken()
        {
            await checkcfg();
            return await Task.FromResult(cfg.spotifytoken);
        }

        public static async Task checkcfg()
        {
            if (cfg == null)
            {
                string content;
                try
                {
                    content = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "botcfg.txt"));
                }
                catch (System.Exception)
                {
                    //just to make debugging easier, cant be bothered to use time on this
                    content = File.ReadAllText(Path.Combine("C:\\Users\\askha\\Documents\\TEMP", "botcfg.txt"));
                }
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
