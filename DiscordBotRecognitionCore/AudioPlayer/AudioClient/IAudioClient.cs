using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBotRecognition.AudioPlayer.AudioClient
{
    public interface IAudioClient : IAsyncDisposable
    {
        event Action Disconnected;

        IReadOnlyDictionary<ulong, Stream> GetStreams();

        Stream GetPCMStream();
    }
}
