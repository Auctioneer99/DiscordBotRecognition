using DiscordBotRecognition.Converter.Settings;
using DiscordBotRecognition.Songs;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Converter
{
    public interface ISongStreamConverter : IDisposable
    {
        public ConvertSettings Settings { get; }

        void SetSong(ISong song);

        Task ConvertToPCM(Stream streamOut, CancellationToken token);

        void Reset();
    }
}
