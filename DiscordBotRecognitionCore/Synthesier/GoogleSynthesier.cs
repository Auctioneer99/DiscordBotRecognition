using System.IO;
using DiscordBotRecognition.AudioPlayer.AudioClient;
using Google.Apis.Texttospeech.v1;
using Google.Apis.Texttospeech.v1.Data;
using Google.Apis.Services;
using System.Text;

namespace DiscordBotRecognitionCore.Synthesier
{
    public class GoogleSynthesier : ASynthesier
    {
        private TexttospeechService _service;
        private TextResource _resource;

        public GoogleSynthesier(IAudioClient group, BaseClientService.Initializer initializer) : base(group)
        {
            _service = new TexttospeechService(initializer);
            _resource = new TextResource(_service);
        }

        protected override Stream Convert(string text)
        {
            SynthesizeSpeechRequest body = new SynthesizeSpeechRequest()
            { 
                AudioConfig = new AudioConfig()
                {
                    AudioEncoding = "Linear16",
                },
                Input = new SynthesisInput()
                {
                    Text = text
                },
                Voice = new VoiceSelectionParams()
                {
                    LanguageCode = "ru-RU"
                }
            };
            var request = _resource.Synthesize(body);
            return request.ExecuteAsStream();
        }
    }
}
