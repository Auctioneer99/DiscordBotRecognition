using System;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Song
{
    public interface ISong
    {
        string Name { get; }
        TimeSpan Duration { get; }
        DateTime BeginPlay { get; set; }

        Task<SongStream> GetStream();

        Task<string> GetStreamUrl();
    }
}
