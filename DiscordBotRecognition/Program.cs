using System.Threading.Tasks;
using DiscordBotRecognition.Credentials;

namespace DiscordBotRecognition
{
    internal class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var bot = await DiscordBot.DefaultBuild(Credential.GoogleAPIToken);
            await bot.Start(Credential.DiscordToken);
            await Task.Delay(-1);
        }
    }
}
