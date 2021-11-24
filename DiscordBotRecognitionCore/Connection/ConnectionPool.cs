using DiscordBotRecognition.AudioPlayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Connection
{
    public class ConnectionPool
    {
        public event Action<AudioGroup> Added;
        public event Action<AudioGroup> Removed;
        public event Action<AudioGroup> Get;

        public IEnumerable<KeyValuePair<ulong, AudioGroup>> AudioGroups => ConnectedChannels.AsEnumerable();

        private readonly ConcurrentDictionary<ulong, AudioGroup> ConnectedChannels = new ConcurrentDictionary<ulong, AudioGroup>();

        public bool IsConnected(ulong id)
        {
            return ConnectedChannels.ContainsKey(id);
        }

        public async Task<bool> TryJoin(ulong id, AudioGroup group)
        {
            if (ConnectedChannels.TryAdd(id, group))
            {
                group.Me.Disconnected += () => Leave(id);
                Added?.Invoke(group);
                return true;
            }
            return false;
        }

        public bool Leave(ulong id)
        {
            if (ConnectedChannels.TryRemove(id, out var group))
            {
                Removed?.Invoke(group);
                group.DisposeAsync();
                return true;
            }
            return false;
        }

        public bool TryGetConnection(ulong id, out AudioGroup group)
        {
            if (ConnectedChannels.TryGetValue(id, out group))
            {
                Get?.Invoke(group);
                return true;
            }
            return false;
        }
    }
}
