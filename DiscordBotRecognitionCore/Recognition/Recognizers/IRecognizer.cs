using System;
using System.IO;

namespace DiscordBotRecognition.Recognition
{
    public interface IRecognizer : IAsyncDisposable
    {
        Stream SpeechStream { get; }

        void BeginParse();

        void Stop();
    }
}
