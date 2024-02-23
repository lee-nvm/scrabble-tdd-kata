using System;

namespace Scrabble.Lib
{
    public sealed class SquareType
    {
        public static SquareType Normal = new SquareType("N", 1, 1);
        public static SquareType DoubleLetterScore = new SquareType("DL", 2, 1);
        public static SquareType TripleLetterScore = new SquareType("TL", 3, 1);
        public static SquareType DoubleWordScore = new SquareType("DW", 1, 2);
        public static SquareType TripleWordScore = new SquareType("TW", 1, 3);
        public static SquareType Centre = new SquareType("C", 1, 2);

        private readonly string _description;

        public int WordFactor { get; private set; }
        public int LetterFactor { get; private set; }

        private SquareType(string description, int letterFactor, int wordFactor)
        {
            _description = description;
            WordFactor = wordFactor;
            LetterFactor = letterFactor;
        }

        public static SquareType FromDescription(string description)
        {
            switch (description)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(description), description, "");
                case "N":
                case "":
                    return Normal;
                case "DL":
                    return DoubleLetterScore;
                case "TL":
                    return TripleLetterScore;
                case "DW":
                    return DoubleWordScore;
                case "TW":
                    return TripleWordScore;
                case "C":
                    return Centre;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as SquareType;
            return other != null && _description.Equals(other._description);
        }

        public override int GetHashCode()
        {
            return _description != null ? _description.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return _description == "N" ? "" : _description;
        }
    }
}
