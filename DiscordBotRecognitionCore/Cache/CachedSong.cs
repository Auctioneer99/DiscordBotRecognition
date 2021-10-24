using DiscordBotRecognition.Songs;
using System;
using System.Threading;

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
        private bool _disposed;
        private CancellationTokenSource _cachingToken;

        public CachedSong(ISong song, CancellationTokenSource cachingToken)
        {
            _song = song;
            _cachingToken = cachingToken;
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
                _cachingToken?.Cancel();
                _song.Dispose();
                _disposed = true;
            }
        }

        ~CachedSong()
        {
            Dispose(false);
        }

        public override string ToString()
        {
            return _song.ToString();
        }
    }
}
