using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib.Test
{
    public class FakeTileBag : TileBag
    {
        public static FakeTileBag Create(string tileChars)
        {
            return new FakeTileBag(tileChars);
        }

        private FakeTileBag(string tileChars)
            : base(NonRandomRandom.Instance, tileChars.Select(Tile.FromChar)) { }

        public int TilesPicked { get; private set; }

        public override IEnumerable<Tile> Pick(int count)
        {
            var picked = base.Pick(count).ToList();
            TilesPicked += picked.Count();
            return picked;
        }
    }
}