using DiscordBotRecognition.Song;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Converter
{
    public interface ISongStreamConverter
    {
        Task ConvertToPCM(ISong song, Stream streamOut);
    }
}
