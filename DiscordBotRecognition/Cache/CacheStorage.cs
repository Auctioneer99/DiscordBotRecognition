using DiscordBotRecognition.Songs;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Cache
{
    public class CacheStorage
    {
        private const int _cacheInDays = 7;
        private string _localPath = Path.Join(Directory.GetCurrentDirectory(), "temp");

        public CacheStorage()
        {
            RemoveOldFiles();
        }

        public async Task<string> SaveWebFile(ISong song)
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
                        await stream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return path;
        }

        public bool IsFileExist(ISong song, out string url)
        {
            url = Path.Join(_localPath, song.Id);
            return File.Exists(url);
        }

        private void RemoveOldFiles()
        {
            DateTime threshold = DateTime.Now;
            threshold = threshold.AddDays(-_cacheInDays);
            var files = Directory.GetFiles(_localPath);
            foreach (var file in files)
            {
                if (File.GetLastAccessTime(file) <= threshold)
                {
                    File.Delete(file);
                }
            }
        }
    }
}
