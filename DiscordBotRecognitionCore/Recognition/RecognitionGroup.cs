using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognitionCore.Recognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognition.Recognition
{
    public class RecognitionGroup
    {
        public IAudioClient Me => _audioGroup.Me;
        public ulong Id => Me.Id;

        private Dictionary<ulong, RecognizableClient> _listeningClients;
        private RecognitionSettings _settings;
        private AudioGroup _audioGroup;

        public RecognitionGroup(AudioGroup audioGroup, RecognitionSettings settings)
        {
            _audioGroup = audioGroup;
            _settings = settings;
            _listeningClients = new Dictionary<ulong, RecognizableClient>();
        }

        public bool TryAddRecognizableClient(RecognizableClient client)
        {
            return _listeningClients.TryAdd(client.Id, client);
        }
    }
}
