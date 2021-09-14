using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace DiscordBotRecognition.Song.Converter
{
    public class NAudioConverter : ISongStreamConverter
    {
        public async Task ConvertToPCM(SongStream streamIn, Stream streamOut)
        {
            Console.WriteLine(11);
            try
            {
                Console.WriteLine(12);
                using (StreamMediaFoundationReader reader = new StreamMediaFoundationReader(streamIn.Stream))
                {
                    Console.WriteLine(13);
                    using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                    {
                        Console.WriteLine(14);
                        WaveFileWriter.CreateWaveFile(@"G:\VisualStudio Projects\DiscordBotRecognition\DiscordBotRecognition\bin\Debug\netcoreapp3.1\temp\something1.wav", pcmStream);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
