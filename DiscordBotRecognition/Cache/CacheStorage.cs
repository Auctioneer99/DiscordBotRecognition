using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Cache
{
    public class CacheStorage
    {
        private string _localPath = Path.Join(Directory.GetCurrentDirectory(), "temp");

        public CacheStorage()
        {
            RemoveOldFiles();
        }

        public async Task<string> SaveWebFile(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = await request.GetResponseAsync();

            string name = GetUrlName(url);
            string path = Path.Join(_localPath, name);
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

        public bool IsFileExist(string name, out string url)
        {
            name = GetUrlName(name);
            url = Path.Join(_localPath, name);
            return Directory.Exists(url);
        }

        private string GetUrlName(string url)
        {
            return url.GetHashCode().ToString();
        }

        private void RemoveOldFiles()
        {
            var files = Directory.GetFiles(_localPath);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
