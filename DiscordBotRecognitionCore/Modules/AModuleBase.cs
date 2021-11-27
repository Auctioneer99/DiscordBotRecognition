using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognitionCore.Connection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Modules
{
    public class AModuleBase : ModuleBase<SocketCommandContext>
    {
        public ConnectionPool ConnectionPool { get; set; }
        public DiscordAudioConnector AudioConnector { get; set; }

        public ulong Id => Context.Guild.Id;

        public bool CheckConnection(ulong id, out AudioGroup group)
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

        protected async Task<AudioGroup> Connect()
        {
            AudioConnector.Context = Context;
            return await AudioConnector.TryConnect(Id);
        }

        protected async Task SendFormattedMessage(string message)
        {
            await ReplyAsync($"```\n{message}\n```");
        }
    }
}
