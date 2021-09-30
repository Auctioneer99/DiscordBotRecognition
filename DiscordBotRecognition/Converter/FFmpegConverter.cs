using DiscordBotRecognition.Converter.Settings;
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
        private Process _ffmpeg;

        public ConvertSettings Settings { get; private set; }

        public FFmpegConverter()
        {
            Settings = new ConvertSettings();
        }

        public async Task ConvertToPCM(Stream streamOut, CancellationToken token)
        {
            try
            {
                await _ffmpeg.StandardOutput.BaseStream.CopyToAsync(streamOut, token);
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
            var url = await _song.GetStreamUrl();
            _ffmpeg = CreateProcess(url);
        }

        private Process CreateProcess(string inputUrl)
        {
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel warning -copyts -err_detect ignore_err -i {inputUrl} -f s16le -ac 2 -af \"firequalizer=gain_entry='entry(0,{Settings.Bass});entry(250,{(int)(Settings.Bass/4)});entry(1000,0);entry(4000,{(int)Settings.Treble/4});entry(16000,{Settings.Treble})'\" -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
            return ffmpeg;
        }
    }
}
