using DiscordBotRecognitionCore.Recognition;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBotRecognition.AudioPlayer.AudioClient
{
    public interface IAudioClient : IAsyncDisposable
    {
        event Action<RecognizableClient> StreamConnected;
        event Action<ulong> StreamDisconnected;

        event Action Disconnected;

        ulong Id { get; }

        IEnumerable<RecognizableClient> GetListenerStreams();
        Stream GetPCMStream();
    }
}
