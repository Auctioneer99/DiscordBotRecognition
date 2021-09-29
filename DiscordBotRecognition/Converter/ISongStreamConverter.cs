﻿using DiscordBotRecognition.Song;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Converter
{
    public interface ISongStreamConverter
    {
        public ConvertSettings Settings { get; }

        Task SetSong(ISong song);

        Task ConvertToPCM(Stream streamOut, CancellationToken token);

        void Reset();
    }
}
