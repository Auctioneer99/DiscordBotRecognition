using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.AudioPlayer.Queue;
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
        //public List<ISong> QueuedSongs { get; private set; }
        //public ISong Current { get; private set; } = null;
        public PausableConverter Converter { get; private set; }
        public ISongQueue Queue { get; private set; }

        private AudioGroupSettings _settings;
        private bool _disposed = false;
        private bool _isPlaying = false;
        private CancellationTokenSource _skipTokenSource;

        public AudioGroup(IAudioClient me, ISongStreamConverter converter, AudioGroupSettings settings)
        {
            Me = me;
            Converter = new PausableConverter(converter);
            _settings = settings;
            Queue = new FIFOQueue(_settings.MaxQueueSize);
            //QueuedSongs = new List<ISong>();
            _skipTokenSource = new CancellationTokenSource();
        }
        /*
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
        }*/

        public async Task Play(bool isResuming = false)
        {
            if (_isPlaying)
            {
                return;
            }
            _isPlaying = true;
            while(Queue.TryGetNextSong(out ISong song))
            {
                _skipTokenSource = new CancellationTokenSource();
                var streamOut = Me.GetPCMStream();

                if (Converter.Paused)
                {
                    if (isResuming)
                    {
                        await Converter.Resume(streamOut, _skipTokenSource.Token);
                    }
                }
                else
                {
                    await Converter.SetSong(song);
                    await Converter.ConvertToPCM(streamOut, _skipTokenSource.Token);
                }

                if (Converter.Paused)
                {
                    break;
                }
                Converter.Reset();
            }
            _isPlaying = false;
        }

        public ISong SkipSong()
        {
            var song = Queue.Current;
            _skipTokenSource.Cancel();
            Converter.Reset();
            return song;
            /*
            if (QueuedSongs.Count > id && id >= 0)
            {
                ISong song = QueuedSongs[id];
                if (id == 0)
                {
                    if (Converter.Paused)
                    {
                        QueuedSongs.RemoveAt(id);
                        Current = null;
                    }
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
            }*/
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
                Converter.Reset();
                _disposed = true;
            }
        }

        ~AudioGroup()
        {
            DisposeAsync(false);
        }
    }
}
