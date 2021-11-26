using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognitionCore.BackEnd.Models
{
    public abstract class APlaylist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public string DiscordIdentity { get; set; }

        public bool IsPrivate { get; set; }
    }
}
