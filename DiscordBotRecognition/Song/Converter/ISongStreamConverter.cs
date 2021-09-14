using System.IO;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Song.Converter
{
    public interface ISongStreamConverter
    {
        Task ConvertToPCM(ISong song, Stream streamOut);
    }
}
