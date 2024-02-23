using NUnit.Framework;

namespace Scrabble.Lib.Test
{
    [TestFixture]
    public class TileTests
    {
        [Test]
        public void CorrectTileSelected()
        {
            for (var c = 'A'; c <= 'Z'; ++c)
            {
                var tile = Tile.FromChar(c);
                Assert.That(tile.Letter, Is.EqualTo(c));
            }
            Assert.That(Tile.FromChar(' ').Letter, Is.EqualTo(' '));
        }
    }
}