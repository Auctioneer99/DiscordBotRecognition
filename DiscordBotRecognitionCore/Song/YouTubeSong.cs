using System;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace DiscordBotRecognition.Songs
{
    class YouTubeSong : ISong
    {
        private const string YOUTUBE_PREFIX = "https://www.youtube.com/watch?v=";

        private string _url;
        private YoutubeClient _client;

        public string Id { get; private set; }

        public string Name { get; private set; }

        public TimeSpan Duration { get; private set; }

        public string StreamUrl { get; private set; }

        private bool _disposed;

        public YouTubeSong(string url, YoutubeClient client)
        {
            _url = url;
            _client = client;
        }

        public async Task Initialize()
        {
            Video video = await _client.Videos.GetAsync(YOUTUBE_PREFIX + _url);
            var temp = video.Url.Split('=');
            Id = temp[temp.Length - 1];
            Name = video.Title;
            Duration = video.Duration ?? TimeSpan.Zero;
            var streamManifest = await _client.Videos.Streams.GetManifestAsync(YOUTUBE_PREFIX + _url);
            StreamUrl = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate().Url;
        }

        public override string ToString()
        {
            return $"{Name}, {Duration}";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {

                }

                _disposed = true;
            }
        }

        ~YouTubeSong()
        {
            Dispose(false);
        }
    }
}
