using Discord.Commands;
using DiscordBotRecognitionCore.Connection;
using System.Linq;
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
            var group = await Connect();
            if (group != null)
            {
                await ReplyAsync("```\nConnected\n```");
            }
            else
            {
                await ReplyAsync("```\nCan't connect to server\n```");
            }
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
