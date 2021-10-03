using DiscordBotRecognition.Songs;
using System;
using System.Threading.Tasks;

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
        private CacheStorage _cacheStorage;

        public CachedSong(ISong song, CacheStorage cacheStorage)
        {
            _cacheStorage = cacheStorage;
            _song = song;
        }

        public bool IsLocal()
        {
            return _cacheStorage.IsFileExist(this, out var junk);
        }

        public async Task CacheToLocalSystem()
        {
            if (_cacheStorage.IsFileExist(this, out var localPath))
            {
                _localPath = localPath;
            }
            else
            {
                _localPath = await _cacheStorage.SaveWebFile(this);
            }
            _cached = true;
        }

        public override int GetHashCode()
        {
            return _song.GetHashCode();
        }
    }
}
