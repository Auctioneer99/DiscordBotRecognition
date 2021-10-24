using DiscordBotRecognition.Songs;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Cache
{
    public class CacheStorage
    {
        private const int CACHE_DAYS = 7;
        private string _localPath = Path.Join(Directory.GetCurrentDirectory(), "temp");

        public CacheStorage()
        {
            RemoveOldFiles();
        }

        public CachedSong GetCachedFile(ISong song)
        {
            if (IsFileExist(song, out string path))
            {
                return new CachedSong(song, path);
            }
            else
            {
                var cacheToken = new CancellationTokenSource();
                var cachedSong = new CachedSong(song, cacheToken);
                Task.Run(async () =>
                {
                    var path = await SaveWebFile(song, cacheToken);
                    cachedSong.SetCachedPath(path);

                });
                return cachedSong;
            }
        }

        private bool IsFileExist(ISong song, out string url)
        {
            url = Path.Join(_localPath, song.Id);
            return File.Exists(url);
        }

        private void RemoveOldFiles()
        {
            DateTime threshold = DateTime.Now;
            threshold = threshold.AddDays(-CACHE_DAYS);
            if (Directory.Exists(_localPath) == false)
            {
                Directory.CreateDirectory(_localPath);
            }
            var files = Directory.GetFiles(_localPath);
            foreach (var file in files)
            {
                if (File.GetLastAccessTime(file) <= threshold)
                {
                    File.Delete(file);
                }
            }
        }

        private async Task<string> SaveWebFile(ISong song, CancellationTokenSource cacheToken)
        {
            WebRequest request = WebRequest.Create(song.StreamUrl);
            WebResponse response = await request.GetResponseAsync();

            string path = Path.Join(_localPath, song.Id);
            try
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (var fileStream = File.Create(path))
                    {
                        await stream.CopyToAsync(fileStream, cacheToken.Token);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Cancelling Cache Operation");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            return path;
        }
    }
}
