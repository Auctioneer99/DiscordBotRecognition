using System.IO;

namespace DiscordBotRecognition.Song
{
    public struct SongStream
    {
        public string Format { get; private set; }
        public int Bitrate { get; private set; }
        public Stream Stream { get; private set; }

        public SongStream(string format, int bitrate, Stream stream)
        {
            Format = format;
            Bitrate = bitrate;
            Stream = stream;
        }
    }
}
