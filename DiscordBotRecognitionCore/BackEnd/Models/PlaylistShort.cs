using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public class PlaylistShort : APlaylist
    {
        public int TracksCount { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Name = {Name}, ");
            sb.Append($"User = {DiscordIdentity}, ");
            sb.Append($"Count = {TracksCount}");
            return sb.ToString();
        }
    }
}
