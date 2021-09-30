﻿using System;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace DiscordBotRecognition.Song
{
    class YouTubeSong : ISong
    {
        private const string YOUTUBE_PREFIX = "https://www.youtube.com/watch?v=";

        public string Name { get; private set; }
        public TimeSpan Duration { get; private set; }

        private string _url;
        private YoutubeClient _client;

        public YouTubeSong(string url, YoutubeClient client)
        {
            _url = url;
            _client = client;
        }

        public async Task Initialize()
        {
            Video video = await _client.Videos.GetAsync(YOUTUBE_PREFIX + _url);
            Name = video.Title;
            Duration = video.Duration ?? TimeSpan.Zero;
        }

        public async Task<string> GetStreamUrl()
        {
            var streamManifest = await _client.Videos.Streams.GetManifestAsync(YOUTUBE_PREFIX + _url);
            return streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate().Url;
        }

        public override string ToString()
        {
            return $"{Name}, {Duration}";
        }
    }
}
