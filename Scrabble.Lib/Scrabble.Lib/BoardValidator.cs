using Scrabble.Lib.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    public abstract class BoardValidator
    {
        public void ValidateWordPosition(IEnumerable<TilePoint> tilePoints, IList<Square> boardSquares)
        {
            ValidateSquaresAreContiguous(tilePoints, boardSquares);
            ValidateWordPositionImpl(tilePoints, boardSquares);
        }

        public int ScoreWord(IEnumerable<(Square Square, Tile Tile)> laidTiles, IList<Square> boardSquares)
        {
            return BoardScoreCalculator.ScoreWord(laidTiles, boardSquares);
        }

        protected abstract void ValidateWordPositionImpl(IEnumerable<TilePoint> tilePoints, IList<Square> boardSquares);

        protected static void ValidateSquaresAreContiguous(IEnumerable<TilePoint> squares, IEnumerable<Square> boardSquares)
        {
            var wordSquares = boardSquares.Where(s => squares.Select(tp => tp.Point).Contains(s.Point)).ToList();
            var orientation = wordSquares.GetOrientation();
            if (orientation.HasFlag(Orientation.Horizontal))
            {
                var xPos = wordSquares.Select(ws => ws.Point.X).OrderBy(x => x).ToList();
                for (var i = 1; i < xPos.Count(); ++i)
                {
                    if (xPos[i - 1] == ((char)(xPos[i] - 1)))
                    {
                        continue;
                    }
                    var xReq = (char)(xPos[i] - 1);
                    var yReq = wordSquares[0].Point.Y;

                    var point = Point.Create(xReq, yReq);
                    var boardSquare = boardSquares.First(s => s.Point.Equals(point));
                    if (boardSquare.State is Vacant)
                    {
                        throw SquaresNotContiguousException.Create();
                    }
                }
            }

            if (orientation.HasFlag(Orientation.Vertical))
            {
                var yPos = wordSquares.Select(ws => ws.Point.Y).OrderBy(y => y).ToList();
                for (var i = 1; i < yPos.Count(); ++i)
                {
                    if (yPos[i - 1] == (yPos[i] - 1))
                    {
                        continue;
                    }
                    var xReq = wordSquares[0].Point.X;
                    var yReq = yPos[i] - 1;

                    var point = Point.Create(xReq, yReq);
                    var boardSquare = boardSquares.First(s => s.Point.Equals(point));
                    if (boardSquare.State is Vacant)
                    {
                        throw SquaresNotContiguousException.Create();
                    }
                }
            }
        }
    }
}
