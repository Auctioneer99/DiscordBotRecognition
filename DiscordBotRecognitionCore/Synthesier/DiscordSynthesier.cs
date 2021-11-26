using DiscordBotRecognition.AudioPlayer.AudioClient;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace DiscordBotRecognitionCore.Synthesier
{
    public class DiscordSynthesier : ASynthesier
    {
        private SpeechSynthesizer _synthesier = new SpeechSynthesizer();
        //Microsoft Irina Desktop
        //Microsoft Haruka Desktop
        //Microsoft Zira Desktop

        public DiscordSynthesier(IAudioClient group) : base(group)
        {
            var voices = _synthesier.GetInstalledVoices();
            _synthesier.SelectVoice(voices.FirstOrDefault(v => v.VoiceInfo.Culture.Name == "ru-RU")?.VoiceInfo.Name ?? "");
        }

        protected override Stream Convert(string text)
        {
            Stream stream = new MemoryStream();
            _synthesier.SetOutputToAudioStream(stream, new SpeechAudioFormatInfo(48000, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
            _synthesier.Speak(text);
            stream.Position = 0;
            return stream;
        }
    }
}
