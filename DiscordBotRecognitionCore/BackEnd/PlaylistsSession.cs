using DiscordBotRecognitionCore.BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognitionCore.BackEnd
{
    public class PlaylistsSession
    {
        public TimeSpan RequestedAt { get; set; }
        public ulong Requester { get; set; }
        public List<Playlist> Playlists { get; set; }
    }
}
