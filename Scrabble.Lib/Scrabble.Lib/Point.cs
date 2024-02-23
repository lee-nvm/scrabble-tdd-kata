using System.Globalization;

namespace Scrabble.Lib
{
    public struct Point
    {
        public static Point Create(string gridReference)
        {
            return new Point(gridReference);
        }

        public static Point Create(char x, int y)
        {
            return new Point(x, y);
        }

        private Point(string gridReference)
            : this()
        {
            X = char.Parse(gridReference.Substring(0, 1));
            Y = int.Parse(gridReference.Substring(1));
        }

        private Point(char x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        public char X { get; private set; }
        public int Y { get; private set; }

        public override string ToString()
        {
            return X.ToString(CultureInfo.InvariantCulture) + Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y;
            }
        }
    }
}
