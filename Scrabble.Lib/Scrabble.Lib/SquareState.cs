namespace Scrabble.Lib
{
    public abstract class SquareState
    {
        public string Description { get; protected set; }
    }

    public class Vacant : SquareState
    {
        public static readonly Vacant Instance = new Vacant { Description = "Vacant" };
    }

    public class Occupied : SquareState
    {
        public static Occupied Create(Tile tile)
        {
            return new Occupied(tile);
        }

        private Occupied(Tile tile)
        {
            Description = "Occupied";
            Tile = tile;
        }

        public Tile Tile { get; }
    }
}