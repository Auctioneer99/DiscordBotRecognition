using System;
using System.IO;

namespace DiscordBotRecognitionCore.Recognition
{
    public class RecognizableClient
    {
        public event Action Disconnected;

        public ulong Id { get; private set; }
        public Stream AudioIn { get; private set; }

        public RecognizableClient(ulong id, Stream audioIn)
        {
            Id = id;
            AudioIn = audioIn;
        }
    }
}
