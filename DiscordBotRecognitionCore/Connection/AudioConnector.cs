using Discord;
using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.Recognition;
using DiscordBotRecognitionCore.Converter;
using DiscordBotRecognitionCore.Recognition.Recognizers;
using DiscordBotRecognitionCore.Synthesier;
using System;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Connection
{
    public interface IAudioConnector
    {
        Task<AudioGroup> TryConnect(ulong id);
    }

    public class DiscordAudioConnector : IAudioConnector
    {
        public ConnectionPool ConnectionPool { get; set; }
        public IConverterFactory FactoryConverter { get; set; }
        public IRecognizerFactory FactoryRecognizer { get; set; }

        public SocketCommandContext Context { get; set; }

        private string _greetings;

        public DiscordAudioConnector(string greetingsText)
        {
            _greetings = greetingsText;
        }

        public async Task<AudioGroup> TryConnect(ulong id)
        {
            if (ConnectionPool.IsConnected(id) == false)
            {
                var audioClient = await(Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                IAudioClient discordClient = new DiscordAudioClient(id, audioClient, FactoryRecognizer);
                var group = new AudioGroup(discordClient, FactoryConverter.Get(), new DiscordSynthesier(discordClient), AudioGroupSettings.Default());
                if (await ConnectionPool.TryJoin(id, group))
                {
                    await group.Synthesier.Speak(_greetings);
                    return group;
                }
                else
                {
                    await group.DisposeAsync();
                    return null;
                }
            }
            return null;
        }
    }
}
