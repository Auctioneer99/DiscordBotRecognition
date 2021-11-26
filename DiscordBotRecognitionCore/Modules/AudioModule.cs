using Discord;
using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.MusicSearch;
using DiscordBotRecognition.Songs;
using DiscordBotRecognitionCore.Connection;
using DiscordBotRecognitionCore.Converter;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Modules
{
    public class AudioModule : AModuleBase
    {
        public IMusicSearcher Searcher { get; set; }

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
                ISong song = await Searcher.SearchSong(query);
                if (song == null)
                {
                    await ReplyAsync($"```\nSong not found!\n```");
                }
                else
                {
                    if (ConnectionPool.TryGetConnection(Id, out var group) == false)
                    {
                        group = await Connect();
                    }
                    await ReplyAsync($"```\nSong added! {song.Name}, {song.Duration}\n```");
                    group.Queue.AddSong(song);
                    group.Play(false);
                }
            }
        }

        [Command("pause")]
        [Summary("Pausing current voice output")]
        public async Task Pause()
        {
            if (CheckConnection(Id, out var group))
            {
                group.Converter.Pause();
                await ReplyAsync("```\nSong paused!\n```");
            }
        }

        [Command("skip")]
        [Summary("Skipping current song")]
        public async Task Skip()
        {
            if (CheckConnection(Id, out var group))
            {
                var song = group.SkipSong();
                if (song == null)
                {
                    await ReplyAsync($"```\nThere is nothing to skip```");
                }
                else
                {
                    await ReplyAsync($"```\nSong skiped! {song.Name}\n```");
                }
            }
        }

        [Command("remove")]
        [Summary("Removing song in a queue")]
        public async Task Remove(int songId)
        {
            if (songId <= 0)
            {
                throw new ArgumentOutOfRangeException("Index must be greater than zero");
            }
            if (CheckConnection(Id, out var group))
            {
                if (group.Queue.TryRemove(songId, out var song))
                {
                    await ReplyAsync($"```\nSong removed! {song.Name}\n```");
                }
            }
        }

        [Command("resume", RunMode = RunMode.Async)]
        [Summary("Resuming voice output")]
        public async Task Resume()
        {
            if (CheckConnection(Id, out var group))
            {
                if (group.Converter.Paused || group.IsPlaying == false)
                {
                    await ReplyAsync("```\nResuming!\n```");
                    group.Play(true);
                }
            }
        }

        [Command("current")]
        [Summary("Show information about playing song")]
        public async Task GetCurrentSong()
        {
            if (CheckConnection(Id, out var group))
            {
                ISong song = group.Queue.Current;
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
        }

        [Command("queue")]
        [Summary("Shows song queue")]
        public async Task GetListCmd()
        {
            if (CheckConnection(Id, out var group))
            {
                var songs = group.Queue.GetQueueList();
                if (songs.Count() == 0)
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

        [Command("stop")]
        [Summary("Clears queue")]
        public async Task Stop()
        {
            if (CheckConnection(Id, out var group))
            {
                group.Stop();
            }
        }
    }
}
