using DiscordBotRecognition.Songs;
using System;

namespace DiscordBotRecognition.Cache
{
    public class CachedSong : ISong
    {
        public string Id => _song.Id;

        public string Name => _song.Name;

        public TimeSpan Duration => _song.Duration;

        public string StreamUrl => _cached ? _localPath : _song.StreamUrl;

        private ISong _song;
        private bool _cached;
        private string _localPath;

        public CachedSong(ISong song)
        {
            _song = song;
        }

        public CachedSong(ISong song, string path)
        {
            _song = song;
            SetCachedPath(path);
        }

        public void SetCachedPath(string path)
        {
            _cached = true;
            _localPath = path;
        }
    }
}
