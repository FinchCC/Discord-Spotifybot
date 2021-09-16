using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spotify.Parser;
using Newtonsoft.Json;
using System.IO;

namespace MusicBot
{
    public class SpotifyParser
    {

        public static readonly int defaultsongs = 10;
        public static readonly string splitter = " - ";

        public static async Task<string[]> getSongNamesArr(string args)
        {
            string[] songarg = args.Split(' ');
            string albumid = songarg[0];

            if (albumid.Length > 25)
            {
                try
                {
                    albumid = albumid.Split('/', 5)[4].Split('?')[0];
                }
                catch (Exception)
                {

                    return new string[0];
                }
            }

            int songammount;
            if (args.Count() < 1)
                songammount = defaultsongs;
            else
            {
                try
                {
                    songammount = Convert.ToInt32(songarg[1]);
                }
                catch
                {
                    songammount = defaultsongs;
                }
            }
            var client = new RestClient($"https://unsa-unofficial-spotify-api.p.rapidapi.com/playlist?id={albumid}&start=0&limit={songammount}");

            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "unsa-unofficial-spotify-api.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", MISC.Tokens.GetSpotifyToken());

            IRestResponse response = client.Execute(request);


            string responseS = response.Content;
            List<string> names = new List<string>();
            if(response.IsSuccessful)
            {
                spotifyplaylist playlist = JsonConvert.DeserializeObject<spotifyplaylist>(responseS);
                for (int i = 0; i < playlist.Results.Count(); i++)
                    names.Add(playlist.Results[i].track.name + splitter + playlist.Results[i].track.artists[0].name);
            }

            File.WriteAllLines("C:\\Users\\askha\\source\\repos\\MusicBot\\MusicBot\\log.txt", names);
            return names.ToArray();
        }
    }
}
