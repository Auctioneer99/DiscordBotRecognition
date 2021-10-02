using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.AudioPlayer.Queue;
using DiscordBotRecognition.Cache;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Converter.Settings;
using DiscordBotRecognition.Song;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, AudioGroup> ConnectedChannels = new ConcurrentDictionary<ulong, AudioGroup>();
        private CacheStorage _cacheStorage;

        public AudioService(CacheStorage CacheStorage)
        {
            _cacheStorage = CacheStorage;
        }

        public bool IsConnected(ulong id)
        {
            return ConnectedChannels.TryGetValue(id, out var junk);
        }

        public async Task<bool> TryJoinAudio(ulong id, IAudioClient client, ISongStreamConverter streamConverter)
        {
            AudioGroup group = new AudioGroup(client, streamConverter, AudioGroupSettings.Default());
            if (ConnectedChannels.TryAdd(id, group))
            {
                client.Disconnected += async () => await LeaveAudio(id);
                return true;
            }
            else
            {
                await group.DisposeAsync();
                return false;
            }
        }

        public async Task LeaveAudio(ulong id)
        {
            if (ConnectedChannels.TryRemove(id, out var group))
            {
                await group.DisposeAsync();
            }
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
                CachedSong cached = new CachedSong(song, _cacheStorage);
                if (await cached.IsLocal())
                {
                    await cached.CacheToLocalSystem();
                }
                else
                {
                    cached.CacheToLocalSystem();
                }
                group.Queue.AddSong(cached);
                await group.Play(false);
            }
        }

        public async Task Play(ulong id, bool isResuming)
        {
            if (CheckConnection(id, out var group))
            {
                await group.Play(isResuming);
            }
        }

        public void PauseSong(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                group.Converter.Pause();
            }
        }

        public void SetSpeed(ulong id)
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
            if (ConnectedChannels.TryGetValue(id, out group))
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