using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBotRecognitionCore.Synthesier
{
    public class NullSynthesier : ASynthesier
    {
        public NullSynthesier() : base(null)
        {

        }

        protected override Stream Convert(string text)
        {
            return new MemoryStream();
        }
    }
}
