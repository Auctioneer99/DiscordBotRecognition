using DiscordBotRecognition.AudioPlayer.AudioClient;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Synthesier
{
    public abstract class ASynthesier
    {
        private IAudioClient _group;

        public ASynthesier(IAudioClient client)
        {
            _group = client;
        }

        public async Task Speak(string text)
        {
            using (var stream = Convert(text))
            {
                try
                {
                    await stream.CopyToAsync(_group.GetPCMStream());
                }
                finally
                {
                    await _group.GetPCMStream().FlushAsync();
                }
            }
        }
        protected abstract Stream Convert(string text);
    }
}
