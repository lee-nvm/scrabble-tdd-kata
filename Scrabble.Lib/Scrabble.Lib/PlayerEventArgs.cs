using System;

namespace Scrabble.Lib
{
    public class PlayerEventArgs : EventArgs
    {
        public static PlayerEventArgs Create(Player player)
        {
            return new PlayerEventArgs(player);
        }

        private PlayerEventArgs(Player player)
        {
            Player = player;
        }

        public Player Player { get; set; }
    }
}
