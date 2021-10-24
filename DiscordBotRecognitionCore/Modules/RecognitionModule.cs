using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognition.Modules
{
    public class RecognitionModule : ModuleBase<SocketCommandContext>
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
