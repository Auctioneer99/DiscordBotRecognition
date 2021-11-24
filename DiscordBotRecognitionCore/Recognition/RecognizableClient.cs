using Discord.Audio.Streams;
using DiscordBotRecognition.Recognition;
using DiscordBotRecognitionCore.Recognition.Recognizers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Recognition
{
    public class RecognizableClient : IAsyncDisposable
    {
        public event Action Disconnected;

        public ulong Id { get; private set; }
        public Stream AudioIn { get; private set; }

        private IRecognizer _recognizer;
        private IRecognizerFactory _recognizerFactory;
        private CancellationTokenSource _tokenSource;

        private bool _disposed;

        public RecognizableClient(ulong id, Stream audioIn, IRecognizerFactory recognizerFactory)
        {
            Id = id;
            AudioIn = audioIn;
            _recognizerFactory = recognizerFactory;
            _tokenSource = new CancellationTokenSource();
        }


        public void StartListening()
        {
            _tokenSource = new CancellationTokenSource();
            _recognizer = _recognizerFactory.Create();
            AudioIn.CopyToAsync(_recognizer.SpeechStream);
            _recognizer.BeginParse();
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
                _tokenSource.Cancel();
                _recognizer.Stop();
                _disposed = true;
            }
        }

        ~RecognizableClient()
        {
            DisposeAsync(false);
        }
    }
}
