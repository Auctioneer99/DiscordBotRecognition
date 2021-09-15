using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DiscordBotRecognition.Song;
using NAudio.Wave;

namespace DiscordBotRecognition.Converter
{
    public class NAudioConverter : ISongStreamConverter
    {
        public async Task ConvertToPCM(ISong song, Stream streamOut, CancellationToken token)
        {
            try
            {
                var streamUrl = await song.GetStreamUrl();
                using (MediaFoundationReader reader = new MediaFoundationReader(streamUrl))
                {
                    using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                    {
                        await pcmStream.CopyToAsync(streamOut, token);
                    }
                }
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
