using DiscordBotRecognition.Alive;
using DiscordBotRecognition.Converter.Settings;
using DiscordBotRecognition.Songs;
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
        private Process _ffmpeg;
        private AliveChecker _checker;
        private bool _disposed;

        public ConvertSettings Settings { get; private set; }

        public FFmpegConverter(AliveChecker checker)
        {
            Settings = new ConvertSettings();
            _checker = checker;
        }

        public async Task ConvertToPCM(Stream streamOut, CancellationToken token)
        {
            try
            {
                int read;
                var buffer = new byte[32 * 1024];
                while ((read = await _ffmpeg.StandardOutput.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    streamOut.Write(buffer, 0, read);
                    _checker.Update();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await streamOut.FlushAsync();
                _checker.Update();
            }
        }

        public void Reset()
        {
            _song = null;
            try
            {
                _ffmpeg?.Kill();
            }
            catch { }
            _ffmpeg = null;
        }

        public void SetSong(ISong song)
        {
            _song = song;
            _ffmpeg = CreateProcess(_song.StreamUrl);
        }

        private Process CreateProcess(string inputUrl)
        {
            Console.WriteLine("creating");
            Console.WriteLine(Directory.GetCurrentDirectory());
            string webArgs = "-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5";
            bool shouldAddWebArgs = inputUrl.StartsWith("http");
            string additionalArgs = shouldAddWebArgs ? webArgs : "";
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"{additionalArgs} -loglevel warning -copyts -err_detect ignore_err -i \"{inputUrl}\" -f s16le -ac 2 -af \"atempo={Settings.Speed.Volume},firequalizer=gain_entry='entry(0,{Settings.Bass});entry(250,{(int)(Settings.Bass/4)});entry(1000,0);entry(4000,{(int)Settings.Treble/4});entry(16000,{Settings.Treble})'\" -ar {Settings.Speed.Hz} -copy_unknown -sn -dn -ignore_unknown pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
            return ffmpeg;
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
                _ffmpeg?.Kill();
                _checker.Dispose();
                _disposed = true;
            }
        }

        ~FFmpegConverter()
        {
            Dispose(false);
        }
    }
}
