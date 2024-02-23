using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Scrabble.Lib;

public class BoardScoreCalculator
{
    public static int ScoreWord(IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
    {
        var score = ScoredWords(laidTiles, boardSquares)
            .Distinct(new WordComparer()).ToArray()
            .Select(w => GetTileScore(w, laidTiles, boardSquares))
            .Sum();

        return laidTiles.Count() == 7
            ? score += 50
            : score;
    }

    private static IEnumerable<IEnumerable<(Square Square, Tile Tile)>> ScoredWords(IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
    {
        var words = new List<IEnumerable<(Square Square, Tile Tile)>>();
        foreach (var tile in laidTiles)
        {
            // Horizontal Words
            words.Add(TryFindWholeWord(true, tile.Square.Point, laidTiles, boardSquares));

            // Vertical Words
            words.Add(TryFindWholeWord(false, tile.Square.Point, laidTiles, boardSquares));
        }

        return words.Where(w => w != null);
    }

    private static IEnumerable<(Square Square, Tile Tile)> TryFindWholeWord(bool isHorizontal, Point point, IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
    {
        var word = new List<(Square Square, Tile Tile)>();
        if (isHorizontal)
        {
            word.AddRange(FindConnectedTiles(-1, 0, point, laidTiles, boardSquares)); // LEFT
            word.Add(laidTiles.First(t => t.Square.Point.Equals(point)));
            word.AddRange(FindConnectedTiles(1, 0, point, laidTiles, boardSquares));  // RIGHT
        }
        else
        {
            word.AddRange(FindConnectedTiles(0, -1, point, laidTiles, boardSquares)); // UP
            word.Add(laidTiles.First(t => t.Square.Point.Equals(point)));
            word.AddRange(FindConnectedTiles(0, 1, point, laidTiles, boardSquares));  // DOWN
        }

        return word.Count > 1
            ? word.OrderBy(w => w.Square.Point.X).ThenBy(w => w.Square.Point.Y)
            : null;
    }

    private static List<(Square Square, Tile Tile)> FindConnectedTiles(int horizontalOffset, int verticalOffset, Point point, IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
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
            affectedTitles.AddRange(FindConnectedTiles(horizontalOffset, verticalOffset, nextPoint.Value, laidTiles, boardSquares));
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
            affectedTitles.AddRange(FindConnectedTiles(horizontalOffset, verticalOffset, nextPoint.Value, laidTiles, boardSquares));
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

    private static int GetTileScore(IEnumerable<(Square Square, Tile Tile)> word, IEnumerable<(Square Square, Tile Tile)> laidTiles, IEnumerable<Square> boardSquares)
    {
        var score = 0;
        var multiplier = 0;
        foreach (var tile in word)
        {
            if (laidTiles.Any(t => t.Square.Point.Equals(tile.Square.Point)))
            {
                var square = boardSquares.First(s => s.Point.Equals(tile.Square.Point));
                score += square.Type.ToString() switch
                {
                    "DL" => tile.Tile.Value * 2,
                    "TL" => tile.Tile.Value * 3,
                    _ => tile.Tile.Value
                };
                multiplier += square.Type.ToString() switch
                {
                    "DW" => 2,
                    "TW" => 3,
                    "C" => 2,
                    _ => 0
                };
            }
            else
            {
                score += tile.Tile.Value;
            }
        }

        return score * Math.Max(multiplier, 1);
    }
}

public class WordComparer : IEqualityComparer<IEnumerable<(Square Square, Tile Tile)>>
{
    public bool Equals(IEnumerable<(Square Square, Tile Tile)> x, IEnumerable<(Square Square, Tile Tile)> y) =>
        x.First().Square.Point.Equals(y.First().Square.Point) &&
        x.Last().Square.Point.Equals(y.Last().Square.Point);

    public int GetHashCode([DisallowNull] IEnumerable<(Square Square, Tile Tile)> obj) => obj.First().GetHashCode() + obj.Last().GetHashCode();
}