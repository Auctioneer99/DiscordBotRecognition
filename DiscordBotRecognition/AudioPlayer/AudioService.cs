﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.Converter;
using DiscordBotRecognition.Converter.Settings;
using DiscordBotRecognition.Song;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, AudioGroup> ConnectedChannels = new ConcurrentDictionary<ulong, AudioGroup>();

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
                group.AppendSong(song);
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

        public ISong SkipSong(ulong id, int index = 0)
        {
            if (CheckConnection(id, out var group))
            {
                return group.SkipSong(index);
            }
            return null;
        }

        public List<ISong> GetSongList(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                return group.QueuedSongs;
            }
            return null;
        }

        public ISong GetCurrentSong(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                return group.Current;
            }
            return null;
        }

        public void SetBass(int volume, ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                group.Converter.Settings.Bass = volume;
            }
        }

        public ConvertSettings GetConvertingInfo(ulong id)
        {
            if (CheckConnection(id, out var group))
            {
                return group.Converter.Settings;
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