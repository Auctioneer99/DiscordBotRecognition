using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognition.Recognition
{
    public class RecognitionGroup
    {
        private IRecognizer _recognizer;

        public RecognitionGroup(IRecognizer recognizer)
        {
            _recognizer = recognizer;
        }


    }
}
