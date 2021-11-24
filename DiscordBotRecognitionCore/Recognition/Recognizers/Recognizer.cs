using System;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Recognition
{
    public class Recognizer : IRecognizer
    {
        public event Action<string> Parsed;

        public bool IsParsing { get; private set; }
        public Stream SpeechStream => _speechStream;

        private SpeechRecognitionEngine _recognitionEngine;
        private SpeechStream _speechStream;

        public Recognizer()
        {
            _recognitionEngine = new SpeechRecognitionEngine();
            // grammarBuilder = new GrammarBuilder();
            //grammarBuilder.Append(new Choices("left", "right", "up", "down"));
            //_recognitionEngine.UnloadAllGrammars();
            //_recognitionEngine.LoadGrammar(new Grammar(grammarBuilder));
            _recognitionEngine.LoadGrammar(new DictationGrammar());
            _speechStream = new SpeechStream(16 * 1024);
            _recognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
            _recognitionEngine.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>((a, b) => Console.WriteLine(b.AudioState));
            _recognitionEngine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>((a, b) => Console.WriteLine(b.AudioLevel));
            _recognitionEngine.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>((a, b) => Console.WriteLine(b.AudioPosition));
            _recognitionEngine.SetInputToAudioStream(_speechStream,
                new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
        }

        public void BeginParse()
        {
            if (IsParsing == false)
            {
                IsParsing = true;
                _recognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        public void Stop()
        {
            if (IsParsing)
            {
                _recognitionEngine.RecognizeAsyncStop();
                IsParsing = false;
            }
        }

        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs args)
        {
            Console.WriteLine(args.Result.Text);
            Parsed?.Invoke(args.Result.Text);
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
