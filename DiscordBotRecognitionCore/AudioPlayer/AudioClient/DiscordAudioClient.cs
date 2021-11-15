using DiscordBotRecognitionCore.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotRecognition.AudioPlayer.AudioClient
{
    public class DiscordAudioClient : IAudioClient
    {
        public event Action Disconnected;
        public event Action<RecognizableClient> StreamConnected;
        public event Action<ulong> StreamDisconnected;

        public ulong Id { get; }

        private Discord.Audio.IAudioClient _client;
        private Discord.Audio.AudioOutStream _audioOutStream;

        private bool _disposed;

        public DiscordAudioClient(ulong id, Discord.Audio.IAudioClient client)
        {
            Id = id;
            _client = client;
            _audioOutStream = _client.CreatePCMStream(Discord.Audio.AudioApplication.Music);
            _client.StreamCreated += OnStreamCreated;
            _client.StreamDestroyed += OnStreamDisconnected;
            _client.Disconnected += OnDisconnected;
        }

        public IEnumerable<RecognizableClient> GetListenerStreams()
        {
            return _client.GetStreams().Select(pair => new RecognizableClient(pair.Key, pair.Value));
        }

        public Stream GetPCMStream()
        {
            return _audioOutStream;
        }

        private async Task OnDisconnected(Exception ex)
        {
            Disconnected?.Invoke();
        }

        private async Task OnStreamCreated(ulong id, Stream stream)
        {
            StreamConnected?.Invoke(new RecognizableClient(id, stream));
        }

        private async Task OnStreamDisconnected(ulong id)
        {
            StreamDisconnected?.Invoke(id);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected async ValueTask DisposeAsync(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {

                }
                Disconnected?.Invoke();
                await _audioOutStream.DisposeAsync();
                await _client.StopAsync();
                _disposed = true;
            }
        }

        ~DiscordAudioClient()
        {
            DisposeAsync(false);
        }
    }
}
