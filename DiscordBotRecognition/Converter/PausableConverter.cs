using DiscordBotRecognition.Song;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Converter
{
    public class PausableConverter : ISongStreamConverter
    {
        public bool Paused { get; private set; }

        private ISongStreamConverter _converter;
        private CancellationTokenSource _pauseTokenSource;
        private CancellationTokenSource _linkedTokenSource;

        public PausableConverter(ISongStreamConverter converter)
        {
            _converter = converter;
        }

        public async Task SetSong(ISong song)
        {
            await _converter.SetSong(song);
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
                await _converter.ConvertToPCM(streamOut, _linkedTokenSource.Token);
            }
            catch(OperationCanceledException)
            {
                if (skipToken.IsCancellationRequested)
                {

                }
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
    }
}
