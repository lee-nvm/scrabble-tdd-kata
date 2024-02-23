using System;
using System.Collections.Generic;

namespace Scrabble.Lib.Exceptions
{
    public class PointsNotInALineException : Exception
    {
        public static PointsNotInALineException Create(IEnumerable<Point> enumerable)
        {
            return new PointsNotInALineException(enumerable);
        }

        private PointsNotInALineException(IEnumerable<Point> points)
        {
            Points = points;
        }

        public IEnumerable<Point> Points { get; private set; }
    }
}