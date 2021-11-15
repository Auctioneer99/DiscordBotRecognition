using Discord;
using Discord.Audio;
using DiscordBotRecognitionCore.Connection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DiscordBotRecognition.Recognition
{
    public class RecognitionService
    {
        public ConnectionPool ConnectionPool { get; private set; }

        public void StartListen(ulong id, IVoiceChannel channel)
        {
            if (ConnectionPool.TryGetConnection(id, out var junk))
            {
                return;
            }
            if (channel.Guild.Id != id)
            {
                return;
            }
            //IAudioClient c;
            //c.GetStreams();


            IRecognizer recognizer = new Recognizer();
           // RecognitionGroup group = new RecognitionGroup(recognizer);
           // ConnectedChannels.TryAdd(id, group);
        }

    }
}
