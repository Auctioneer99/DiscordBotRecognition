using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBotRecognitionCore.BackEnd;
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
            var users = (await Context.Channel.GetUsersAsync(CacheMode.AllowDownload).FlattenAsync())
                .Select(user => user.ToString())
                .ToArray();
            var response = await Service.GetPlaylistsByUsers(users);
            await ReplyAsync(string.Join(",\n", response.Select((p, i) => $"{i + 1}) {p}")));
        }

        [Command("playlist ")]
        [Summary("Plays all tracks specified in a playlist")]
        public async Task AddTracksAsPlaylist()
        {

        }
    }
}
