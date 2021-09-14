using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotRecognition.AudioPlayer.AudioClient
{
    public class DiscordAudioClient : IAudioClient
    {
        private Discord.Audio.IAudioClient _client;
        private Discord.Audio.AudioOutStream _audioOutStream;

        private bool _disposed;

        public DiscordAudioClient(Discord.Audio.IAudioClient client)
        {
            _client = client;
            _audioOutStream = _client.CreatePCMStream(Discord.Audio.AudioApplication.Music);
        }

        public Stream GetPCMStream()
        {
            return _audioOutStream;
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
