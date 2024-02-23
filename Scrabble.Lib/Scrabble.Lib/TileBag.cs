using Scrabble.Lib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    public class TileBag : List<Tile>
    {
        private readonly Random _indexFactory;

        public static TileBag Create()
        {
            return new TileBag();
        }

        public static TileBag Create(Random indexFactory)
        {
            return new TileBag(indexFactory);
        }

        public static TileBag Create(Random indexFactory, IEnumerable<Tile> tiles)
        {
            return new TileBag(indexFactory, tiles);
        }

        private TileBag()
            : this(new Random())
        { }

        private TileBag(Random indexFactory)
            : this(indexFactory, BuildStandardTiles())
        { }

        protected TileBag(Random indexFactory, IEnumerable<Tile> tiles)
        {
            _indexFactory = indexFactory;
            AddRange(tiles);
        }

        public virtual IEnumerable<Tile> Pick(int numberRequired)
        {
            var picked = new List<Tile>();

            for (var i = 0; i < numberRequired && Count > 0; ++i)
            {
                var index = _indexFactory.Next(Count);
                picked.Add(this[index]);
                RemoveAt(index);
            }

            return picked;
        }

        private static IEnumerable<Tile> BuildStandardTiles()
        {
            return new[] { Tile.Blank, Tile.Blank }
            .Concat(GetTiles('A', 9))
            .Concat(GetTiles('B', 2))
            .Concat(GetTiles('C', 2))
            .Concat(GetTiles('D', 4))
            .Concat(GetTiles('E', 12))
            .Concat(GetTiles('F', 2))
            .Concat(GetTiles('G', 3))
            .Concat(GetTiles('H', 2))
            .Concat(GetTiles('I', 9))
            .Concat(GetTiles('J', 1))
            .Concat(GetTiles('K', 1))
            .Concat(GetTiles('L', 4))
            .Concat(GetTiles('M', 2))
            .Concat(GetTiles('N', 6))
            .Concat(GetTiles('O', 8))
            .Concat(GetTiles('P', 2))
            .Concat(GetTiles('Q', 1))
            .Concat(GetTiles('R', 6))
            .Concat(GetTiles('S', 4))
            .Concat(GetTiles('T', 6))
            .Concat(GetTiles('U', 4))
            .Concat(GetTiles('V', 2))
            .Concat(GetTiles('W', 2))
            .Concat(GetTiles('X', 1))
            .Concat(GetTiles('Y', 2))
            .Concat(GetTiles('Z', 1));
        }

        private static IEnumerable<Tile> GetTiles(char tileChar, int count)
        {
            return new string(tileChar, count).Select(Tile.FromChar);
        }

        public IEnumerable<Tile> SwapTiles(IEnumerable<Tile> playerTiles, char[] tileCharsToSwap)
        {
            if (Count < 7)
            {
                throw TooFewTilesRemainingException.Create();
            }

            var tilesToSwap = tileCharsToSwap.Select(Tile.FromChar).ToList();
            var currentTiles = playerTiles.ToList();
            foreach (var tileToSwap in tilesToSwap)
            {
                currentTiles.Remove(tileToSwap);
                currentTiles.AddRange(Pick(1));
            }
            AddRange(tilesToSwap);
            return currentTiles;
        }
    }
}
