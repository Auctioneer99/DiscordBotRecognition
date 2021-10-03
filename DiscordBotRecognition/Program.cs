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
using DiscordBotRecognition.Cache;
using DiscordBotRecognition.Songs;

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
            IMusicSearcher searcher = new YouTubeSearcher(Credential.GoogleAPIToken);
            CacheStorage CacheStorage = new CacheStorage();
            AudioService audio = new AudioService(CacheStorage);

            ServiceProvider provider = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(audio)
                .AddSingleton(searcher)
                .AddTransient<ISongStreamConverter>((service) => new FFmpegConverter())
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
