using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotRecognition.Alive;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.Cache;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Modules;
using DiscordBotRecognition.MusicSearch;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DiscordBotRecognition
{
    public class DiscordBot
    {
        private DiscordSocketClient _client;

        public DiscordBot(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task<bool> Start(string botToken)
        {
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            return true;
        }

        public async Task<bool> Stop()
        {
            await _client.StopAsync();
            return true;
        }

        public static async Task<DiscordBot> DefaultBuild(string googleToken)
        {
            DiscordSocketClient client = new DiscordSocketClient();
            CommandService commands = new CommandService();
            IMusicSearcher searcher = new YouTubeSearcher(googleToken);
            CacheStorage cacheStorage = new CacheStorage();
            AudioService audio = new AudioService(cacheStorage);

            ServiceProvider provider = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(audio)
                .AddSingleton(searcher)
                .AddTransient<ISongStreamConverter>((service) => new FFmpegConverter(new AliveChecker()))
                .BuildServiceProvider();

            CommandHandler handler = new CommandHandler(client, commands, provider);
            await handler.InitializeAsync();

            return new DiscordBot(client);
        }
    }
}
