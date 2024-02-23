using System;

namespace Scrabble.Lib.Test
{
    public class NonRandomRandom : Random
    {
        private static NonRandomRandom _instance;
        public static NonRandomRandom Instance
        {
            get { return _instance ?? (_instance = new NonRandomRandom()); }
        }

        private NonRandomRandom()
        {
        }

        public override int Next(int maxValue)
        {
            return 0;
        }
    }
}