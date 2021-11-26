using DiscordBotRecognition.AudioPlayer.AudioClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Synthesier
{
    public class DiscordSynthesier : ISynthesier
    {
        private SpeechSynthesizer _synthesier = new SpeechSynthesizer();
        //Microsoft Irina Desktop
        //Microsoft Haruka Desktop
        //Microsoft Zira Desktop
        private IAudioClient _group;

        public DiscordSynthesier(IAudioClient group)
        {
            _group = group;
            _synthesier.SelectVoice("Microsoft Irina Desktop");
        }

        public async Task Speak(string text)
        {
            using (Stream stream = new MemoryStream())
            {
                _synthesier.SetOutputToAudioStream(stream, new SpeechAudioFormatInfo(48000, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
                _synthesier.Speak(text);
                try
                {
                    stream.Position = 0;
                    await stream.CopyToAsync(_group.GetPCMStream());
                }
                finally
                {
                    await _group.GetPCMStream().FlushAsync();
                }
            }
        }
    }
}
