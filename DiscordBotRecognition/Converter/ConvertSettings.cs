using System;

namespace DiscordBotRecognition.Converter
{
    public class ConvertSettings
    {
        public int Bass
        {
            get => _bass;
            set
            {
                if (value > 16 || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _bass = value;
            }
        }
        private int _bass = 0;

        public int Treble
        {
            get => _treble;
            set
            {
                if (value > 16 || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _treble = value;
            }
        }
        private int _treble;
    }
}
