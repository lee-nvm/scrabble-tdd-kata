using NUnit.Framework;
using System.Linq;

namespace Scrabble.Lib.Test
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void BoardIsCorrectShape()
        {
            var board = Board.Create();
            var squares = board.VacantSquares.ToArray();
            Assert.That(squares.Length == 15 * 15);
            Assert.That(squares.Skip(112).First().Type, Is.EqualTo(SquareType.Centre));

            var first112 = squares.Take(112).Select(s => s.Type).ToArray();
            var last112 = squares.Skip(113).Select(s => s.Type).Reverse().ToArray();

            for (var i = 0; i < first112.Length; ++i)
            {
                Assert.That(first112[i], Is.EqualTo(last112[i]));
            }
        }

        [TestCase("A15", "TW")]
        [TestCase("A14", "N")]
        [TestCase("O15", "TW")]
        [TestCase("H8", "C")]
        [TestCase("F2", "TL")]
        public void GetSquare(string gridRef, string squareType)
        {
            var board = Board.Create();
            var point = Point.Create(gridRef);
            Assert.That(board[point].Type, Is.EqualTo(SquareType.FromDescription(squareType)));
        }

        [Test]
        public void CorrectPointCalculatedFromIndex()
        {
            Assert.That(Board.GetPoint(0).ToString(), Is.EqualTo("A15"));
            Assert.That(Board.GetPoint(14).ToString(), Is.EqualTo("O15"));
            Assert.That(Board.GetPoint(210).ToString(), Is.EqualTo("A1"));
            Assert.That(Board.GetPoint(224).ToString(), Is.EqualTo("O1"));
        }
    }
}