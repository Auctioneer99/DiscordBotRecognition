using DiscordBotRecognition.Song;
using FFMpegCore;
using FFMpegCore.Pipes;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Converter
{
    public class FFmpegConverter : ISongStreamConverter
    {
        public const string FORMAT = "s16le";

        public async Task ConvertToPCM(ISong song, Stream streamOut, CancellationToken token)
        {
            var streamIn = await song.GetStream();
            var source = new StreamPipeSource(streamIn.Stream);
            var sink = new StreamPipeSink(streamOut);
            try
            {
                await FFMpegArguments
                    .FromPipeInput(source)
                    .OutputToPipe(sink, options => options
                        .ForceFormat(FORMAT)
                        )
                    .ProcessAsynchronously();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on converting");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await streamOut.FlushAsync();
            }
        }

        public Task ConvertToPCM(Stream streamOut, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public Task SetSong(ISong song)
        {
            throw new NotImplementedException();
        }
    }
}
