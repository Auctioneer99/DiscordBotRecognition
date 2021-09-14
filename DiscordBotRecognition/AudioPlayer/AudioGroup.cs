﻿using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.Song;
using DiscordBotRecognition.Song.Converter;
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
        private bool _disposed = false;
        private bool _isPlaying = false;
        private CancellationTokenSource _cancelTokenSource;

        public AudioGroup(IAudioClient me, ISongStreamConverter converter)
        {
            Me = me;
            _converter = converter;
            Initialize();
        }

        private void Initialize()
        {
            QueuedSongs = new List<ISong>();
            _cancelTokenSource = new CancellationTokenSource();
        }

        public void AppendSong(ISong song)
        {
            QueuedSongs.Add(song);
            Task.Run(() => PlayNextSong());
        }

        public void SkipSong(int id)
        {
            if (QueuedSongs.Count > id && id >= 0)
            {
                QueuedSongs.RemoveAt(id);
                if (id == 0)
                {
                    _cancelTokenSource.Cancel();
                }
            }
        }

        public async Task PlayNextSong()
        {
            if (_isPlaying)
            {
                Console.WriteLine("Already Playing");
                return;
            }
            Current = QueuedSongs.FirstOrDefault();
            if (Current == null)
            {
                return;
            }
            Console.WriteLine("Playing song");
            Current.BeginPlay = DateTime.Now;
            _isPlaying = true;

            Console.WriteLine(0);
            SongStream streamContaineer = await Current.GetStream();
            using (streamContaineer.Stream)
            {
                Console.WriteLine(1);

                var streamOut = Me.GetPCMStream();
                Console.WriteLine(2);

                await _converter.ConvertToPCM(streamContaineer, streamOut);

                Console.WriteLine(4);

                if (_cancelTokenSource.IsCancellationRequested == false)
                {
                    QueuedSongs.RemoveAt(0);
                }
                _isPlaying = false;
                Console.WriteLine(3);
                await PlayNextSong();
                /*
                using (outputStream.Stream)
                {
                    Console.WriteLine(2);
                    using (var stream = Me.GetPCMStream())
                    {
                        try
                        {
                            Console.WriteLine(3);
                            await outputStream.Stream.CopyToAsync(stream);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error");
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            Console.WriteLine(4);
                            await stream.FlushAsync();
                            if (_cancelTokenSource.IsCancellationRequested == false)
                            {
                                QueuedSongs.RemoveAt(0);
                            }
                            _isPlaying = false;
                            await PlayNextSong();
                        }
                    }
                }*/
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
                _disposed = true;
            }
        }

        ~AudioGroup()
        {
            DisposeAsync(false);
        }
    }
}
