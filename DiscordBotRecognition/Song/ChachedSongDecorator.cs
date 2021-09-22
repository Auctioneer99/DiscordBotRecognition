using System;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Song
{
    public class ChachedSongDecorator : ISong
    {
        public string Name => _song.Name;
        public TimeSpan Duration => _song.Duration;

        private ISong _song;
        private SongStream _stream;

        public ChachedSongDecorator(ISong song)
        {
            _song = song;
        }

        public async Task<SongStream> GetStream()
        {
            if (_stream.Stream == null)
            {
                _stream = await _song.GetStream();
            }
            return _stream;
        }

        public async Task<string> GetStreamUrl()
        {
            return await _song.GetStreamUrl();
        }
    }
}
