using DiscordBotRecognition.Song;
using System.Collections.Generic;

namespace DiscordBotRecognition.AudioPlayer.Queue
{
    public interface ISongQueue
    {
        EQueueType Type { get; }

        ISong Current { get; }

        IEnumerable<ISong> GetQueueList();

        bool TryGetNextSong(out ISong song);

        void AddSong(ISong song);

        ISongQueue Convert(EQueueType type);

        void Clear();

        bool TryRemove(int id, out ISong song);
    }
}
