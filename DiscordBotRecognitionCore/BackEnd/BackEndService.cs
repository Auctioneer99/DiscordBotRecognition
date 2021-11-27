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
        private string _playlistById => $"{_domain}/admin/api/v1/playlists/";
        private string _getMusicServices => $"{_domain}/public/api/v1/music-services";
        private string _getAllPlaylists => $"{_domain}/admin/api/v1/playlists/discord?pageNumber=0&pageSize=10&sortBy=id";
        private string _authPath => $"https://apollon-music-keycloak.herokuapp.com/auth/realms/apollon-music/protocol/openid-connect/token";


        private KeycloakAuthResponse _keycloakAuthResponse = new KeycloakAuthResponse()
        {
            ExpiresIn = 0
        };
        private DateTime _lastUpdate = new DateTime(0);
        private long _threshold = 30;
        private HttpClient _client = new HttpClient();
        private string _clientSecret;
        
        public BackEndService(string clientSecret)
        {
            _clientSecret = clientSecret;
        }

        private async Task UpdateAccessToken()
        {
            if ((_keycloakAuthResponse.ExpiresIn + _lastUpdate.Ticks / 1000 - _threshold) <= DateTime.Now.Ticks / 1000)
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(_authPath),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", "discord-bot"),
                    new KeyValuePair<string, string>("client_secret", _clientSecret)
                })
                };

                HttpResponseMessage response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _lastUpdate = DateTime.Now;
                    string data = await response.Content.ReadAsStringAsync();
                    _keycloakAuthResponse = JsonSerializer.Deserialize<KeycloakAuthResponse>(data);
                }
                else
                {
                    throw new Exception("Internal server error");
                }
            }
        }

        public async Task<List<PlaylistShort>> GetPublicPlaylistsByUsers(IEnumerable<string> users)
        {
            var query = Uri.EscapeDataString($"{string.Join(",", users)}");
            var url = _discordget + "&discordIdentities=" + query;
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            
            HttpResponseMessage response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<List<PlaylistShort>>(data, options);
                return result;
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
            await UpdateAccessToken();
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
                return JsonSerializer.Deserialize<Playlist>(data, options);
            }
            return null;
        }

        public async Task<List<PlaylistShort>> GetAvailablePlaylists(string requesterDiscordIdentity, IEnumerable<string> requestedDiscordIdentities, string playlistName ="")
        {
            string query = $"&requesterDiscordIdentity={Uri.EscapeDataString(requesterDiscordIdentity)}&requestedDiscordIdentities={Uri.EscapeDataString(string.Join(",", requestedDiscordIdentities))}";
            if (playlistName != "")
            {
                query += $"&playlistName={Uri.EscapeDataString(playlistName)}";
            }


            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_getAllPlaylists + query),
                Method = HttpMethod.Get
            };
            await UpdateAccessToken();
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
                return JsonSerializer.Deserialize<List<PlaylistShort>>(data, options);
            }
            return new List<PlaylistShort>();
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
