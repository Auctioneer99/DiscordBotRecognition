using DiscordBotRecognition.Converter.Settings;
using DiscordBotRecognition.Songs;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Converter
{
    public class PausableConverter : ISongStreamConverter
    {
        public bool Paused { get; private set; }

        public ConvertSettings Settings => _converter.Settings;

        private ISongStreamConverter _converter;
        private CancellationTokenSource _pauseTokenSource;
        private CancellationTokenSource _linkedTokenSource;
        private bool _disposed;

        public PausableConverter(ISongStreamConverter converter)
        {
            _converter = converter;
        }

        public void SetSong(ISong song)
        {
            _converter.SetSong(song);
        }

        public void Reset()
        {
            _converter.Reset();
            Paused = false;
        }

        public async Task ConvertToPCM(Stream streamOut, CancellationToken skipToken)
        {
            if (Paused)
            {
                return;
            }
            _pauseTokenSource = new CancellationTokenSource();
            _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_pauseTokenSource.Token, skipToken);

            try
            {
                Console.WriteLine("Converting pausable");
                await _converter.ConvertToPCM(streamOut, _linkedTokenSource.Token);
            }
            catch
            {

            }
        }

        public void Pause()
        {
            Paused = true;
            _pauseTokenSource.Cancel();
        }

        public async Task Resume(Stream streamOut, CancellationToken skipToken)
        {
            Paused = false;
            await ConvertToPCM(streamOut, skipToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {

                }
                _converter.Dispose();
                _disposed = true;
            }
        }

        ~PausableConverter()
        {
            Dispose(false);
        }
    }
}
