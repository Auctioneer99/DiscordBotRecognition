using DiscordBotRecognition.Song;
using System;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;

namespace DiscordBotRecognition.MusicSearch
{
    public class SoundCloudSearcher : IMusicSearcher
    {
        private HttpClient _client;

        public void Initialize(string token)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.ConnectionClose = true;
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MySoundCloudClient", "1.0"));
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
        }

        public async Task<ISong> SearchSong(string query)
        {
            var uri = new Uri("https://api.soundcloud.com/tracks?q=" + query);
            HttpResponseMessage message = await _client.GetAsync(uri);
            Console.WriteLine(await message.Content.ReadAsStringAsync());
            return null;
            /*
            try
            {
                Console.WriteLine(1);
                Console.WriteLine(2);
                var tracks = await _client.Tracks.GetAsync(query, 1);
                Console.WriteLine(3);
                var track = tracks.FirstOrDefault();
                Console.WriteLine(4);
                ISong song = new SoundCloudSong(track);
                Console.WriteLine(5);
                return song;
            }
            catch (SoundCloudException ex)
            {
                Console.WriteLine(22);
                Console.WriteLine("error");
                Console.WriteLine(23);
                Console.WriteLine(string.Join(", ", ex.Errors.errors));
                Console.WriteLine(24);
                return null;
            }*/
        }
    }
}
