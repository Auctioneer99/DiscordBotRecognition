using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBotRecognition.AudioPlayer.Queue;
using DiscordBotRecognition.Converter.Settings;
using DiscordBotRecognition.Songs;
using DiscordBotRecognitionCore.Connection;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioService
    {
        public ConnectionPool ConnectionPool { get; private set; }

        public AudioService(ConnectionPool connectionPool)
        {
            ConnectionPool = connectionPool;
        }

        public bool IsPaused(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                return group.Converter.Paused;
            }
            return false;
        }

        public async Task AddSong(ulong id, ISong song)
        {
            if (CheckConnection(id, out var group))
            {
                //CachedSong cached = _cacheStorage.GetCachedFile(song);
                group.Queue.AddSong(song);
                //group.Play(false);
            }
        }

        public async Task Play(ulong id, bool isResuming)
        {
            if (CheckConnection(id, out var group))
            {
               // await group.Play(isResuming);
            }
        }

        public void PauseSong(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                group.Converter.Pause();
            }
        }

        public ISong SkipSong(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                return group.SkipSong();
            }
            return null;
        }

        public ISong RemoveSong(ulong id, int index)
        {
            if (CheckConnection(id, out var group))
            {
                if (group.Queue.TryRemove(index, out var song))
                {
                    return song;
                }
            }
            return null;
        }

        public List<ISong> GetSongList(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                return group.Queue.GetQueueList().ToList();
            }
            return null;
        }

        public ISong GetCurrentSong(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                return group.Queue.Current;
            }
            return null;
        }

        public void SetLooping(ulong id, EQueueType type)
        {
            if (CheckConnection(id, out var group))
            {
                group.SetQueueType(type);
            }
        }

        public void SetBass(int volume, ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                group.Converter.Settings.Bass = volume;
            }
        }

        public AudioGroupInfo GetInfo(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                var info = new AudioGroupInfo();
                info.ConvertInfo = group.Converter.Settings;
                info.QueueType = group.Queue.Type;
                return info;
            }
            return null;
        }

        public void Stop(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                group.Stop();
            }
        }

        public void SetSpeed(Speed speed, ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                group.Converter.Settings.Speed = speed;
            }
        }

        private bool CheckConnection(ulong id, out AudioGroup group)
        {
            if (ConnectionPool.TryGetConnection(id, out group))
            {
                return true;
            }
            else
            {
                throw new Exception("I must be in voice channel to perform this task");
            }
        }
    }
}