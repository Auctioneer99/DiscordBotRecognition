using DiscordBotRecognition.Song;
using System;
using System.Collections.Generic;

namespace DiscordBotRecognition.AudioPlayer.Queue
{
    public class LoopQueue : ISongQueue
    {
        public EQueueType Type => EQueueType.Loop;

        private List<ISong> _internalQueue;
        private int _index;
        private int _maxSize;

        public LoopQueue(int maxSize)
        {
            _maxSize = maxSize;
            _internalQueue = new List<ISong>(_maxSize);
        }

        public LoopQueue(List<ISong> songs, int maxSize)
        {
            _maxSize = maxSize;
            _internalQueue = songs;
        }

        public ISong Current { get; private set; }

        public IEnumerable<ISong> GetQueueList()
        {
            int i = 0;
            foreach(var s in _internalQueue)
            {
                if (i >= _index)
                {
                    yield return s;
                }
                i++;
            }
            i = 0;
            foreach (var s in _internalQueue)
            {
                if (i < _index)
                {
                    yield return s;
                }
                i++;
            }
        }

        public bool TryGetNextSong(out ISong song)
        {
            if (_internalQueue.Count == 0)
            {
                song = null;
                return false;
            }
            if (_internalQueue.Count <= _index)
            {
                _index = 0;
            }
            song = _internalQueue[_index];
            Current = song;
            _index++;
            return true;
        }

        public void AddSong(ISong song)
        {
            if (_maxSize > _internalQueue.Count)
            {
                //chached strategy
                _internalQueue.Add(song);
            }
            else
            {
                throw new Exception($"Queue limit reached ({_maxSize}), song not added");
            }
        }

        public ISongQueue Convert(EQueueType type)
        {
            switch(type)
            {
                case EQueueType.FIFO:
                    var songs = new List<ISong>(_maxSize);
                    int i = 0;
                    foreach (var s in _internalQueue)
                    {
                        if (i >= _index)
                        {
                            songs.Add(s);
                        }
                        i++;
                    }
                    return new FIFOQueue(songs, _maxSize);
                case EQueueType.Loop:
                    return this;
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
            int count = _internalQueue.Count;
            if (id < count)
            {
                int i = (id + _index) % count;
                if (i <= _index)
                {
                    _index--;
                }
                song = _internalQueue[i];
                _internalQueue.RemoveAt(i);
                return true;
            }
            song = null;
            return false;
        }
    }
}
