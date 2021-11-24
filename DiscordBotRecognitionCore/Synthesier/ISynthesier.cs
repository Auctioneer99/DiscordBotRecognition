using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotRecognitionCore.Synthesier
{
    public interface ISynthesier
    {
        Task Speak(string text);
    }
}
