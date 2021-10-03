using System;

namespace DiscordBotRecognition.Songs
{
    public interface ISong
    {
        public string Id { get; }
        public string Name { get; }
        public TimeSpan Duration { get; }
        public string StreamUrl { get; }
    }
}
