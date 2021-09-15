using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBotRecognition.MusicSearch;
using DiscordBotRecognition.Modules;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.Credentials;
using DiscordBotRecognition.Converter;

namespace DiscordBotRecognition
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            DiscordSocketClient client = new DiscordSocketClient();
            CommandService commands = new CommandService();
            YouTubeSearcher searcher = new YouTubeSearcher(Credential.GoogleAPIToken); //new SoundCloudSearcher();
            //searcher.Initialize(Credential.SoundCloudToken);// new YouTubeSearcher(Credential.GoogleAPIToken);
            ISongStreamConverter converter = new NAudioConverter();
            AudioService audio = new AudioService(searcher, converter);

            ServiceProvider provider = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(audio)
                .BuildServiceProvider();

            CommandHandler handler = new CommandHandler(client, commands, provider);
            await handler.InitializeAsync();

            string token = Credential.DiscordToken;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
