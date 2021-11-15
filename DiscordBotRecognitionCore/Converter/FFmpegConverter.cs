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
        private bool _disposed;
        private string _executable;

        public ConvertSettings Settings { get; private set; }

        public FFmpegConverter(string executionPath)
        {
            Settings = new ConvertSettings();
            _executable = executionPath;
        }

        public async Task ConvertToPCM(Stream streamOut, CancellationToken token)
        {
            try
            {
                Console.WriteLine("Converting ffmpeg");
                await _ffmpeg.StandardOutput.BaseStream.CopyToAsync(streamOut, token);
                //int read;
                //var buffer = new byte[32 * 1024];
                //while ((read = await _ffmpeg.StandardOutput.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                //{
                //    streamOut.Write(buffer, 0, read);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await streamOut.FlushAsync();
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
            string webArgs = "-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5";
            bool shouldAddWebArgs = inputUrl.StartsWith("http");
            string additionalArgs = shouldAddWebArgs ? webArgs : "";

            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = _executable,
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
                _disposed = true;
            }
        }

        ~FFmpegConverter()
        {
            Dispose(false);
        }
    }
}
