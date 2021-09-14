using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using DiscordBotRecognition.MusicSearch;
using DiscordBotRecognition.Song;
using DiscordBotRecognition.Song.Converter;

namespace DiscordBotRecognition.AudioPlayer
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, AudioGroup> ConnectedChannels = new ConcurrentDictionary<ulong, AudioGroup>();

        private IMusicSearcher _searcher;
        private ISongStreamConverter _streamConverter;

        public AudioService(IMusicSearcher searcher, ISongStreamConverter converter)
        {
            _searcher = searcher;
            _streamConverter = converter;
        }

        public async Task JoinAudio(ulong id, IVoiceChannel target)
        {
            if (ConnectedChannels.TryGetValue(id, out var client))
            {
                return;
            }
            if (target.Guild.Id != id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            DiscordAudioClient discordClient = new DiscordAudioClient(audioClient);

            AudioGroup group = new AudioGroup(discordClient, _streamConverter);
            ConnectedChannels.TryAdd(id, group);
        }

        public async Task LeaveAudio(ulong id)
        {
            if (ConnectedChannels.TryRemove(id, out var client))
            {
                await client.DisposeAsync();
            }
        }

        public async Task<ISong> AddSong(ulong id, string query)
        {
            if (ConnectedChannels.TryGetValue(id, out var group))
            {
                ISong song = await _searcher.SearchSong(query);
                group.AppendSong(song);
                return song;
            }
            return null;
        }

        public async Task SkipSong(ulong id, int index = 0)
        {
            if (ConnectedChannels.TryGetValue(id, out var group))
            {
                group.SkipSong(index);
            }
        }

        public List<ISong> GetSongList(ulong id)
        {
            if (ConnectedChannels.TryGetValue(id, out var group))
            {
                return group.QueuedSongs;
            }
            return null;
        }

        public ISong GetCurrentSong(ulong id)
        {
            if (ConnectedChannels.TryGetValue(id, out var group))
            {
                return group.Current;
            }
            return null;
        }
    }
}