using System;

namespace Scrabble.Lib
{
    [Flags]
    public enum Direction
    {
        None,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8
    }

    [Flags]
    public enum Orientation
    {
        None,
        Horizontal = 1,
        Vertical = 2
    }
}