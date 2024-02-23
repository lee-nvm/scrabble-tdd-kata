using System;

namespace Scrabble.Lib.Exceptions
{
    public class NoIntersectionException : Exception
    {
        public static NoIntersectionException Create()
        {
            return new NoIntersectionException();
        }

        private NoIntersectionException()
        {
        }
    }
}
