using Discord;
using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognitionCore.Converter;
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

        public SocketCommandContext Context { get; set; }

        public async Task<AudioGroup> TryConnect(ulong id)
        {
            if (ConnectionPool.IsConnected(id) == false)
            {
                var audioClient = await(Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                IAudioClient discordClient = new DiscordAudioClient(id, audioClient);
                var group = new AudioGroup(discordClient, FactoryConverter.Get(), AudioGroupSettings.Default());
                if (await ConnectionPool.TryJoin(id, group))
                {
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
