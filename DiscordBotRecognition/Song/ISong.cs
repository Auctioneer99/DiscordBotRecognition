using System;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Song
{
    public interface ISong
    {
        string Name { get; }
        TimeSpan Duration { get; }

        Task<SongStream> GetStream();

        Task<string> GetStreamUrl();
    }
}
