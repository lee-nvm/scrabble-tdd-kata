using Scrabble.Lib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Lib
{
    /// <summary>
    /// The game. Consists of the board, players, the "bag" containing the remaining tiles
    /// </summary>
    public class Game
    {
        private int _currentPlayerIndex;
        private int _previousPlayerIndex = -1;
        private IList<TilePoint> _lastWord = new List<TilePoint>();
        private int _lastWordScore;
        private readonly TileBag _tileBag;

        public event EventHandler<PlayerEventArgs> PlayerChanged;
        public event EventHandler ScoreChanged;
        public event EventHandler<PlayerEventArgs> PlayerTilesSwapped;
        public event EventHandler GameStarted;

        public Board Board { get; private set; }
        public IReadOnlyList<Player> Players { get; private set; }

        public static Game Create(IEnumerable<Player> players, Board board, TileBag tileBag)
        {
            return new Game(players, board, tileBag);
        }

        public static Game Create(IEnumerable<string> names)
        {
            return new Game(names);
        }

        private Game(IEnumerable<string> names)
            : this(names.Select(Player.Create), Board.Create(), TileBag.Create())
        {
        }

        private Game(IEnumerable<Player> players, Board board, TileBag tileBag)
        {
            if (players == null)
            {
                throw new ArgumentException("Need some players", "players");
            }

            Players = players.ToList();
            if (!Players.Any())
            {
                throw new ArgumentException("Too few players", "players");
            }

            if (Players.Count() > 4)
            {
                throw new ArgumentException("Too many players", "players");
            }

            Board = board;
            _currentPlayerIndex = 0;
            _tileBag = tileBag;

            foreach (var player in Players)
            {
                player.ScoreChanged += (s, e) => OnScoreChanged();
                player.Tiles = _tileBag.Pick(7);
            }
        }

        public void Start()
        {
            OnGameStarted();
        }

        public Player CurrentPlayer
        {
            get
            {
                return Players[_currentPlayerIndex];
            }
        }

        public Player PreviousPlayer
        {
            get
            {
                return _previousPlayerIndex == -1 ? null : Players[_previousPlayerIndex];
            }
        }

        public Player NextPlayer
        {
            get
            {
                return _currentPlayerIndex + 1 == Players.Count ? Players[0] : Players[_currentPlayerIndex + 1];
            }
        }

        public LayWordResponse LayWord(string playerName, params TilePoint[] tilePoints)
        {
            try
            {
                if (CurrentPlayer.Name != playerName)
                {
                    return LayWordResponse.CreateFailureResponse(Players.Single(p => p.Name == playerName), "It's not your turn");
                }
                var tilePointsList = tilePoints as IList<TilePoint> ?? tilePoints.ToList();
                ValidateCurrentPlayerTiles(tilePointsList.Select(tp => tp.Tile));
                Board.ValidateWordPosition(tilePointsList);
                _lastWord = tilePointsList;
                _lastWordScore = Board.ScoreWord(_lastWord);
                return LayWordResponse.CreateSuccessResponse(CurrentPlayer, 0);
            }
            catch (NoIntersectionException)
            {
                return LayWordResponse.CreateFailureResponse(CurrentPlayer, "Word does not touch an existing word");
            }
            catch (InitialWordNotIntersectingCentreException)
            {
                return LayWordResponse.CreateFailureResponse(CurrentPlayer, "First word needs to go through the centre square");
            }
            catch (SquaresNotContiguousException)
            {
                return LayWordResponse.CreateFailureResponse(CurrentPlayer, "Tiles need to touch");
            }
            catch (TileInvalidForPlayerException)
            {
                return LayWordResponse.CreateFailureResponse(CurrentPlayer, "Tiles laid are not in the player's hand");
            }
            catch (PointsNotInALineException)
            {
                return LayWordResponse.CreateFailureResponse(CurrentPlayer, "Tiles have not been laid in the same row or column");
            }
            catch (SquareOccupiedException)
            {
                return LayWordResponse.CreateFailureResponse(CurrentPlayer, "One or more tiles have been laid on an occupied square");
            }
        }

        public LayWordResponse AcceptWord()
        {
            if (_lastWord.Count == 0)
            {
                return LayWordResponse.CreateFailureResponse(CurrentPlayer, "No word to accept");
            }
            Board.LayWord(_lastWord);
            CurrentPlayer.IncrementScore(_lastWordScore);
            CurrentPlayer.RemoveTiles(_lastWord.Select(tp => tp.Tile));
            CurrentPlayer.PickTiles(_tileBag.Pick(_lastWord.Count()));
            var response = LayWordResponse.CreateSuccessResponse(CurrentPlayer, CurrentPlayer.Score);
            MoveToNextPlayer();
            return response;
        }

        public void DisallowWord()
        {
            foreach (var tile in _lastWord)
            {
                Board[tile.Point].State = Vacant.Instance;
            }
            _lastWord = new List<TilePoint>();
            _lastWordScore = 0;
        }

        public void MoveToNextPlayer()
        {
            _previousPlayerIndex = _currentPlayerIndex++;
            if (_currentPlayerIndex == Players.Count())
            {
                _currentPlayerIndex = 0;
            }
            OnPlayerChanged(CurrentPlayer);
        }

        private void ValidateCurrentPlayerTiles(IEnumerable<Tile> laidTiles)
        {
            var laidTilesList = laidTiles as IList<Tile> ?? laidTiles.ToList();

            if (laidTilesList.Any(tp => !CurrentPlayer.Tiles.Contains(tp)))
            {
                throw TileInvalidForPlayerException.Create();
            }

            var laidCounts = laidTilesList
                .GroupBy(t => t.Letter)
                .OrderBy(t => t.Key)
                .Select(t => new { key = t.Key, cnt = t.Count() })
                .ToList();

            var heldCounts = CurrentPlayer.Tiles
                .Where(t => laidTilesList.Contains(t))
                .GroupBy(t => t.Letter)
                .OrderBy(t => t.Key)
                .Select(t => new { key = t.Key, cnt = t.Count() })
                .ToList();

            for (var i = 0; i < laidCounts.Count(); ++i)
            {
                if (laidCounts[i].cnt > heldCounts[i].cnt)
                {
                    throw TileInvalidForPlayerException.Create();
                }
            }
        }

        public void SwapCurrentPlayerTiles(params char[] tileChars)
        {
            CurrentPlayer.Tiles = _tileBag.SwapTiles(CurrentPlayer.Tiles, tileChars);
            OnPlayerTilesSwapped(CurrentPlayer);
        }

        private void OnPlayerTilesSwapped(Player player)
        {
            PlayerTilesSwapped?.Invoke(this, PlayerEventArgs.Create(player));
        }

        private void OnPlayerChanged(Player currentPlayer)
        {
            PlayerChanged?.Invoke(this, PlayerEventArgs.Create(currentPlayer));
        }

        private void OnGameStarted()
        {
            GameStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnScoreChanged()
        {
            ScoreChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
