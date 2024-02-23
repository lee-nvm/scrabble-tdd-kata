using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    public class Player
    {
        private IList<Tile> _tiles;

        public event EventHandler ScoreChanged;

        public static Player Create(string name)
        {
            return new Player(name);
        }

        private Player(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public int Score { get; private set; }
        public IEnumerable<Tile> Tiles
        {
            get
            {
                return _tiles;
            }
            internal set
            {
                _tiles = value.ToList();
            }
        }

        public void IncrementScore(int points)
        {
            Score += points;
            OnScoreChanged();
        }

        private void OnScoreChanged()
        {
            ScoreChanged?.Invoke(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Score);
        }

        internal void PickTiles(IEnumerable<Tile> newTiles)
        {
            Tiles = Tiles.Concat(newTiles);
        }

        internal void RemoveTiles(IEnumerable<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                _tiles.Remove(tile);
            }
        }

        public override bool Equals(object obj)
        {
            var objAsPlayer = obj as Player;
            if (objAsPlayer == null)
            {
                return false;
            }
            return objAsPlayer.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
