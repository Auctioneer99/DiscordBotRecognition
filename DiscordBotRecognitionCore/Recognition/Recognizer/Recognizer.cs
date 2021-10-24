using System;
using System.IO;
using System.Speech.Recognition;

namespace DiscordBotRecognition.Recognition
{
    public class Recognizer : IRecognizer
    {
        public bool IsParsing { get; private set; }

        public Stream InputStream => _speechStream;

        private SpeechRecognitionEngine _recognitionEngine;
        private SpeechStream _speechStream;

        public Recognizer()
        {
            Initialize();
        }

        private void Initialize()
        {
            _recognitionEngine = new SpeechRecognitionEngine();
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(new Choices("left", "right", "up", "down"));
            _recognitionEngine.UnloadAllGrammars();
            _recognitionEngine.LoadGrammar(new Grammar(grammarBuilder));
            _speechStream = new SpeechStream(16 * 1024);
            _recognitionEngine.SetInputToWaveStream(_speechStream);
        }

        public void BeginParse()
        {
            if (IsParsing == false)
            {
                IsParsing = true;
                _recognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
                _recognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        public void Stop()
        {
            if (IsParsing)
            {
                _recognitionEngine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
                _recognitionEngine.RecognizeAsyncStop();
            }
        }

        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs args)
        {
            Console.WriteLine(args.Result.Text);
        }
    }
}
