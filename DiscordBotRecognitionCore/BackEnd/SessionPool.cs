using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognitionCore.BackEnd
{
    public class SessionPool
    {
        public Dictionary<ulong, PlaylistsSession> Sessions { get; private set; }


    }
}
