using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Modules
{
    public class PingPong : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public Task Ping() => ReplyAsync("Pong!");
    }
}
