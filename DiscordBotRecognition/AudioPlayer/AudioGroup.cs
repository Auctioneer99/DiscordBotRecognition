using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Song;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioGroup : IAsyncDisposable
    {
        public IAudioClient Me { get; private set; }
        public List<ISong> QueuedSongs { get; private set; }
        public ISong Current { get; private set; } = null;
        public bool Paused => _converter.Paused;

        private PausableConverter _converter;
        private AudioGroupSettings _settings;
        private bool _disposed = false;
        private bool _isPlaying = false;
        private CancellationTokenSource _skipTokenSource;

        public AudioGroup(IAudioClient me, ISongStreamConverter converter, AudioGroupSettings settings)
        {
            Me = me;
            _converter = new PausableConverter(converter);
            _settings = settings;
            Initialize();
        }

        private void Initialize()
        {
            QueuedSongs = new List<ISong>();
            _skipTokenSource = new CancellationTokenSource();
        }

        public void AppendSong(ISong song)
        {
            if (_settings.MaxQueueSize > QueuedSongs.Count)
            {
                QueuedSongs.Add(song);
            }
            else
            {
                throw new Exception($"Queue limit reached ({_settings.MaxQueueSize}), song not added");
            }
        }

        public async Task Play(bool isResuming = false)
        {
            if (_isPlaying)
            {
                return;
            }
            _isPlaying = true;
            while(QueuedSongs.Count > 0)
            {
                _skipTokenSource = new CancellationTokenSource();
                var streamOut = Me.GetPCMStream();

                if (_converter.Paused)
                {
                    if (isResuming)
                    {
                        await _converter.Resume(streamOut, _skipTokenSource.Token);
                    }
                }
                else
                {
                    Current = QueuedSongs.First();
                    await _converter.SetSong(Current);
                    await _converter.ConvertToPCM(streamOut, _skipTokenSource.Token);
                }

                if (_converter.Paused)
                {
                    break;
                }
                QueuedSongs.RemoveAt(0);
                Current = null;
                _converter.Reset();
            }
            _isPlaying = false;
        }

        public ISong SkipSong(int id)
        {
            if (QueuedSongs.Count > id && id >= 0)
            {
                ISong song = QueuedSongs[id];
                if (id == 0)
                {
                    if (_converter.Paused)
                    {
                        QueuedSongs.RemoveAt(id);
                        Current = null;
                    }
                    _skipTokenSource.Cancel();
                    _converter.Reset();
                }
                else
                {
                    QueuedSongs.RemoveAt(id);
                }
                return song;
            }
            else
            {
                throw new Exception("Song id must be within size of queue");
            }
        }

        public void PauseSong()
        {
            if (Current != null)
            {
                _converter.Pause();
            }
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
                _converter.Reset();
                _disposed = true;
            }
        }

        ~AudioGroup()
        {
            DisposeAsync(false);
        }
    }
}
