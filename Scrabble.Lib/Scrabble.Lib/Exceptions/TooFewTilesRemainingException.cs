using System;

namespace Scrabble.Lib.Exceptions
{
    public class TooFewTilesRemainingException : Exception
    {
        public static TooFewTilesRemainingException Create()
        {
            return new TooFewTilesRemainingException();
        }

        private TooFewTilesRemainingException()
            : base("You can't swap tiles, there are fewer than 7 tiles remaining in the bag")
        {
        }
    }
}
