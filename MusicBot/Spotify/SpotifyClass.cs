using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Parser
{
    public class spotifyplaylist
    {
        public string PlaylistId { get; set; }

        public result[] Results { get; set; }
    }

    public class result
    {
        public string added_at { get; set; }
        public addedby added_by { get; set; }
        public bool is_local { get; set; }

        public Track track { get; set; }

        public class Track
        {
            public string name { get; set; }
            public Artists[] artists { get; set; }

            public class Artists
            {
                public string name { get; set; }
            }
        }
    }

    public class addedby
    {
        public externalurls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }

    }

    public class externalurls
    {

    }
}
