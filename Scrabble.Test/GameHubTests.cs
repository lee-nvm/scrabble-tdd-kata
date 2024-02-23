using NUnit.Framework;
using Rhino.Mocks;
using Scrabble.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scrabble.Test
{
    [TestFixture]
    public class GameHubTests
    {
        [SetUp]
        public void CreatePlayers()
        {
            _players = _names.Select(Player.Create).ToArray();
        }

        private Player[] _players;
        private readonly string[] _names = { "Tony", "Tanny", "Anon1", "Anon2" };

        private static IEnumerable<Square> WithCountOfSquares(int squareCount)
        {
            return Arg<IEnumerable<Square>>.Matches(sqs => sqs.Count() == squareCount);
        }

        private GameHub CreateMockedHubWithStandardGame()
        {
            var hub = MockRepository.GenerateMock<GameHub>();
            GameHub.InitialiseConnectionIds();
            hub.Stub(h => h.NotifyScoreChanged());
            hub.Stub(h => h.NotifyBoardChanged(AnySquares));

            foreach (var player in _players)
            {
                hub.CapturePlayerName(player.Name);
            }

            GameHub.SetAllPlayerGames(Game.Create(_players, Board.Create(),
                FakeTileBag.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ")));

            hub.StartGame();
            return hub;
        }

        public static T AnyArg<T>()
        {
            return Arg<T>.Is.Anything;
        }

        private static IEnumerable<Square> AnySquares
        {
            get { return Arg<IEnumerable<Square>>.Is.Anything; }
        }

        private static IList<TilePoint> AnyTilePoints
        {
            get { return Arg<IList<TilePoint>>.Is.Anything; }
        }

        private static Player AnyPlayer
        {
            get { return Arg<Player>.Is.Anything; }
        }

        private static Player PlayerNameIs(string playerName)
        {
            return Arg<Player>.Matches(p => p.Name == playerName);
        }

        private static string MessageMatches(string message)
        {
            return Arg<string>.Matches(e => e == message);
        }

        public class NonRandomRandom : Random
        {
            private NonRandomRandom()
            {
            }

            public static NonRandomRandom Create()
            {
                return new NonRandomRandom();
            }

            public override int Next(int maxValue)
            {
                return 0;
            }
        }

        public class FakeTileBag : TileBag
        {
            private FakeTileBag(string tileChars)
                : base(NonRandomRandom.Create(), tileChars.Select(Tile.FromChar))
            {
            }

            private FakeTileBag(int blanks, string tileChars)
                : base(NonRandomRandom.Create(), GetBlanks(blanks).Concat(tileChars.Select(Tile.FromChar)))
            {
            }

            public int TilesPicked { get; private set; }

            public static FakeTileBag Create(string tileChars)
            {
                return new FakeTileBag(tileChars);
            }

            public static FakeTileBag CreateWithBlanks(int blanks, string tileChars)
            {
                return new FakeTileBag(blanks, tileChars);
            }

            public override IEnumerable<Tile> Pick(int count)
            {
                var picked = base.Pick(count).ToList();
                TilesPicked += picked.Count();
                return picked;
            }

            private static IEnumerable<Tile> GetBlanks(int count)
            {
                var blanks = new List<Tile>(count);
                for (var i = 0; i < count; ++i)
                {
                    blanks.Add(Tile.Blank);
                }
                return blanks;
            }
        }

        [Test]
        public void AcceptingWordIncreasesPlayerScoreAndMovesOn()
        {
            var hub = CreateMockedHubWithStandardGame();

            var tiles = new List<TilePoint>
            {
                TilePoint.Create('A', "H8"),
                TilePoint.Create('B', "I8"),
                TilePoint.Create('C', "J8")
            };

            hub.LayWordWithList(tiles, _players[0].Name);
            hub.AssertWasCalled(h => h.CallNewWordHandler(tiles, _players[0], new List<string> { "Tanny", "Anon1", "Anon2" }));
            hub.AssertWasCalled(h => h.CallWordJudgementHandler(_players[1]));
            hub.AcceptWord();
            hub.AssertWasCalled(h => h.NotifyBoardChanged(WithCountOfSquares(3)));
            hub.AssertWasCalled(h => h.NotifyPlayerChanged(new List<string> { "Tony", "Tanny", "Anon1", "Anon2" }, "Tanny"));
            hub.AssertWasCalled(h => h.NotifyActivatedPlayer("Tanny"));
            hub.AssertWasCalled(h => h.NotifyTilesChanged(PlayerNameIs("Tony")));
            hub.AssertWasCalled(h => h.NotifyScoreChanged());
        }

        [Test]
        public void AcceptingWordWhenAlreadyDisallowedShouldNotMovePlayer()
        {
            var hub = CreateMockedHubWithStandardGame();

            var tiles = new List<TilePoint>
            {
                TilePoint.Create('A', "H8"),
                TilePoint.Create('B', "I8"),
                TilePoint.Create('C', "J8")
            };

            hub.LayWordWithList(tiles, _players[0].Name);
            hub.DisallowWord();
            hub.AcceptWord();
            hub.AssertWasNotCalled(h => h.NotifyPlayerChanged(Arg<List<string>>.Is.Anything, Arg<string>.Is.Equal("Tanny")));
            hub.AssertWasNotCalled(h => h.NotifyActivatedPlayer("Tanny"));
        }

        [Test]
        public void AddingPlayerInitialisesPlayerWithNoGame()
        {
            GameHub.InitialiseConnectionIds();
            var hub = MockRepository.GenerateMock<GameHub>();

            hub.CapturePlayerName(_names[0]);
        }

        [Test]
        public void AddingPlayersAndStartingAGameInitialisesAllPlayers()
        {
            GameHub.InitialiseConnectionIds();
            var hub = MockRepository.GenerateMock<GameHub>();

            foreach (var name in _names)
            {
                hub.CapturePlayerName(name);
            }

            hub.StartGame();
            var args = hub.GetArgumentsForCallsMadeOn(h => h.NotifyTilesChanged(AnyPlayer));

            Assert.That(args.Count, Is.EqualTo(4));
            foreach (var name in _names)
            {
                Assert.That(args.Any(a => ((Player)a[0]).Name == name));
            }
        }

        [Test]
        public void RestartingAGameReinitialisesAllPlayers()
        {
            GameHub.InitialiseConnectionIds();
            var hub = MockRepository.GenerateMock<GameHub>();

            foreach (var name in _names)
            {
                hub.CapturePlayerName(name);
            }

            hub.StartGame();
            hub.RestartGame();
            var args = hub.GetArgumentsForCallsMadeOn(h => h.NotifyTilesChanged(AnyPlayer));

            Assert.That(args.Count, Is.EqualTo(8));
            foreach (var name in _names)
            {
                Assert.That(args.Any(a => ((Player)a[0]).Name == name));
            }
        }

        [Test]
        public void PlayerCanEndAGame()
        {
            GameHub.InitialiseConnectionIds();
            var hub = MockRepository.GenerateMock<GameHub>();

            foreach (var name in _names)
            {
                hub.CapturePlayerName(name);
            }

            hub.StartGame();
            hub.EndGame();
            Assert.Throws<InvalidOperationException>(() => hub.StartGame());
        }

        [Test]
        public void DisallowingWordResetsPlayerTilesAndBoard()
        {
            var hub = CreateMockedHubWithStandardGame();

            var tiles = new List<TilePoint>
            {
                TilePoint.Create('A', "H8"),
                TilePoint.Create('B', "I8"),
                TilePoint.Create('C', "J8")
            };

            hub.LayWordWithList(tiles, _players[0].Name);
            hub.AssertWasCalled(h => h.CallNewWordHandler(tiles, _players[0], new List<string> { "Tanny", "Anon1", "Anon2" }));
            hub.AssertWasCalled(h => h.CallWordJudgementHandler(_players[1]));
            hub.DisallowWord();

            hub.AssertWasCalled(h => h.NotifyBoardChanged(WithCountOfSquares(0)));
            hub.AssertWasCalled(h => h.NotifyTilesChanged(PlayerNameIs("Tony")));

            hub.AssertWasNotCalled(h => h.NotifyPlayerChanged(Arg<List<string>>.Is.Anything, Arg<string>.Is.Equal("Tanny")));
            hub.AssertWasNotCalled(h => h.NotifyActivatedPlayer("Tanny"));
        }

        [Test]
        public void DisallowingWordWhenAlreadyDisallowedShouldNotThrowException()
        {
            var hub = CreateMockedHubWithStandardGame();

            var tiles = new List<TilePoint>
            {
                TilePoint.Create('A', "H8"),
                TilePoint.Create('B', "I8"),
                TilePoint.Create('C', "J8")
            };

            hub.LayWordWithList(tiles, _players[0].Name);
            hub.DisallowWord();
            hub.DisallowWord();
        }

        [Test]
        public void DisallowingWordWhenNoWordLaidShouldNotThrowException()
        {
            var hub = CreateMockedHubWithStandardGame();
            hub.DisallowWord();
        }

        [Test]
        public void LayingInvalidWordSendsFailureMessageToCurrentPlayerAndResetsBoard()
        {
            var hub = CreateMockedHubWithStandardGame();

            //Doesn't intersect centre square
            var tiles = new List<TilePoint>
            {
                TilePoint.Create('B', "I8"),
                TilePoint.Create('C', "J8")
            };

            hub.LayWordWithList(tiles, _players[0].Name);

            hub.AssertWasCalled(
                h =>
                    h.OnInvalidWordLaid(PlayerNameIs("Tony"),
                        MessageMatches("First word needs to go through the centre square")));
            hub.AssertWasCalled(h => h.NotifyBoardChanged(WithCountOfSquares(0)));
            hub.AssertWasCalled(h => h.NotifyTilesChanged(_players[0]));

            hub.AssertWasNotCalled(h => h.CallNewWordHandler(AnyTilePoints, AnyPlayer, AnyArg<List<string>>()));
            hub.AssertWasNotCalled(h => h.CallWordJudgementHandler(AnyPlayer));
        }

        [Test]
        public void NonCurrentPlayerLayingWordSendsFailureMessageToLayingPlayerAndResetsBoard()
        {
            var hub = CreateMockedHubWithStandardGame();

            var tiles1 = new List<TilePoint>
            {
                TilePoint.Create('A', "H8"),
                TilePoint.Create('B', "I8")
            };

            var tiles2 = new List<TilePoint>
            {
                TilePoint.Create('C', "J8"),
                TilePoint.Create('D', "K8")
            };

            hub.LayWordWithList(tiles1, _players[0].Name);
            hub.AcceptWord();
            hub.LayWordWithList(tiles2, _players[0].Name);

            hub.AssertWasCalled(
                h =>
                    h.OnInvalidWordLaid(PlayerNameIs("Tony"),
                        MessageMatches("It's not your turn")));
        }

        [Test]
        public void SwappingPlayerTilesSwapsTilesTellsPlayerAndMovesPlayer()
        {
            var hub = CreateMockedHubWithStandardGame();
            hub.SwapPlayerTiles(_players[0].Name, new[] { "A1", "D4" });

            hub.AssertWasCalled(h => h.NotifyTilesChanged(PlayerNameIs("Tony")));
            hub.AssertWasCalled(h => h.NotifyTilesChanged(PlayerTilesAre("BCEFGCD")));
            hub.AssertWasCalled(h => h.NotifyPlayerChanged(new List<string> { "Tony", "Tanny", "Anon1", "Anon2" }, "Tanny"));
            hub.AssertWasCalled(h => h.NotifyActivatedPlayer("Tanny"));
        }

        [Test]
        public void SwappingTilesWhenNotCurrentPlayerDoesntWork()
        {
            var hub = CreateMockedHubWithStandardGame();
            hub.SwapPlayerTiles(_players[2].Name, new[] { "B1" });

            //NotifyTilesChanged is called once for each player on startup
            hub.AssertWasCalled(h => h.NotifyTilesChanged(AnyPlayer), c => c.Repeat.Times(4));

            hub.AssertWasNotCalled(h => h.NotifyPlayerChanged(Arg<List<string>>.Is.Anything, Arg<string>.Is.Equal("Tanny")));
            hub.AssertWasNotCalled(h => h.NotifyActivatedPlayer("Tanny"));
        }

        [Test]
        public void SwappingABlankTileWorks()
        {
            var hub = MockRepository.GenerateMock<GameHub>();
            GameHub.InitialiseConnectionIds();
            hub.Stub(h => h.NotifyScoreChanged());
            hub.Stub(h => h.NotifyBoardChanged(AnySquares));

            foreach (var player in _players)
            {
                hub.CapturePlayerName(player.Name);
            }

            var game = Game.Create(_players, Board.Create(),
                FakeTileBag.CreateWithBlanks(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            GameHub.SetAllPlayerGames(game);

            hub.StartGame();

            Assert.That(game.CurrentPlayer.Tiles.Count(t => t.Letter == ' '), Is.EqualTo(2));

            hub.SwapPlayerTiles(game.CurrentPlayer.Name, new[] { "", "" });

            Assert.That(game.CurrentPlayer.Tiles.Count(t => t.Letter == ' '), Is.EqualTo(0));
        }

        private static Player PlayerTilesAre(string expectedLetters)
        {
            return Arg<Player>.Matches(p => expectedLetters.All(c => p.Tiles.Select(t => t.Letter).Contains(c)));
        }
    }
}