using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBotRecognition.MusicSearch;
using DiscordBotRecognition.Modules;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.Credentials;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Recognition;
using NAudio.Wave;
using System.IO;
using System;
using System.Speech.Recognition;
using System.Linq;

namespace DiscordBotRecognition
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        void test(object o, WaveInEventArgs args)
        {
            speechStream.Write(args.Buffer, 0, args.BytesRecorded);
        }

        MemoryStream buffer = new MemoryStream();

        static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Recognized text: " + e.Result.Text);
        }

        public void asd(object o, AudioStateChangedEventArgs arg)
        {
            Console.WriteLine(arg.AudioState.ToString());
        }

        public void asdf(object o, SpeechDetectedEventArgs arg)
        {
            Console.WriteLine("speech");
        }

        SpeechStream speechStream;

        public async Task MainAsync()
        {
            /*
            

            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(SpeechRecognitionEngine.InstalledRecognizers().First()))
            {
                
                WaveInEvent waveIn = new WaveInEvent();
                waveIn.DeviceNumber = 0;
                waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(test);
                waveIn.StartRecording();


                recognizer.LoadGrammar(new DictationGrammar());
                recognizer.AudioStateChanged += asd;
                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
                Console.WriteLine(waveIn.WaveFormat.SampleRate);
                Console.WriteLine(waveIn.WaveFormat.BitsPerSample);
                Console.WriteLine(waveIn.WaveFormat.Channels);
                recognizer.SpeechDetected += asdf;

                speechStream = new SpeechStream(16 * 1024);


                recognizer.SetInputToAudioStream(speechStream,
                    new System.Speech.AudioFormat
                    .SpeechAudioFormatInfo(waveIn.WaveFormat.SampleRate,
                    System.Speech.AudioFormat.AudioBitsPerSample.Sixteen,
                    System.Speech.AudioFormat.AudioChannel.Mono));

                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                Console.WriteLine("asd");
                while (true)
                {
                }

            }
            return;

            Recognizer r = new Recognizer();


            /*
             * writer = new WaveFileWriter(@"G:\VisualStudio Projects\DiscordBotRecognition\DiscordBotRecognition\bin\Debug\netcoreapp3.1\temp\123.wav", new WaveFormat(44100, 1));
            WaveInEvent sourceStream = null;

            sourceStream = new WaveInEvent();

            sourceStream.DataAvailable += new EventHandler<WaveInEventArgs>(test);

            sourceStream.DeviceNumber = 0;
            sourceStream.WaveFormat = new WaveFormat(16000, 2);

            sourceStream.StartRecording();

            Console.ReadKey();
            sourceStream.StopRecording();
            writer.Flush();

            return;
            */


            DiscordSocketClient client = new DiscordSocketClient();
            CommandService commands = new CommandService();
            IMusicSearcher searcher = new YouTubeSearcher(Credential.GoogleAPIToken);
            //ISongStreamConverter converter = new NAudioConverter();
            AudioService audio = new AudioService();

            ServiceProvider provider = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(audio)
                .AddSingleton(searcher)
                .AddTransient<ISongStreamConverter>((service) => new FFmpegConverter())//NAudioConverter())
                .BuildServiceProvider();

            CommandHandler handler = new CommandHandler(client, commands, provider);
            await handler.InitializeAsync();

            string token = Credential.DiscordToken;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
