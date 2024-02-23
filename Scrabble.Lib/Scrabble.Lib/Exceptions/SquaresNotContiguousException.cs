using System;

namespace Scrabble.Lib.Exceptions
{
    public class SquaresNotContiguousException : Exception
    {
        public static SquaresNotContiguousException Create()
        {
            return new SquaresNotContiguousException();
        }

        private SquaresNotContiguousException()
        {
        }
    }
}
