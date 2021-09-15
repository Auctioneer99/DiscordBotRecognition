using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioGroupSettings
    {
        public int MaxQueueSize { get; set; }

        public static AudioGroupSettings Default()
        {
            return new AudioGroupSettings()
            {
                MaxQueueSize = 10,
            };
        }
    }
}
