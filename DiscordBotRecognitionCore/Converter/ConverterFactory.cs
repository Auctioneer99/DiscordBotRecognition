using DiscordBotRecognition.Converter;

namespace DiscordBotRecognitionCore.Converter
{
    public interface IConverterFactory
    {
        public ISongStreamConverter Get();
    }

    public class FFmpegConverterFactory : IConverterFactory
    {
        private string _executable;

        public FFmpegConverterFactory(string executionPath)
        {
            _executable = executionPath;
        }

        public ISongStreamConverter Get()
        {
            return new FFmpegConverter(_executable);
        }
    }
}
