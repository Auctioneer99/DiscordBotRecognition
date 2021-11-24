using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public string DiscordIdentity { get; set; }

        public bool IsPrivate { get; set; }

        public List<Track> Tracks { get; set; }

        public static Playlist MapJSON(string json)
        {
            return JsonSerializer.Deserialize<Playlist>(json);
        }

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
