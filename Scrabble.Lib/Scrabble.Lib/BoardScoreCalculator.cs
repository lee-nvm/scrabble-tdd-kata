using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    public class BoardScoreCalculator
    {
        public static int ScoreWord(IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
        {
            var score = 0;
            var affectedWords = AffectedWords(laidTiles.ToArray(), boardSquares);

            var distinctWords = GetDistinctWords(affectedWords);

            foreach (var word in distinctWords)
            {
                var tileScore = GetTileScore(word, laidTiles, boardSquares);
                score += tileScore * GetMultiplier(word, laidTiles, boardSquares);
            }

            if (laidTiles.Count() == 7)
            {
                score += 50;
            }

            return score;
        }

        private static IEnumerable<IEnumerable<(Square Square, Tile Tile)>> AffectedWords((Square Square, Tile Tile)[] laidTiles, IEnumerable<Square> boardSquares)
        {
            var words = new List<IEnumerable<(Square Square, Tile Tile)>>();

            foreach (var tile in laidTiles)
            {
                // Horizontal Words
                if (TryFindWholeWord(true, tile.Square.Point, laidTiles, boardSquares, out var horizontalWord))
                {
                    words.Add(horizontalWord.OrderBy(word => word.Square.Point.X).ThenBy(word => word.Square.Point.Y));
                }

                // Vertical Words
                if (TryFindWholeWord(false, tile.Square.Point, laidTiles, boardSquares, out var verticalWord))
                {
                    words.Add(verticalWord.OrderBy(word => word.Square.Point.X).ThenBy(word => word.Square.Point.Y));
                }
            }

            return words;
        }

        private static bool TryFindWholeWord(bool isHorizontal, Point point, IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares, out List<(Square Square, Tile Tile)> word)
        {
            word = [];

            if (isHorizontal)
            {

                word.AddRange(FindAllTiles(-1, 0, point, laidTiles, boardSquares)); // LEFT
                word.Add(laidTiles.First(t => t.Square.Point.Equals(point)));
                word.AddRange(FindAllTiles(1, 0, point, laidTiles, boardSquares));  // RIGHT
            }
            else
            {
                word.AddRange(FindAllTiles(0, -1, point, laidTiles, boardSquares)); // UP
                word.Add(laidTiles.First(t => t.Square.Point.Equals(point)));
                word.AddRange(FindAllTiles(0, 1, point, laidTiles, boardSquares));  // DOWN
            }

            return word.Count > 1;
        }

        private static IEnumerable<(Square Square, Tile Tile)> FindAllTiles(int horizontalOffset, int verticalOffset, Point point, IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
        {
            var affectedTitles = new List<(Square Square, Tile Tile)>();

            var nextPoint = GetPoint(horizontalOffset, verticalOffset, point);
            if (nextPoint == null)
            {
                return affectedTitles;
            }

            var existingTile = laidTiles.FirstOrDefault(t => t.Square.Point.Equals(nextPoint));
            if (existingTile != default((Square Square, Tile Tile)))
            {
                affectedTitles.Add(existingTile);
                affectedTitles.AddRange(FindAllTiles(horizontalOffset, verticalOffset, nextPoint.Value, laidTiles, boardSquares));
            }
            else
            {
                var nextSquare = boardSquares.First(s => s.Point.Equals(nextPoint));
                if (nextSquare.State is Vacant)
                {
                    return affectedTitles;
                }

                var state = nextSquare.State as Occupied;
                affectedTitles.Add((nextSquare, state.Tile));
                affectedTitles.AddRange(FindAllTiles(horizontalOffset, verticalOffset, nextPoint.Value, laidTiles, boardSquares));
            }

            return affectedTitles;
        }

        private static Point? GetPoint(int horizontalOffset, int verticalOffset, Point point)
        {
            var horizontalPos = point.X + horizontalOffset;
            if (horizontalPos < 65 || horizontalPos > 79) return null;  // A -> O

            var verticalPos = point.Y + verticalOffset;
            if (verticalPos < 1 || verticalPos > 15) return null;

            return Point.Create($"{(char)horizontalPos}{verticalPos}");
        }

        private static IEnumerable<IEnumerable<(Square Square, Tile Tile)>> GetDistinctWords(IEnumerable<IEnumerable<(Square Square, Tile Tile)>> words)
        {
            var distinctWords = new List<IEnumerable<(Square Square, Tile Tile)>>();
            foreach (var word in words)
            {
                if (!distinctWords.Any(dw => dw.First().Square.Point.Equals(word.First().Square.Point) && dw.Last().Square.Point.Equals(word.Last().Square.Point)))
                {
                    distinctWords.Add(word);
                }
            }
            return distinctWords;
        }

        private static int GetTileScore(IEnumerable<(Square Square, Tile Tile)> word, IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
        {
            var score = 0;
            foreach (var tile in word)
            {
                var square = boardSquares.First(s => s.Point.Equals(tile.Square.Point));
                score += square.Type.ToString() switch
                {
                    "DL" => laidTiles.Any(t => t.Square.Point.Equals(tile.Square.Point)) ? tile.Tile.Value * 2 : tile.Tile.Value,
                    "TL" => laidTiles.Any(t => t.Square.Point.Equals(tile.Square.Point)) ? tile.Tile.Value * 3 : tile.Tile.Value,
                    _ => tile.Tile.Value
                };
            }

            return score;
        }

        private static int GetMultiplier(IEnumerable<(Square Square, Tile Tile)> word, IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
        {
            var multiplier = 0;
            foreach (var tile in word)
            {
                if (laidTiles.Any(t => t.Square.Point.Equals(tile.Square.Point)))
                {
                    var square = boardSquares.First(s => s.Point.Equals(tile.Square.Point));
                    multiplier += square.Type.ToString() switch
                    {
                        "DW" => 2,
                        "TW" => 3,
                        "C" => 2,
                        _ => 0
                    };
                }
            }

            return Math.Max(multiplier, 1);
        }
    }
}