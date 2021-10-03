using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DiscordBotRecognition.Converter.Settings;
using DiscordBotRecognition.Songs;
using NAudio.Wave;

namespace DiscordBotRecognition.Converter
{
    public class NAudioConverter : ISongStreamConverter
    {
        private MediaFoundationReader _reader;
        private WaveStream _pcmStream;
        private ISong _song;

        public ConvertSettings Settings => throw new NotImplementedException();

        public void SetSong(ISong song)
        {
            _song = song;
            _reader = new MediaFoundationReader(song.StreamUrl);
            _pcmStream = WaveFormatConversionStream.CreatePcmStream(_reader);
        }

        public void Reset()
        {
            _pcmStream?.Close();
            _reader?.Close();
            _song = null;
        }

        public async Task ConvertToPCM(Stream streamOut, CancellationToken token)
        {
            try
            {
                await _pcmStream.CopyToAsync(streamOut, token);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await streamOut.FlushAsync();
            }
        }
    }
}
