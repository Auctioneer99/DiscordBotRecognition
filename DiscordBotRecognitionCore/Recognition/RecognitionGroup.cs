using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognitionCore.Recognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognition.Recognition
{
    public class RecognitionGroup
    {
        public ulong Id => _client.Id;

        private Dictionary<ulong, RecognizableClient> _listeningClients;
        private IAudioClient _client;
        private IRecognizer _recognizer;
        private RecognitionSettings _settings;

        public RecognitionGroup(IAudioClient client, IRecognizer recognizer, RecognitionSettings settings)
        {
            _client = client;
            _recognizer = recognizer;
            _settings = settings;
            _listeningClients = new Dictionary<ulong, RecognizableClient>();
        }

        public void AddRecognizableClient(RecognizableClient client)
        {

        }
    }
}
