using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public class Playlist : APlaylist
    {
        public List<Track> Tracks { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Name = {Name}, ");
            sb.Append($"User = {DiscordIdentity}, ");
            sb.Append($"Count = {Tracks.Count}");
            return sb.ToString();
        }
    }
}
