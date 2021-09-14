using System.IO;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Song.Converter
{
    public interface ISongStreamConverter
    {
        Task ConvertToPCM(SongStream streamIn, Stream streamOut);
    }
}
