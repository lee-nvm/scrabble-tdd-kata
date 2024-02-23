using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Scrabble.Lib
{
    public class Square
    {
        public Square(SquareType type, Point point)
        {
            Type = type;
            Point = point;
            State = Vacant.Instance;
        }

        public static IEnumerable<Square> FromPattern(string typePattern)
        {
            return typePattern.Split(',').Select((t, i) => new Square(SquareType.FromDescription(t), Board.GetPoint(i)));
        }

        public Point Point { get; set; }
        public SquareType Type { get; set; }

        internal string HtmlColour
        {
            get
            {
                switch (Type.ToString())
                {
                    case "C":
                    case "DW":
                        return "pink";
                    case "DL":
                        return "lightblue";
                    case "TL":
                        return "blue";
                    case "TW":
                        return "red";
                    default:
                        return "white";
                }
            }
        }
        public SquareState State { get; set; }

        public override string ToString()
        {
            return $"{(State is Occupied x ? x.Tile.Letter.ToString(CultureInfo.InvariantCulture) : "(vacant)")} {Point} {Type} ";
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
            return obj.GetType() == GetType() && Equals((Square)obj);
        }

        protected bool Equals(Square other)
        {
            return Equals(Point, other.Point) && Equals(Type, other.Type);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Point.GetHashCode() * 397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }
}
