using DiscordBotRecognition.AudioPlayer.Queue;
using DiscordBotRecognition.Converter.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioGroupInfo
    {
        public EQueueType QueueType { get; set; }

        public ConvertSettings ConvertInfo { get; set; }
    }
}
