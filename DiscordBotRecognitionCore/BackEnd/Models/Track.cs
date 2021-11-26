using DiscordBotRecognition.Songs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public class Track
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Label { get; set; }

        public MusicService MusicService { get; set; }

        public async Task<ISong> Convert()
        {
            switch(MusicService.Name)
            {
                case "YouTube":
                    var res = new YouTubeSong(Url);
                    await res.Initialize();
                    return res;
                    break;
            }
            return null;
        }
    }
}
