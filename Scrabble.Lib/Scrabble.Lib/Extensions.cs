using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    public static class Extensions
    {
        public static Orientation GetOrientation(this IEnumerable<Square> squares)
        {
            var orientation = Orientation.None;

            var squaresList = squares as IList<Square> ?? squares.ToList();

            if (squaresList.Count() == 1)
            {
                return orientation;
            }

            if (squaresList.GroupBy(s => s.Point.X).Count() == 1)
            {
                orientation |= Orientation.Vertical;
            }

            if (squaresList.GroupBy(s => s.Point.Y).Count() == 1)
            {
                orientation |= Orientation.Horizontal;
            }

            return orientation;
        }
    }
}
