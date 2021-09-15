using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Song;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioGroup : IAsyncDisposable
    {
        public IAudioClient Me { get; private set; }
        public List<ISong> QueuedSongs { get; private set; }
        public ISong Current { get; private set; }

        private ISongStreamConverter _converter;
        private AudioGroupSettings _settings;
        private bool _disposed = false;
        private bool _isPlaying = false;
        private CancellationTokenSource _cancelTokenSource;

        public AudioGroup(IAudioClient me, ISongStreamConverter converter, AudioGroupSettings settings)
        {
            Me = me;
            _converter = converter;
            _settings = settings;
            Initialize();
        }

        private void Initialize()
        {
            QueuedSongs = new List<ISong>();
            _cancelTokenSource = new CancellationTokenSource();
        }

        public void AppendSong(ISong song)
        {
            if (_settings.MaxQueueSize > QueuedSongs.Count)
            {
                QueuedSongs.Add(song);
                Task.Run(() => PlayNextSong());
            }
            else
            {
                throw new Exception($"Queue limit reached ({_settings.MaxQueueSize}), song not added");
            }
        }

        public ISong SkipSong(int id)
        {
            id--;
            if (QueuedSongs.Count > id && id >= 0)
            {
                ISong song = QueuedSongs[id];
                if (id == 0)
                {
                    _cancelTokenSource.Cancel();
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

        public async Task PlayNextSong()
        {
            if (_isPlaying)
            {
                return;
            }
            Current = QueuedSongs.FirstOrDefault();
            if (Current == null)
            {
                return;
            }

            _isPlaying = true;
            _cancelTokenSource = new CancellationTokenSource();
            var streamOut = Me.GetPCMStream();
            Current.BeginPlay = DateTime.Now;
            await _converter.ConvertToPCM(Current, streamOut, _cancelTokenSource.Token);
            QueuedSongs.RemoveAt(0);
            _isPlaying = false;

            await PlayNextSong();
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
                _disposed = true;
            }
        }

        ~AudioGroup()
        {
            DisposeAsync(false);
        }
    }
}
