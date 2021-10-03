using DiscordBotRecognition.Songs;
using System;
using System.Collections.Generic;

namespace DiscordBotRecognition.AudioPlayer.Queue
{
    public class FIFOQueue : ISongQueue
    {
        public EQueueType Type => EQueueType.FIFO;

        private List<ISong> _internalQueue;
        private int _maxSize;

        public FIFOQueue(int maxSize)
        {
            _maxSize = maxSize;
            _internalQueue = new List<ISong>(_maxSize);
        }

        public FIFOQueue(List<ISong> queue, int maxSize)
        {
            _maxSize = maxSize;
            _internalQueue = queue;
        }

        public ISong Current { get; private set; }


        public IEnumerable<ISong> GetQueueList()
        {
            foreach (var s in _internalQueue)
            {
                yield return s;
            }
        }

        public bool TryGetNextSong(out ISong song)
        {
            if (_internalQueue.Count == 0)
            {
                song = null;
                return false;
            }
            song = _internalQueue[0];
            _internalQueue.RemoveAt(0);
            Current = song;
            return true;
        }

        public void AddSong(ISong song)
        {
            if (_maxSize > _internalQueue.Count)
            {
                _internalQueue.Add(song);
            }
            else
            {
                throw new Exception($"Queue limit reached ({_maxSize}), song not added");
            }
        }

        public ISongQueue Convert(EQueueType type)
        {
            switch (type)
            {
                case EQueueType.FIFO:
                    return this;
                case EQueueType.Loop:
                    var songs = new List<ISong>(_maxSize);
                    songs.AddRange(_internalQueue);
                    if (Current != null)
                    {
                        songs.Add(Current);
                    }
                    return new LoopQueue(songs, _maxSize);
                default:
                    throw new NotImplementedException();
            }
        }

        public void Clear()
        {
            _internalQueue = new List<ISong>(_maxSize);
            Current = null;
        }

        public bool TryRemove(int id, out ISong song)
        {
            if (id < _internalQueue.Count)
            {
                song = _internalQueue[id];
                _internalQueue.RemoveAt(id);
                return true;
            }
            song = null;
            return false;
        }
    }
}
