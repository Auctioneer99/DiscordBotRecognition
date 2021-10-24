using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        public CommandService Service { get; set; }

        [Command("ping")]
        [Summary("Sends o pong signal")]
        public Task Ping() => ReplyAsync("Pong!");

        [Command("help")]
        [Summary("Show information about commands")]
        public async Task Help()
        {
            var output = string.Join("\n", Service
                .Commands
                .Select(info => $"{info.Name} {string.Join(", ", info.Parameters.Select(parameter => $"[{parameter.Name}]"))} - {info.Summary}"));
            await ReplyAsync($"```{output}```");
        }
    }
}
