using DiscordBotRecognitionCore.BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Linq;

namespace DiscordBotRecognitionCore.BackEnd
{
    public class BackEndService
    {
        private string _domain = "https://apollon-music-resource-server.herokuapp.com";
        private string _discordget => $"{_domain}/public/api/v1/playlists/discord?pageNumber=0&pageSize=10&sortBy=id";
        private string _playlistById => $"{_domain}/public/api/v1/users/me/playlists/";
        private string _playlistByName => $"{_domain}/admin/api/v1/playlists/discord";
        private string _getMusicServices => $"{_domain}/public/api/v1/music-services";
        private string _authPath => $"https://apollon-music-keycloak.herokuapp.com/auth/realms/apollon-music/protocol/openid-connect/token";

        private string _getAllPlaylists => $"{_domain}/admin/api/v1/playlists/discord?pageNumber=0&pageSize=10&sortBy=id";

        private KeycloakAuthResponse _keycloakAuthResponse;
        private HttpClient _client = new HttpClient();
        
        public BackEndService()
        {

        }

        public async Task Initialize(string clientSecret)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_authPath),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", "discord-bot"),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                })
            };

            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                _keycloakAuthResponse = JsonSerializer.Deserialize<KeycloakAuthResponse>(data);
            }
            else
            {
                throw new Exception("Not authorized");
            }
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylists(string[] users, string discordIdentity)
        {
            var content = JsonSerializer.Serialize(users);
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_getAllPlaylists),
                Method = HttpMethod.Post,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = 
                new AuthenticationHeaderValue(_keycloakAuthResponse.TokenType, _keycloakAuthResponse.AccessToken);


            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<Playlist>>(data, options).Where(p => (p.IsPrivate && p.DiscordIdentity == discordIdentity) || p.IsPrivate == false);
            }
            return null;
        }

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

        public async Task<List<Playlist>> GetPlaylists(string requesterDiscordIdentity, IEnumerable<string> requestedDiscordIdentities, string playlistName)
        {
            string query = $"?requesterDiscordIdentity={requesterDiscordIdentity}&requestedDiscordIdentities={string.Join(",", requestedDiscordIdentities)}&playlistName={playlistName}";
            
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_playlistByName + query),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization =
                new AuthenticationHeaderValue(_keycloakAuthResponse.TokenType, _keycloakAuthResponse.AccessToken);


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
            return new List<Playlist>();
        }

        public async Task<List<MusicService>> GetMusicServices()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_getMusicServices),
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
                return JsonSerializer.Deserialize<List<MusicService>>(data, options);
            }
            return null;
        }
    }
}
