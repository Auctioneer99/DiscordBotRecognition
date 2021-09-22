using System.IO;

namespace DiscordBotRecognition.Recognition
{
    public interface IRecognizer
    {
        Stream InputStream { get; }

        void BeginParse();

        void Stop();
    }
}
