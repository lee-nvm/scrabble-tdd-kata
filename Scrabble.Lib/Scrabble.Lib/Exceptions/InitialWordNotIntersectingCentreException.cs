using System;

namespace Scrabble.Lib.Exceptions
{
    public class InitialWordNotIntersectingCentreException : Exception
    {
        public static InitialWordNotIntersectingCentreException Create()
        {
            return new InitialWordNotIntersectingCentreException();
        }

        private InitialWordNotIntersectingCentreException()
        {
        }
    }
}
