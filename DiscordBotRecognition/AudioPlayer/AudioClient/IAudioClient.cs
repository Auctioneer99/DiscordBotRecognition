using System;
using System.IO;

namespace DiscordBotRecognition.AudioPlayer.AudioClient
{
    public interface IAudioClient : IAsyncDisposable
    {
        Stream GetPCMStream();
    }
}
