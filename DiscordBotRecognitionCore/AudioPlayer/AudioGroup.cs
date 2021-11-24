using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.AudioPlayer.Queue;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Songs;
using DiscordBotRecognitionCore.Synthesier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioGroup : IAsyncDisposable
    {
        public ulong Id => Me.Id;
        public IAudioClient Me { get; private set; }
        public PausableConverter Converter { get; private set; }
        public ISongQueue Queue { get; private set; }

        public ISynthesier Synthesier { get; private set; }

        public bool IsPlaying => _isPlaying;

        private AudioGroupSettings _settings;
        private bool _disposed = false;
        private bool _isPlaying = false;
        private CancellationTokenSource _skipTokenSource;

        public AudioGroup(IAudioClient me, ISongStreamConverter converter, ISynthesier synthesier, AudioGroupSettings settings)
        {
            Synthesier = synthesier;
            Me = me;
            Converter = new PausableConverter(converter);
            _settings = settings;
            Queue = new FIFOQueue(_settings.MaxQueueSize);
            _skipTokenSource = new CancellationTokenSource();
        }

        public async Task Play(bool _isResuming)
        {
            if (_isPlaying)
            {
                return;
            }
            _isPlaying = true;
            var streamOut = Me.GetPCMStream();
            if (Converter.Paused)
            {
                if (_isResuming)
                {
                    await Converter.Resume(streamOut, _skipTokenSource.Token);
                }
            }
            while (Converter.Paused == false && Queue.TryGetNextSong(out ISong song))
            {
                Converter.Reset();
                _skipTokenSource = new CancellationTokenSource();
                Converter.SetSong(song);
                await Converter.ConvertToPCM(streamOut, _skipTokenSource.Token);
            }
            _isPlaying = false;
        }

        public ISong SkipSong()
        {
            var song = Queue.Current;
            _skipTokenSource.Cancel();
            Converter.Reset();
            return song;
        }

        public void SetQueueType(EQueueType type)
        {
            Queue = Queue.Convert(type);
        }

        public void Stop()
        {
            Converter.Pause();
            Queue.Clear();
            _skipTokenSource.Cancel();
            Converter.Reset();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        private async Task DisposeAsync(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {

                }
                await Me.DisposeAsync();
                Converter.Dispose();
                _disposed = true;
            }
        }

        ~AudioGroup()
        {
            DisposeAsync(false);
        }
    }
}
