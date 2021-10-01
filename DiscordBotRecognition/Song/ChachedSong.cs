using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Song
{
    public class ChachedSong : ISong
    {
        public string Name => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        private ISong _song;

        public ChachedSong(ISong song)
        {
            _song = song;
        }

        public Task<string> GetStreamUrl()
        {
            throw new NotImplementedException();
        }
    }
}
