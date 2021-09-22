using Discord;
using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognition.Recognition
{
    public class RecognitionService
    {
        private readonly ConcurrentDictionary<ulong, RecognitionGroup> ConnectedChannels = new ConcurrentDictionary<ulong, RecognitionGroup>();

        public void StartListen(ulong id, IVoiceChannel channel)
        {
            if (ConnectedChannels.TryGetValue(id, out var junk))
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
            RecognitionGroup group = new RecognitionGroup(recognizer);
            ConnectedChannels.TryAdd(id, group);
        }

    }
}
