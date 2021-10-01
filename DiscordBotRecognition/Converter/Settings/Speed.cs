﻿namespace DiscordBotRecognition.Converter.Settings
{
    public class Speed
    {
        public double Volume { get; private set; }
        public int Hz { get; private set; }

        private Speed(double volume, int hz)
        {
            Volume = volume;
            Hz = hz;
        }

        public static Speed Nightcore()
        {
            return new Speed(.8, 35280);
        }

        public static Speed Normal()
        {
            return new Speed(1d, 44100);
        }

        public static Speed Slowed()
        {
            return new Speed(1.25, 55125);
        }
    }
}
