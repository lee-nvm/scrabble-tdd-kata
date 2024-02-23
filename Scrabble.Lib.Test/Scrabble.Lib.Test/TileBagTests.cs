using NUnit.Framework;
using NUnit.Framework.Legacy;
using Scrabble.Lib;
using System;
using System.Linq;

namespace Scrabble.Test
{
    [TestFixture]
    class TileBagTests
    {
        [Test]
        public void GameConsistsOf100Tiles()
        {
            var tileBag = TileBag.Create();
            Assert.That(tileBag.Count(), Is.EqualTo(100));
        }

        [Test]
        public void WhenPlayerSwapsTiles()
        {
            var tileBag = TileBag.Create(new Random());
            var origTiles = new[] { Tile.Blank, Tile.Blank, Tile.Blank, Tile.Blank, Tile.Blank, Tile.Blank, Tile.Blank };
            var swapped = tileBag.SwapTiles(
                origTiles, "       ".ToCharArray());
            CollectionAssert.AreNotEquivalent(swapped, origTiles);
        }
    }
}
