using System;

namespace Scrabble.Lib.Exceptions
{
    public class TileInvalidForPlayerException : Exception
    {
        public static TileInvalidForPlayerException Create()
        {
            return  new TileInvalidForPlayerException();
        }

        private TileInvalidForPlayerException()
        {           
        }
    }
}
