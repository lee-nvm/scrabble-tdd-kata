using Scrabble.Lib.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Scrabble.Lib
{
    /// <summary>
    /// The game board
    /// </summary>
    public class Board
    {
        private Square[] Squares { get; set; }

        internal BoardValidator Validator { private get; set; }

        public static Board Create()
        {
            return new Board(InitialBoardValidator.Instance);
        }

        public static Board Create(BoardValidator validator)
        {
            return new Board(validator);
        }

        private Board(BoardValidator validator)
        {
            Validator = validator;

            Squares = Square.FromPattern(
                "TW,,,DL,,,,TW,,,,DL,,,TW" +
                ",,DW,,,,TL,,,,TL,,,,DW," +
                ",,,DW,,,,DL,,DL,,,,DW,," +
                ",DL,,,DW,,,,DL,,,,DW,,,DL" +
                ",,,,,DW,,,,,,DW,,,," +
                ",,TL,,,,TL,,,,TL,,,,TL," +
                ",,,DL,,,,DL,,DL,,,,DL,," +
                ",TW,,,DL,,,,C,,,,DL,,,TW" +
                ",,,DL,,,,DL,,DL,,,,DL,," +
                ",,TL,,,,TL,,,,TL,,,,TL," +
                ",,,,,DW,,,,,,DW,,,," +
                ",DL,,,DW,,,,DL,,,,DW,,,DL" +
                ",,,DW,,,,DL,,DL,,,,DW,," +
                ",,DW,,,,TL,,,,TL,,,,DW," +
                ",TW,,,DL,,,,TW,,,,DL,,,TW").ToArray();
        }

        public Square this[Point point]
        {
            get
            {
                return Squares[GetSquareIndexFromPoint(point)];
            }
        }

        public IEnumerable<Square> OccupiedSquares
        {
            get
            {
                return Squares.Where(s => s.State is Occupied);
            }
        }

        public IEnumerable<Square> VacantSquares
        {
            get
            {
                return Squares.Where(s => s.State is Vacant);
            }
        }

        public void ValidateWordPosition(IEnumerable<TilePoint> tilePoints)
        {
            var tilePointsList = tilePoints as IList<TilePoint> ?? tilePoints.ToList();
            ValidateOccupiedSquares(tilePointsList);
            ValidatePointsInALine(tilePointsList);
            Validator.ValidateWordPosition(tilePointsList, Squares);
        }

        public int ScoreWord(IEnumerable<TilePoint> tilePoints)
        {
            var tilePointsList = tilePoints as IList<TilePoint> ?? tilePoints.ToList();
            return Validator.ScoreWord(tilePointsList.Select(ta => (this[ta.Point], ta.Tile)), Squares);
        }

        public void LayWord(IEnumerable<TilePoint> tilePoints)
        {
            foreach (var tp in tilePoints)
            {
                Squares[GetSquareIndexFromPoint(tp.Point)].State = Occupied.Create(tp.Tile);
            }
            Validator = SubsequentBoardValidator.Instance;
        }

        public static Point GetPoint(int index)
        {
            return Point.Create(GetXPos(index).ToString(CultureInfo.InvariantCulture) + GetYPos(index));
        }

        private static void ValidatePointsInALine(IList<TilePoint> tilePoints)
        {
            if ((tilePoints.Select(tp => tp.Point.X).GroupBy(x => x).Count() > 1)
                && (tilePoints.Select(tp => tp.Point.Y).GroupBy(y => y).Count() > 1))
            {
                throw PointsNotInALineException.Create(tilePoints.Select(tp => tp.Point));
            }
        }

        private void ValidateOccupiedSquares(IEnumerable<TilePoint> tilePoints)
        {
            var occupiedTilePoints = tilePoints.Where(tp => Squares[GetSquareIndexFromPoint(tp.Point)].State is Occupied).ToList();
            if (occupiedTilePoints.Any())
            {
                throw SquareOccupiedException.Create(occupiedTilePoints.Select(tp => Squares[GetSquareIndexFromPoint(tp.Point)]));
            }
        }

        private static char GetXPos(int index)
        {
            return (char)(65 + (index % 15));
        }

        private static int GetYPos(int index)
        {
            return 15 - (int)Math.Floor(index / 15.0);
        }

        private static int GetSquareIndexFromPoint(Point point)
        {
            var rowIndex = (15 - point.Y) * 15;
            var colIndex = point.X - 65;
            return rowIndex + colIndex;
        }
    }
}
