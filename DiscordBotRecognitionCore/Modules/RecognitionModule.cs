using Discord;
using Discord.Audio.Streams;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.Recognition;
using DiscordBotRecognitionCore.Connection;
using DiscordBotRecognitionCore.Recognition;
using DiscordBotRecognitionCore.Recognition.Recognizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Modules
{
    public class RecognitionModule : AModuleBase
    {
        public DiscordAudioConnector Connector { get; set; }
        public IRecognizerFactory Factory { get; set; }
        public RecognitionPool RecognitionPool { get; set; }

        [Command("listen", RunMode = RunMode.Async)]
        public async Task Listen()
        {
            if (RecognitionPool.TryGetConnection(Id, out var recognitionGroup) == false)
            {
                if (ConnectionPool.TryGetConnection(Id, out var audioGroup) == false)
                {
                    Connector.Context = Context;
                    audioGroup = await Connector.TryConnect(Id);
                }

                var settings = RecognitionSettings.Default();
                recognitionGroup = new RecognitionGroup(audioGroup, settings);
                if (await RecognitionPool.TryJoin(Id, recognitionGroup) == false)
                {
                    return;
                }
            }

            RecognizableClient client = recognitionGroup.Me.GetListenerStreams().Where(c => c.Id == Context.Message.Author.Id).FirstOrDefault();

            if (client != null)
            {
                if (recognitionGroup.TryAddRecognizableClient(client))
                {
                    client.StartListening();
                    await ReplyAsync("You are being listened from now on");
                }
                else
                {
                    await ReplyAsync("You are already being listened");
                }
            }
            else
            {
                await ReplyAsync("You are not in voice channel");
            }
        }
    }
}
