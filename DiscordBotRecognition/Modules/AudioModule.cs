using Discord;
using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.AudioPlayer.Queue;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Converter.Settings;
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

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Connecting bot to active voice channel")]
        public async Task JoinCmd()
        {
            if (_service.IsConnected(Context.Guild.Id) == false)
            {
                var audioClient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                IAudioClient discordClient = new DiscordAudioClient(audioClient);
                if (await _service.TryJoinAudio(Context.Guild.Id, discordClient, _converter))
                {
                    await ReplyAsync("```\nConnected\n```");
                }
                else
                {
                    await discordClient.DisposeAsync();
                    await ReplyAsync("```\nCan't connect to server\n```");
                }
            }
        }

        [Command("leave")]
        [Summary("Leaving active voice channel in this channel")]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild.Id);
            await ReplyAsync("```\nLeaving channel\n```");
        }

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Adding song to queue or resuming/playing")]
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
        [Summary("Pausing current voice output")]
        public async Task Pause()
        {
            _service.PauseSong(Context.Guild.Id);
            await ReplyAsync("```\nSong paused!\n```");
        }

        [Command("skip")]
        [Summary("Skipping current song")]
        public async Task Skip()
        {
            var song = _service.SkipSong(Context.Guild.Id);
            await ReplyAsync($"```\nSong skiped! {song.Name}\n```");
        }

        [Command("remove")]
        [Summary("Removing song in a queue")]
        public async Task Remove(int songId = 1)
        {
            var song = _service.RemoveSong(Context.Guild.Id, songId - 1);
            await ReplyAsync($"```\nSong removed! {song.Name}\n```");
        }

        [Command("resume", RunMode = RunMode.Async)]
        [Summary("Resuming voice output")]
        public async Task Resume()
        {
            if (_service.IsPaused(Context.Guild.Id))
            {
                await ReplyAsync("```\nResuming!\n```");
                await _service.Play(Context.Guild.Id, true);
            }
        }

        [Command("current")]
        [Summary("Show information about playing song")]
        public async Task GetCurrentSong()
        {
            ISong song = _service.GetCurrentSong(Context.Guild.Id);
            string output;
            if (song != null)
            {
                output = $"```\nCurrent song: {song}\n```";
            }
            else
            {
                output = $"```\nNo song is playing\n```";
            }
            await ReplyAsync(output);
        }

        [Command("queue")]
        [Summary("Shows song queue")]
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

        [Command("loop")]
        [Summary("Loops queue, toggle = on/off")]
        public async Task SetLooping(string toggle)
        {
            switch(toggle)
            {
                case "on":
                    await ReplyAsync("```\nRepeating queue!\n```");
                    _service.SetLooping(Context.Guild.Id, EQueueType.Loop);
                    return;
                case "off":
                    await ReplyAsync("```\nQueue sets to fifo!\n```");
                    _service.SetLooping(Context.Guild.Id, EQueueType.FIFO);
                    return;
                default:
                    return;
            }
        }

        [Command("stop")]
        [Summary("Clears queue")]
        public async Task Stop()
        {
            _service.Stop(Context.Guild.Id);
        }

        [Command("bass")]
        [Summary("Setting bass option")]
        public async Task SetBass(int volume)
        {
            _service.SetBass(volume, Context.Guild.Id);
        }

        [Command("speed")]
        [Summary("Setting speed option, volume = slow/nightcore/normal")]
        public async Task SetSpeed(string volume)
        {
            Speed speed;
            switch (volume)
            {
                case "slow":
                    speed = Speed.Slowed();
                    break;
                case "nightcore":
                case "nc":
                    speed = Speed.Nightcore();
                    break;
                case "normal":
                default:
                    speed = Speed.Normal();
                    break;
            }
            _service.SetSpeed(speed, Context.Guild.Id);
        }

        [Command("info")]
        [Summary("Get info about audio group")]
        public async Task GetInfo()
        {
            var info = _service.GetInfo(Context.Guild.Id);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine($"QUEUE SETTINGS:");
            sb.AppendLine($"QueueType = {info.QueueType.ToString()}");
            sb.AppendLine($"CONVERT SETTINGS:");
            sb.AppendLine($"Bass = {info.ConvertInfo.Bass}");
            sb.AppendLine($"Treble = {info.ConvertInfo.Treble}");
            sb.AppendLine("```");
            await ReplyAsync(sb.ToString());
        }
    }
}
