using DiscordBotRecognition.Songs;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;

namespace DiscordBotRecognition.MusicSearch
{
    public class YouTubeSearcher : IMusicSearcher
    {
        private YoutubeClient _client;
        private YouTubeService _service;
        private string _apiKey;

        public YouTubeSearcher(string apiKey)
        {
            _apiKey = apiKey;
            Initialize();
        }

        private void Initialize()
        {
            BaseClientService.Initializer i = new BaseClientService.Initializer();
            i.ApiKey = _apiKey;
            _service = new YouTubeService(i);
            _client = new YoutubeClient();
        }

        public async Task<ISong> SearchSong(string input)
        {
            ISong song;
            if (input.StartsWith("http://") || input.StartsWith("https://"))
            {
                song = await GetSongByLink(input);
            }
            else
            {
                song = await SearchSongByQuery(input);
            }
            return song;
        }

        private async Task<ISong> GetSongByLink(string link)
        {
            YouTubeSong song = new YouTubeSong(link);
            await song.Initialize();
            return song;
        }

        private async Task<ISong> SearchSongByQuery(string query)
        {
            var search = _service.Search.List("snippet");
            search.Q = query;
            search.MaxResults = 1;
            search.Type = "video";

            var response = await search.ExecuteAsync();
            var rawSong = response.Items.FirstOrDefault();
            if (rawSong == null)
            {
                return null;
            }

            YouTubeSong song = new YouTubeSong("https://www.youtube.com/watch?v=" + rawSong.Id.VideoId);
            await song.Initialize();
            return song;
        }
    }
}
