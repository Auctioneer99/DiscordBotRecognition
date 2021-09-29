using DiscordBotRecognition.Song;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Converter
{
    public class FFmpegConverter : ISongStreamConverter
    {
        public const string FORMAT = "s16le";

        private ISong _song;
        private SongStream _stream;
        private Process _ffmpeg;
        private CancellationTokenSource _songEndSource;
        private CancellationTokenSource _linkedTokenSource;

        public ConvertSettings Settings { get; private set; }

        public FFmpegConverter()
        {
            Settings = new ConvertSettings();
        }

        public async Task ConvertToPCM(Stream streamOut, CancellationToken token)
        {
            var pipe1 = _ffmpeg.StandardOutput.BaseStream;

            _songEndSource = new CancellationTokenSource();
            _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_songEndSource.Token, token);

            try
            {
                await pipe1.CopyToAsync(streamOut, _linkedTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await streamOut.FlushAsync();
            }
            Console.WriteLine("Streaming has ended");
        }

        public void Reset()
        {
            _stream.Stream.Close();
            _song = null;
            try
            {
                _ffmpeg?.Kill();
            }
            catch { }
            _ffmpeg = null;
        }

        public async Task SetSong(ISong song)
        {
            _song = song;
            _stream = await song.GetStream();
            _ffmpeg = CreateProcess();
            _stream.Stream.CopyToAsync(_ffmpeg.StandardInput.BaseStream)
                .ContinueWith((_, __) =>
                {
                    _songEndSource.Cancel();
                }, null);
        }

        private Process CreateProcess()
        {
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-re -i pipe:0 -f s16le -ac 2 -af \"firequalizer=gain_entry='entry(0,{Settings.Bass});entry(250,{(int)(Settings.Bass/4)});entry(1000,0);entry(4000,{(int)Settings.Treble/4});entry(16000,{Settings.Treble})'\" -ar 48000 pipe:1",
                //Arguments = $"-ss {SSText} -re -i pipe:0 -f s16le -ac 2 -af \"{Filter.Tag}\" -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            ffmpeg.BeginErrorReadLine();
            return ffmpeg;
        }
    }
}
