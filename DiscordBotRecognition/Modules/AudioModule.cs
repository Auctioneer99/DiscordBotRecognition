using Discord;
using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.Song;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Modules.Audio
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _service;

        public AudioModule(AudioService service)
        {
            _service = service;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            await _service.JoinAudio(Context.Guild.Id, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("leave")]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild.Id);
            await ReplyAsync("```\nLeaving channel\n```");
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task AddSongCmd([Remainder] string query)
        {
            await JoinCmd();
            ISong song = await _service.AddSong(Context.Guild.Id, query);
            await ReplyAsync($"```\nSong added! {song.Name}, {song.Duration}\n```");
        }

        [Command("skip")]
        public async Task Skip(int songId = 1)
        {
            var song = await _service.SkipSong(Context.Guild.Id, songId);
            await ReplyAsync($"```\nSong skiped! {song.Name}\n```");
        }

        [Command("current")]
        public async Task GetCurrentSong()
        {
            ISong song = _service.GetCurrentSong(Context.Guild.Id);
            string output = "";
            if (song != null)
            {
                DateTime now = DateTime.Now;
                output = $"```\nCurrent song: {song}, {Math.Round((now - song.BeginPlay).TotalSeconds / song.Duration.TotalSeconds * 100, 2)} %\n```";
            }
            else
            {
                output = $"```\nNo song is playing\n```";
            }
            await ReplyAsync(output);
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
