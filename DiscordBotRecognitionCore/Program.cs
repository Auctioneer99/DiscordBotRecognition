using System;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Recognition;
using System.Threading.Tasks;
using DiscordBotRecognition.Credentials;
using DiscordBotRecognition.Recognition;

namespace DiscordBotRecognition
{
    internal class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            //Task.Run(async () => await Test());

            //var bot = await DiscordBot.DefaultBuild("Пошлите в каэску господа", Credential.GoogleAPIToken, Credential.KeycloakSecret);
            //await bot.Start(Credential.DiscordToken);
            throw new NotImplementedException("Do not run project");
            await Task.Delay(-1);
        }
        /*
        private async Task Test()
        {
            WaveInEvent waveSource = null;
            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(44100, 1);
            SpeechStream s = new SpeechStream(16 * 1024);
            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            //Recognizer r = new Recognizer();
            waveSource.StartRecording();


            var _recognitionEngine = new SpeechRecognitionEngine();
            Console.WriteLine(2);
            _recognitionEngine.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>((a, b) => Console.WriteLine("error"));
            _recognitionEngine.AudioSignalProblemOccurred += new EventHandler<AudioSignalProblemOccurredEventArgs>((a, b) => Console.WriteLine(b.AudioSignalProblem));
            //_recognitionEngine.SetInputToWaveStream(s);
            _recognitionEngine.SetInputToAudioStream(s,
                new SpeechAudioFormatInfo(
                44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));
            Console.WriteLine(3);
            _recognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
            Console.WriteLine(4);
            //r.Initialize(s);
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(new Choices("left", "right", "up", "down"));
            _recognitionEngine.UnloadAllGrammars();
            _recognitionEngine.LoadGrammarAsync(new Grammar(grammarBuilder));
            Console.WriteLine(1);
            _recognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            Console.WriteLine("Initialized");

            void waveSource_DataAvailable(object sender, WaveInEventArgs e)
            {
                 s.Write(e.Buffer, 0, e.BytesRecorded);
            }

            void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs args)
            {
                Console.WriteLine(args.Result.Text);
            }
        }*/
    }
}
