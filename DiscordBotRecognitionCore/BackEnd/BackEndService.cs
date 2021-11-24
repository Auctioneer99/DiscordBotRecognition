using DiscordBotRecognitionCore.BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DiscordBotRecognitionCore.BackEnd
{
    public class BackEndService
    {
        private string _domain = "https://apollon-music-resource-server.herokuapp.com/public";
        private string _discordget => $"{_domain}/api/v1/playlists/discord?pageNumber=0&pageSize=10&sortBy=id";
        private string _playlistById => $"{_domain}/api/v1/users/me/playlists/";


        HttpClient _client = new HttpClient();

        public async Task<List<Playlist>> GetPlaylistsByUsers(string[] users)
        {
            var content = JsonSerializer.Serialize(users);
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_discordget),
                Method = HttpMethod.Post,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<Playlist>>(data, options);
            }
            return null;
        }

        public async Task<Playlist> GetPlaylist(long id)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_playlistById + id),
                Method = HttpMethod.Get,
            };

            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<Playlist>(data, options);
            }
            return null;
        }
    }
}
