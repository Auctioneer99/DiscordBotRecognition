using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBotRecognitionCore.BackEnd;
using DiscordBotRecognitionCore.BackEnd.Models;
using Keycloak;

namespace DiscordBotRecognitionCore.Modules
{
    public class BackEndModule : AModuleBase
    {
        public BackEndService Service { get; set; }

        [Command("serverplaylist", RunMode = RunMode.Async)]
        [Summary("Get all public playlists of users on this server")]
        public async Task GetServerPlaylists()
        {
            var users = await GetAllUsers();
            var response = await Service.GetPlaylistsByUsers(users);
            await ReplyAsync(string.Join(",\n", response.Select((p, i) => $"{i + 1}) {p}")));
        }

        [Command("services")]
        [Summary("Retrieves all supported music services")]
        public async Task GetMusicServices()
        {
            var response = await Service.GetMusicServices();
            await ReplyAsync(string.Join(",\n", response.Select((p, i) => $"{i + 1}) {p}")));
        }

        [Command("playlists")]
        [Summary("Retrieves all supported music services")]
        public async Task GetAllPlaylists()
        {
            var response = await Service.GetAllPlaylists(await GetAllUsers(), Context.User.ToString());
            await (await Context.User.GetOrCreateDMChannelAsync())
                .SendMessageAsync(string.Join(",\n", response.Select((p, i) => $"{i + 1}) {p}")));
        }

        [Command("playlist", RunMode = RunMode.Async)]
        [Summary("Plays playlist")]
        public async Task GetAllPlaylistsByName([Remainder] string query = "")
        {
            var playlist = Regex.Replace(query, @"<@!\d+>", string.Empty).Trim();
            if (playlist == "")
            {
                await ReplyAsync("Playlist did not specified");
                return;
            }
            var requestedIdentities = Context.Message.MentionedUsers.Select(u => u.UrlEncodedName());
            if (requestedIdentities.Any() == false)
            {
                requestedIdentities = new List<string>() { Context.User.UrlEncodedName() };
            }


            var response = await Service.GetPlaylists(Context.User.UrlEncodedName(), requestedIdentities, playlist);

            switch(response.Count)
            {
                case 0:
                    await ReplyAsync($"```\nNo playlists found!\n```");
                    break;
                case 1:
                    {
                        if (ConnectionPool.TryGetConnection(Id, out var group) == false)
                        {
                            group = await Connect();
                        }
                        var pl = response.FirstOrDefault();
                        var songs = pl.Tracks.Select(t => t.Convert());
                        await ReplyAsync($"```\nPlaylist added! {pl.Name}, {pl.DiscordIdentity}\n```");
                        bool isFirst = true;
                        foreach (var s in songs)
                        {
                            group.Queue.AddSong(s.GetAwaiter().GetResult());
                            if (isFirst)
                            {
                                group.Play(false);
                                isFirst = false;
                            }
                        }
                    }
                    break;
                default:
                    {
                        //if (ConnectionPool.TryGetConnection(Id, out var group) == false)
                        //{
                        //    group = await Connect();
                        //}
                        await ReplyAsync($"```\nMany playlists\n```");
                        //group.Queue.AddSong(song);
                        //group.Play(false);
                    }
                    break;
            }
        }

        private async Task<string[]> GetAllUsers()
        {
            return (await Context.Channel.GetUsersAsync(CacheMode.AllowDownload).FlattenAsync())
                .Select(user => user.ToString())
                .ToArray();
        }
    }
}
