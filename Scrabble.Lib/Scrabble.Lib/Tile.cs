using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Scrabble.Lib
{
    public sealed class Tile
    {
        #region tile definitions
        public static readonly Tile A = new Tile('A', 1);
        public static readonly Tile B = new Tile('B', 3);
        public static readonly Tile C = new Tile('C', 3);
        public static readonly Tile D = new Tile('D', 2);
        public static readonly Tile E = new Tile('E', 1);
        public static readonly Tile F = new Tile('F', 4);
        public static readonly Tile G = new Tile('G', 2);
        public static readonly Tile H = new Tile('H', 4);
        public static readonly Tile I = new Tile('I', 1);
        public static readonly Tile J = new Tile('J', 8);
        public static readonly Tile K = new Tile('K', 5);
        public static readonly Tile L = new Tile('L', 1);
        public static readonly Tile M = new Tile('M', 3);
        public static readonly Tile N = new Tile('N', 1);
        public static readonly Tile O = new Tile('O', 1);
        public static readonly Tile P = new Tile('P', 3);
        public static readonly Tile Q = new Tile('Q', 10);
        public static readonly Tile R = new Tile('R', 1);
        public static readonly Tile S = new Tile('S', 1);
        public static readonly Tile T = new Tile('T', 1);
        public static readonly Tile U = new Tile('U', 1);
        public static readonly Tile V = new Tile('V', 4);
        public static readonly Tile W = new Tile('W', 4);
        public static readonly Tile X = new Tile('X', 8);
        public static readonly Tile Y = new Tile('Y', 4);
        public static readonly Tile Z = new Tile('Z', 10);

        public static Tile Blank => new Tile(' ', 0);

        private static readonly IEnumerable<Tile> _tiles = new[] { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, Blank };
        #endregion

        public Tile(char letter, int value)
        {
            Letter = DisplayLetter = letter;
            Value = value;
        }

        public char Letter { get; set; }
        public char DisplayLetter { get; set; }
        public int Value { get; set; }

        public static Tile FromChar(char letter)
        {
            if (letter == ' ')
            {
                return Blank;
            }

            var tile = _tiles.SingleOrDefault(f => f.Letter == letter);
            if (tile != null)
            {
                return tile;
            }

            throw new ArgumentOutOfRangeException(nameof(letter));
        }

        public static Tile GetBlank(char assignedLetter)
        {
            var tile = Blank;
            tile.DisplayLetter = assignedLetter;
            return tile;
        }

        public override string ToString()
        {
            return Letter.ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tile objTile))
            {
                return false;
            }

            return Letter == objTile.Letter;
        }

        public override int GetHashCode()
        {
            return Letter.GetHashCode();
        }
    }
}
