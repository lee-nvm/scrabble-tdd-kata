namespace Scrabble.Lib
{
    public struct TilePoint
    {
        public static TilePoint Create(char tileLetter, string gridReference)
        {
            return new TilePoint(tileLetter, gridReference);
        }

        public static TilePoint CreateBlank(char assignedLetter, string gridReference)
        {
            return new TilePoint(Tile.GetBlank(assignedLetter), gridReference);
        }

        private TilePoint(Tile tile, string gridReference)
            : this()
        {
            Tile = tile;
            Point = Point.Create(gridReference);
        }

        private TilePoint(char tileLetter, string gridReference)
            : this(Tile.FromChar(tileLetter), gridReference)
        { }

        public Tile Tile { get; set; }
        public Point Point { get; set; }

        public override string ToString()
        {
            return Tile + " " + Point;
        }
    }
}
