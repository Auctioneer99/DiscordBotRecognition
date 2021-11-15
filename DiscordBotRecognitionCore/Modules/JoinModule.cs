using Discord.Commands;
using DiscordBotRecognitionCore.Connection;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Modules
{
    public class JoinModule : AModuleBase
    {
        public DiscordAudioConnector AudioConnector { get; set; }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Connecting bot to active voice channel")]
        public async Task JoinCmd()
        {
            AudioConnector.Context = Context;
            var group = await AudioConnector.TryConnect(Id);
            if (group != null)
            {
                await ReplyAsync("```\nConnected\n```");
            }
            else
            {
                await ReplyAsync("```\nCan't connect to server\n```");
            }
            /*
            if (ConnectionPool.IsConnected(Id) == false)
            {
                var audioClient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                IAudioClient discordClient = new DiscordAudioClient(Id, audioClient);
                var group = new AudioGroup(discordClient, FactoryConverter.Get(), AudioGroupSettings.Default());
                if (await ConnectionPool.TryJoin(Context.Guild.Id, group))
                {
                }
                else
                {
                    await group.DisposeAsync();
                }
            }*/
        }

        [Command("leave")]
        [Summary("Leaving active voice channel in this channel")]
        public async Task LeaveCmd()
        {
            if (ConnectionPool.Leave(Id))
            {
                await ReplyAsync("```\nLeaving channel\n```");
            }
        }
    }
}
