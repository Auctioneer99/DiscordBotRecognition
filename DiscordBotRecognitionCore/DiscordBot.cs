using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.Cache;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Modules;
using DiscordBotRecognition.MusicSearch;
using DiscordBotRecognitionCore.Alive;
using DiscordBotRecognitionCore.Connection;
using DiscordBotRecognitionCore.Converter;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DiscordBotRecognition
{
    public class DiscordBot
    {
        private DiscordSocketClient _client;
        private AliveChecker _aliveChecker;

        private DiscordBot(AliveChecker checker, DiscordSocketClient client)
        {
            _aliveChecker = checker;
            _client = client;
        }

        public async Task<bool> Start(string botToken)
        {
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            _aliveChecker.Start();
            return true;
        }

        public async Task<bool> Stop()
        {
            _aliveChecker.Stop();
            await _client.StopAsync();
            return true;
        }

        public static async Task<DiscordBot> DefaultBuild(string googleToken)
        {
            DiscordSocketClient client = new DiscordSocketClient();
            CommandService commands = new CommandService();
            IMusicSearcher searcher = new YouTubeSearcher(googleToken);
            ConnectionPool connectionPool = new ConnectionPool();
            var aliveChecker = new AliveChecker(connectionPool);
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var filePath = isWindows ?
                Path.Join(Directory.GetCurrentDirectory(), "ffmpeg.exe") :
                "/usr/bin/ffmpeg";
            IConverterFactory converterFactory = new FFmpegConverterFactory(filePath);

            DiscordAudioConnector connector = new DiscordAudioConnector()
            {
                FactoryConverter = converterFactory,
                ConnectionPool = connectionPool
            };
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton(connector)
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(connectionPool)
                .AddSingleton(searcher)
                .BuildServiceProvider();

            CommandHandler handler = new CommandHandler(client, commands, provider);
            await handler.InitializeAsync();

            return new DiscordBot(aliveChecker, client);
        }
    }
}
