using Discord.Commands;
using DiscordBotRecognition.AudioPlayer;
using DiscordBotRecognition.AudioPlayer.Queue;
using DiscordBotRecognition.Converter.Settings;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Modules
{
    public class SettingsModule : AModuleBase
    {
        [Command("loop")]
        [Summary("Loops queue, toggle = on/off")]
        public async Task SetLooping(string toggle)
        {
            if (CheckConnection(Id, out var group))
            {
                EQueueType type;
                switch (toggle)
                {
                    case "on":
                        await ReplyAsync("```\nRepeating queue!\n```");
                        type = EQueueType.Loop;
                        break;
                    case "off":
                        type = EQueueType.FIFO;
                        await ReplyAsync("```\nQueue sets to fifo!\n```");
                        break;
                    default:
                        return;
                }
                group.SetQueueType(type);
            }
        }

        [Command("bass")]
        [Summary("Setting bass option")]
        public async Task SetBass(int volume)
        {
            if (CheckConnection(Id, out var group))
            {
                group.Converter.Settings.Bass = volume;
            }
        }

        [Command("speed")]
        [Summary("Setting speed option, volume = slow/nightcore/normal")]
        public async Task SetSpeed(string volume)
        {
            if (CheckConnection(Id, out var group))
            {
                Speed speed;
                switch (volume)
                {
                    case "slow":
                        speed = Speed.Slowed();
                        break;
                    case "nightcore":
                    case "nc":
                        speed = Speed.Nightcore();
                        break;
                    case "normal":
                    default:
                        speed = Speed.Normal();
                        break;
                }
                group.Converter.Settings.Speed = speed;
            }
        }

        [Command("info")]
        [Summary("Get info about audio group")]
        public async Task GetInfo()
        {
            if (CheckConnection(Id, out var group))
            {
                var info = new AudioGroupInfo();
                info.ConvertInfo = group.Converter.Settings;
                info.QueueType = group.Queue.Type;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("```");
                sb.AppendLine($"QUEUE SETTINGS:");
                sb.AppendLine($"QueueType = {info.QueueType.ToString()}");
                sb.AppendLine($"CONVERT SETTINGS:");
                sb.AppendLine($"Bass = {info.ConvertInfo.Bass}");
                sb.AppendLine($"Treble = {info.ConvertInfo.Treble}");
                sb.AppendLine("```");
                await ReplyAsync(sb.ToString());
            }
        }
    }
}
