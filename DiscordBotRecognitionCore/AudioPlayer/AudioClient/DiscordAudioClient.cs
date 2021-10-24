using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotRecognition.AudioPlayer.AudioClient
{
    public class DiscordAudioClient : IAudioClient
    {
        private Discord.Audio.IAudioClient _client;
        private Discord.Audio.AudioOutStream _audioOutStream;

        private bool _disposed;

        public event Action<ulong, Stream> StreamConnected;

        public event Action Disconnected;

        public DiscordAudioClient(Discord.Audio.IAudioClient client)
        {
            _client = client;
            _audioOutStream = _client.CreatePCMStream(Discord.Audio.AudioApplication.Music);
            _client.StreamCreated += OnStreamCreated;
            _client.Disconnected += OnDisconnected;
        }

        public IReadOnlyDictionary<ulong, Stream> GetStreams()
        {
            return _client.GetStreams() as IReadOnlyDictionary<ulong, Stream>;
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
            StreamConnected?.Invoke(id, stream);
        }

        private async Task OnStreamDisconnected()
        {

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
