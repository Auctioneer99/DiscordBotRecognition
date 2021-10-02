using DiscordBotRecognition.Song;
using System;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Cache
{
    public class CachedSong : ISong
    {
        public string Name => _song.Name;

        public TimeSpan Duration => _song.Duration;

        private ISong _song;
        private string _url;
        private CacheStorage _cacheStorage;

        public CachedSong(ISong song, CacheStorage cacheStorage)
        {
            _cacheStorage = cacheStorage;
            _song = song;
        }

        public async Task<string> GetStreamUrl()
        {
            if (string.IsNullOrEmpty(_url))
            {
                _url = await _song.GetStreamUrl();
                Console.WriteLine($"url setted to {_url}");
            }
            Console.WriteLine($"Returning url {_url}");
            return _url;
        }

        public async Task<bool> IsLocal()
        {
            return _cacheStorage.IsFileExist(await GetStreamUrl(), out var junk);
        }

        public async Task CacheToLocalSystem()
        {
            string url = await GetStreamUrl();
            if (_cacheStorage.IsFileExist(url, out var localUrl))
            {
                _url = localUrl;
                Console.WriteLine($"url setted to {_url}");
            }
            else
            {
                _url = await _cacheStorage.SaveWebFile(_url);
                Console.WriteLine($"url setted to {_url}");
            }
        }
    }
}
