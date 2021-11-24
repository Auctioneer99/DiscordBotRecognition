using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public class Track
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Label { get; set; }

        public MusicService MusicService { get; set; }
    }
}
