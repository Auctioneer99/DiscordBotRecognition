using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotRecognitionCore.Recognition
{
    public class RecognitionSettings
    {
        public int MaxListeners { get; private set; }

        public static RecognitionSettings Default()
        {
            return new RecognitionSettings() { MaxListeners = 1 };
        }
    }
}
