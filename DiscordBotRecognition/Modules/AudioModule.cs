using Discord;
using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.MusicSearch;
using DiscordBotRecognition.Song;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Modules.Audio
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _service;
        private readonly ISongStreamConverter _converter;
        private readonly IMusicSearcher _searcher;

        public AudioModule(AudioService service, ISongStreamConverter converter, IMusicSearcher searcher)
        {
            _service = service;
            _converter = converter;
            _searcher = searcher;
        }

        [Command("join")]
        public async Task JoinCmd()
        {
            if (_service.IsConnected(Context.Guild.Id) == false)
            {
                var audioClient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                IAudioClient discordClient = new DiscordAudioClient(audioClient);
                if (await _service.TryJoinAudio(Context.Guild.Id, discordClient, _converter))
                {

                }
                else
                {
                    await discordClient.DisposeAsync();
                    await ReplyAsync("```\nCan't connect to server\n```");
                }
            }
        }

        [Command("leave")]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild.Id);
            await ReplyAsync("```\nLeaving channel\n```");
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task AddSong([Remainder] string query = "")
        {
            if (query == "")
            {
                await Resume();
            }
            else
            {
                await JoinCmd();
                ISong song = await _searcher.SearchSong(query);
                if (song == null)
                {
                    await ReplyAsync($"```\nSong not found!\n```");
                }
                else
                {
                    await ReplyAsync($"```\nSong added! {song.Name}, {song.Duration}\n```");
                    await _service.AddSong(Context.Guild.Id, song);
                }
            }
        }

        [Command("pause")]
        public async Task Pause()
        {
            _service.PauseSong(Context.Guild.Id);
            await ReplyAsync("```\nSong paused!\n```");
        }

        [Command("skip")]
        public async Task Skip(int songId = 1)
        {
            var song = _service.SkipSong(Context.Guild.Id, songId - 1);
            await ReplyAsync($"```\nSong skiped! {song.Name}\n```");
        }

        [Command("resume", RunMode = RunMode.Async)]
        public async Task Resume()
        {
            if (_service.IsPaused(Context.Guild.Id))
            {
                await ReplyAsync("```\nResuming!\n```");
                await _service.Play(Context.Guild.Id, true);
            }
        }

        [Command("current")]
        public async Task GetCurrentSong()
        {
            Console.WriteLine(0);
            ISong song = _service.GetCurrentSong(Context.Guild.Id);
            Console.WriteLine(1);
            string output;
            Console.WriteLine(2);
            if (song != null)
            {
                Console.WriteLine(3);
                output = $"```\nCurrent song: {song}\n```";
            }
            else
            {
                Console.WriteLine(4);
                output = $"```\nNo song is playing\n```";
            }
            Console.WriteLine(5);
            await ReplyAsync(output);
            Console.WriteLine(6);
        }

        [Command("queue")]
        public async Task GetListCmd()
        {
            var songs = _service.GetSongList(Context.Guild.Id);
            if (songs.Count == 0)
            {
                await ReplyAsync("```\nThere is no songs!\n```");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                int i = 1;
                sb.AppendLine("```");
                foreach (var song in songs)
                {
                    sb.AppendLine($"{i++}) {song}");
                }
                sb.AppendLine("```");
                await ReplyAsync(sb.ToString());
            }
        }
    }
}
