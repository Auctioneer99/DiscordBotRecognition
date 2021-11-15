using Discord;
using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.Recognition;
using DiscordBotRecognitionCore.Connection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Modules
{
    public class RecognitionModule : AModuleBase
    {
        [Command("listen")]
        public async Task ToggleListen(bool listen)
        {
            if (listen)
            {
                
            }
            else
            {

            }
            //await _service.JoinAudio(Context.Guild.Id, (Context.User as IVoiceState).VoiceChannel);
        }
    }
}
