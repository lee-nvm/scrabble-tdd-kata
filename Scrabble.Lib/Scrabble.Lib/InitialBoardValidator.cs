using Scrabble.Lib.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    public class InitialBoardValidator : BoardValidator
    {
        private static InitialBoardValidator _instance;
        public static InitialBoardValidator Instance
        {
            get
            {
                return _instance ?? (_instance = new InitialBoardValidator());
            }
        }

        private InitialBoardValidator() { }

        protected override void ValidateWordPositionImpl(IEnumerable<TilePoint> tilePoints, IList<Square> boardSquares)
        {
            var centreSquare = boardSquares.First(s => s.Type.Equals(SquareType.Centre));
            if (!tilePoints.Any(tp => tp.Point.Equals(centreSquare.Point)))
            {
                throw InitialWordNotIntersectingCentreException.Create();
            }
        }
    }
}