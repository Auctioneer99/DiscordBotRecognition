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

        [Command("serverplaylists", RunMode = RunMode.Async)]
        [Alias("sp")]
        [Summary("Get all public playlists of users on this server")]
        public async Task GetServerPlaylists([Remainder]string query = "")
        {
            IEnumerable<string> users = null;
            if (query != "")
            {
                var mentions = Context.Message.MentionedUsers;
                if (mentions.Count > 0)
                {
                    users = Context.Message.MentionedUsers.Select(u => u.ToString());
                }
            }
            if (users == null)
            {
                users = await GetAllUsers();
            }
            var response = await Service.GetPublicPlaylistsByUsers(users);
            if (response.Count > 0)
            {
                await SendFormattedMessage(
                    string.Join("\n", 
                    response
                    .GroupBy(p => p.DiscordIdentity)
                    .Select(group => $"Плейлисты пользователя {group.Key}\n" + string.Join(
                        "\n", 
                        group.Select((p, id) => $"\t{id + 1}) {p.Name}, Tracks = {p.TracksCount}")))
                    )
                    );
            }
            else
            {
                await SendFormattedMessage("No playlists found");
            }
        }

        [Command("services")]
        [Summary("Retrieves all supported music services")]
        public async Task GetMusicServices()
        {
            var response = await Service.GetMusicServices();
            await SendFormattedMessage(string.Join(",\n", response.Select((p, i) => $"{i + 1}) {p}")));
        }

        [Command("availableplaylists")]
        [Alias("ap")]
        [Summary("Retrieves all possible playlists for you")]
        public async Task GetAllPlaylists([Remainder] string query = "")
        {
            IEnumerable<string> users = null;
            if (query != "")
            {
                var mentions = Context.Message.MentionedUsers;
                if (mentions.Count > 0)
                {
                    users = Context.Message.MentionedUsers.Select(u => u.ToString());
                }
            }
            if (users == null)
            {
                users = await GetAllUsers();
            }
            var user = Context.User.ToString();
            var response = await Service.GetAvailablePlaylists(user, users.Append(user));
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            if (response.Any())
            {
                var mes = string.Join("\n",
                    response
                    .GroupBy(p => p.DiscordIdentity)
                    .Select(group => $"Плейлисты пользователя {group.Key}\n" + string.Join(
                        "\n",
                        group.Select((p, id) => $"\t{id + 1}) {p.Name}, Tracks = {p.TracksCount}")))
                    );
                await channel.SendMessageAsync($"```\n{mes}\n```");
            }
            else
            {
                await channel.SendMessageAsync("```\nNo available playlists\n```");
            }
        }

        [Command("playlist", RunMode = RunMode.Async)]
        [Alias("pl")]
        [Summary("Plays playlist")]
        public async Task GetAllPlaylistsByName([Remainder] string query = "")
        {
            var playlistName = Regex.Replace(query, @"<@!\d+>", string.Empty).Trim();
            if (playlistName == "")
            {
                await SendFormattedMessage("Playlist did not specified");
                return;
            }
            var requestedIdentities = Context.Message.MentionedUsers.Select(u => u.ToString());
            if (requestedIdentities.Any() == false)
            {
                requestedIdentities = new List<string>() { Context.User.ToString() };
            }

            var response = await Service.GetAvailablePlaylists(Context.User.ToString(), requestedIdentities, playlistName);

            switch(response.Count)
            {
                case 0:
                    await SendFormattedMessage($"No playlists found!");
                    break;
                case 1:
                    {
                        var shortPlaylist = response.FirstOrDefault();
                        var playlist = await Service.GetPlaylist(shortPlaylist.Id);
                        if (playlist == null)
                        {
                            await ReplyAsync("There is no playlist");
                            return;
                        }
                        var songs = playlist.Tracks.Select(t => t.Convert());
                        if (ConnectionPool.TryGetConnection(Id, out var group) == false)
                        {
                            group = await Connect();
                        }
                        bool isFirst = true;
                        foreach (var s in songs)
                        {
                            group.Queue.AddSong(s.GetAwaiter().GetResult());
                            if (isFirst)
                            {
                                await SendFormattedMessage($"Playlist added! {playlist.Name}, {playlist.DiscordIdentity}, Count = {playlist.Tracks.Count}");
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
                        await SendFormattedMessage($"Many playlists");
                        //group.Queue.AddSong(song);
                        //group.Play(false);
                    }
                    break;
            }
        }

        private async Task<IEnumerable<string>> GetAllUsers()
        {
            return (await Context.Channel.GetUsersAsync(CacheMode.AllowDownload).FlattenAsync())
                .Select(user => user.ToString());
        }
    }
}
