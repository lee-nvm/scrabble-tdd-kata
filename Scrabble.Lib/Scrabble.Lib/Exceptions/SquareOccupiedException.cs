using System;
using System.Collections.Generic;

namespace Scrabble.Lib.Exceptions
{
    public class SquareOccupiedException : Exception
    {
        public static SquareOccupiedException Create(IEnumerable<Square> squares)
        {
            return new SquareOccupiedException(squares);
        }

        private SquareOccupiedException(IEnumerable<Square> squares)
        {
            Squares = squares;
        }

        public IEnumerable<Square> Squares { get; private set; }
    }
}