using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace DiscordBotRecognition.Song.Converter
{
    public class NAudioConverter : ISongStreamConverter
    {
        public async Task ConvertToPCM(ISong song, Stream streamOut)
        {
            try
            {
                var streamUrl = await song.GetStreamUrl();
                using (MediaFoundationReader reader = new MediaFoundationReader(streamUrl))
                {
                    using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                    {
                        await pcmStream.CopyToAsync(streamOut);
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
