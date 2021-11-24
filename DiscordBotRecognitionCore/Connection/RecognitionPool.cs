using DiscordBotRecognition.Recognition;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Connection
{
    public class RecognitionPool
    {
        private readonly ConcurrentDictionary<ulong, RecognitionGroup> ConnectedChannels = new ConcurrentDictionary<ulong, RecognitionGroup>();

        public bool IsConnected(ulong id)
        {
            return ConnectedChannels.ContainsKey(id);
        }

        public async Task<bool> TryJoin(ulong id, RecognitionGroup group)
        {
            if (ConnectedChannels.TryAdd(id, group))
            {
                group.Me.Disconnected += () => Leave(id);
                return true;
            }
            return false;
        }

        public bool Leave(ulong id)
        {
            if (ConnectedChannels.TryRemove(id, out var group))
            {
                //group.DisposeAsync();
                return true;
            }
            return false;
        }

        public bool TryGetConnection(ulong id, out RecognitionGroup group)
        {
            return ConnectedChannels.TryGetValue(id, out group);
        }
    }
}
