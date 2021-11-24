using DiscordBotRecognition.Recognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognitionCore.Recognition.Recognizers
{
    public interface IRecognizerFactory
    {
        IRecognizer Create();
    }

    public class RecognizerFactory : IRecognizerFactory
    {
        public IRecognizer Create()
        {
            return new Recognizer();
        }
    }
}
