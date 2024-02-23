using NUnit.Framework;
using Scrabble.Lib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using static Scrabble.Lib.Test.WordScoreTests;

namespace Scrabble.Lib.Test
{
    [TestFixture]
    public class GameTests
    {
        private string _tilesToSelect;

        internal Game GivenAStandardGame()
        {
            return Game.Create(GetPlayers(), Board.Create(), TileBag.Create(NonRandomRandom.Instance));
        }

        internal Game GivenAGameWithTilesToSelect()
        {
            return Game.Create(GetPlayers(), Board.Create(), FakeTileBag.Create(_tilesToSelect));
        }

        private Game GivenGameWithOccupiedSquares(params string[] occupied)
        {
            var board = Board.Create(SubsequentBoardValidator.Instance);

            occupied.ToList().ForEach(occupiedSquare => board[Point.Create(occupiedSquare)].State = Occupied.Create(Tile.Blank));

            return Game.Create(GetPlayers(), board, FakeTileBag.Create(_tilesToSelect));
        }

        internal static Player[] GetPlayers()
        {
            var players = new[] { Player.Create("Victoria"), Player.Create("Albert") };
            return players;
        }

        [Test]
        public void TilesCannotBeLaidOnOccupiedSquare()
        {
            _tilesToSelect = "ABABCDEABCDEFGA";

            var game = GivenGameWithOccupiedSquares("L2");
            var tilePoint = TilePoint.Create('A', "L1");

            Assert.That(game.LayWord("Victoria", tilePoint).ValidWord, Is.True);
            game.AcceptWord();
            var response = game.LayWord("Albert", tilePoint);
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("One or more tiles have been laid on an occupied square"));
        }

        [Test]
        public void TilesNotInAStraightLineAreInvalid()
        {
            _tilesToSelect = "ABABCDE" + "ABCDEFG";

            var game = GivenGameWithOccupiedSquares();
            var tilePoints = new[] {
                TilePoint.Create('A', "L1"),
                TilePoint.Create('B', "M7")
            };

            var response = game.LayWord("Victoria", tilePoints);
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Tiles have not been laid in the same row or column"));
        }

        [Test]
        public void InitialWordNotThroughCentreIsInvalid()
        {
            _tilesToSelect = "ABABCDE" + "ABCDEFG";

            var game = GivenAGameWithTilesToSelect();

            var tilePoints = new[] {
                TilePoint.Create('A', "A1"),
                TilePoint.Create('B', "B1")
            };

            var response = game.LayWord("Victoria", tilePoints);
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("First word needs to go through the centre square"));

        }

        [Test]
        public void SubsequentWordNotTouchingOccupiedSquareInvalid()
        {
            _tilesToSelect = "ABABCDE" + "ABCDEFG";

            var game = GivenGameWithOccupiedSquares();

            var tilePoints = new[] {
                TilePoint.Create('A', "L1"),
                TilePoint.Create('B', "L2")
            };

            var response = game.LayWord("Victoria", tilePoints);
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Word does not touch an existing word"));
        }

        [Test]
        public void VerticalInitialWordMustBeContiguous()
        {
            _tilesToSelect = "BUSABCD" + "ABCDEFG";

            var game = GivenAGameWithTilesToSelect();

            var tilePoints = new[] {
                TilePoint.Create('B', "H8"),
                TilePoint.Create('U', "H7"),
                TilePoint.Create('S', "H5")
            };

            var response = game.LayWord("Victoria", tilePoints);
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Tiles need to touch"));
        }

        [Test]
        public void VerticalSubsequentWordMustBeContiguous()
        {
            _tilesToSelect = "BUSABCD" + "ABCDEFG";

            var game = GivenGameWithOccupiedSquares("G8", "H8");

            var tilePoints = new[] {
                TilePoint.Create('B', "H9"),
                TilePoint.Create('U', "H7"),
                TilePoint.Create('S', "H5")
            };

            var response = game.LayWord("Victoria", tilePoints);
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Tiles need to touch"));
        }

        [Test]
        public void HorizontalInitialWordMustBeContiguous()
        {
            _tilesToSelect = "BUSABCD" + "ABCDEFG";

            var game = GivenAGameWithTilesToSelect();

            var tilePoints = new[] {
                TilePoint.Create('B', "H8"),
                TilePoint.Create('U', "I8"),
                TilePoint.Create('S', "K8")
            };

            var response = game.LayWord("Victoria", tilePoints);
            game.AcceptWord();
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Tiles need to touch"));
        }

        [Test]
        public void HorizontalSubsequentWordMustBeContiguous()
        {
            _tilesToSelect = "BUSABCD" + "ABCDEFG";

            var game = GivenGameWithOccupiedSquares("H8", "H7");

            var tilePoints = new[] {
                TilePoint.Create('B', "G8"),
                TilePoint.Create('U', "I8"),
                TilePoint.Create('S', "K8")
            };

            var response = game.LayWord("Victoria", tilePoints);
            game.AcceptWord();
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Tiles need to touch"));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void FewerThanFivePlayersIsFine(int playerCount)
        {
            var players = new List<Player>();
            for (var i = 0; i < playerCount; ++i)
            {
                players.Add(Player.Create("P" + i));
            }
            Assert.DoesNotThrow(() => Game.Create(players, Board.Create(), TileBag.Create(NonRandomRandom.Instance)));
        }

        [Test]
        public void FivePlayersIsTooMany()
        {
            var players = new[] { Player.Create("P1"), Player.Create("P2"), Player.Create("P3"), Player.Create("P4"), Player.Create("P5") };
            Assert.Throws<ArgumentException>(() => Game.Create(players, Board.Create(), TileBag.Create(NonRandomRandom.Instance)));
        }

        [Test]
        public void NeedSomePlayers()
        {
            var players = Enumerable.Empty<Player>();
            Assert.Throws<ArgumentException>(() => Game.Create(players, Board.Create(), TileBag.Create(NonRandomRandom.Instance)));

            players = null;
            Assert.Throws<ArgumentException>(() => Game.Create(players, Board.Create(), TileBag.Create(NonRandomRandom.Instance)));
        }

        [Test]
        public void EachPlayerHas7Tiles()
        {
            var game = GivenAStandardGame();
            Assert.That(game.Players[0].Tiles.Count(), Is.EqualTo(7));
            Assert.That(game.Players[1].Tiles.Count(), Is.EqualTo(7));
        }

        [Test]
        public void TilesInNewWordMustBeInPlayersTiles()
        {
            var tileBag = FakeTileBag.Create("AAAAAAAAAAAAAA");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);

            var response = game.LayWord("Victoria", TilePoint.Create('Q', "H8"));
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Tiles laid are not in the player's hand"));
        }

        [Test]
        public void PlayerMustHoldEnoughOfEachTileInNewWord()
        {
            var tileBag = FakeTileBag.Create("ABCDEFGHIJKLMN");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);

            var response = game.LayWord("Victoria", TilePoint.Create('B', "H8"), TilePoint.Create('B', "H9"));
            Assert.That(response.ValidWord, Is.False);
            Assert.That(response.Error, Is.EqualTo("Tiles laid are not in the player's hand"));
        }

        [Test]
        public void IfThePlayerHoldsEnoughOfEachTileTheTheresNotAProblemIsThere()
        {
            var tileBag = FakeTileBag.Create("THREESABCDEFGHIJKLMN");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);

            var response = game.LayWord("Victoria", TilePoint.Create('T', "H8"), TilePoint.Create('H', "H9"), TilePoint.Create('R', "H10"), TilePoint.Create('E', "H11"), TilePoint.Create('E', "H12"), TilePoint.Create('S', "H13"));
            Assert.That(response.ValidWord, Is.True);
        }

        [Test]
        public void WhenBlankTileLaidThenPlayerMustHoldBlankTile()
        {
            var tileBag = FakeTileBag.Create("AAAAAAAAAAAAAA");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);

            Assert.That(game.LayWord("Victoria", TilePoint.CreateBlank('A', "H8")).ValidWord, Is.False);
        }

        [Test]
        public void WhenBlankTileLaidThenItsLetterMustBeAssigned()
        {
            var tileBag = FakeTileBag.Create(" AAAAAAAAAAAAAA");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);

            game.LayWord("Victoria", TilePoint.CreateBlank('A', "H8"));
            game.AcceptWord();
            Assert.That((((Occupied)game.Board[Point.Create("H8")].State).Tile).DisplayLetter, Is.EqualTo('A'));
        }

        [Test]
        public void WhenLayingAnInvalidWordThenPlayDoesNotMoveToNextPlayer()
        {
            var tileBag = FakeTileBag.Create("AAAAAAAAAAAAAA");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Victoria"));
            game.LayWord("Victoria", TilePoint.Create('H', "H8"));
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Victoria"));
        }

        [Test]
        public void WhenLayingInAValidPositionAndNextPlayerAcceptsWordThenPlayMovesToNextPlayer()
        {
            var tileBag = FakeTileBag.Create("AAAAAAAAAAAAAAA");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Victoria"));
            Assert.That(game.CurrentPlayer.Score, Is.EqualTo(0));
            game.LayWord("Victoria", TilePoint.Create('A', "H8"));
            Assert.That(game.CurrentPlayer.Score, Is.EqualTo(0));
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Victoria"));
            game.AcceptWord();
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Albert"));
            Assert.That(game.CurrentPlayer.Score, Is.EqualTo(0));
        }

        [Test]
        public void WhenLayingInAValidPositionAndNextPlayerDisallowsWordThenPlayStaysWithCurrentPlayerAndTilesRemovedFromBoard()
        {
            var tileBag = FakeTileBag.Create("AAAAAAAAAAAAAAA");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Victoria"));
            Assert.That(game.CurrentPlayer.Score, Is.EqualTo(0));
            game.LayWord("Victoria", TilePoint.Create('A', "H8"));
            Assert.That(game.CurrentPlayer.Score, Is.EqualTo(0));
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Victoria"));
            game.DisallowWord();
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("Victoria"));
            Assert.That(game.CurrentPlayer.Score, Is.EqualTo(0));
            Assert.That(game.Board[Point.Create("H8")].State, Is.InstanceOf<Vacant>());
        }

        [Test]
        public void WhenMovingPlayerThenPreviousAndNextPlayersSetCorrectly()
        {
            var players = new[] { Player.Create("P0"), Player.Create("P1"), Player.Create("P2"), Player.Create("P3") };

            var tileBag = FakeTileBag.Create("AAAAAAAAAAAAAAA");
            var game = Game.Create(players, Board.Create(), tileBag);
            Assert.That(game.PreviousPlayer, Is.Null);
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo("P0"));
            Assert.That(game.NextPlayer.Name, Is.EqualTo("P1"));
            LayTile("P0", game);
            AssertPlayerNames(game, "P0", "P1", "P2");
            LayTile("P1", game);
            AssertPlayerNames(game, "P1", "P2", "P3");
            LayTile("P2", game);
            AssertPlayerNames(game, "P2", "P3", "P0");
            LayTile("P3", game);
            AssertPlayerNames(game, "P3", "P0", "P1");
            LayTile("P0", game);
            AssertPlayerNames(game, "P0", "P1", "P2");
        }

        [Test]
        public void DisallowingFirstWordLeavesGameInInitialState()
        {
            var tileBag = FakeTileBag.Create("AAAAAAAAAAAAAAA");
            var game = Game.Create(GetPlayers(), Board.Create(), tileBag);
            Assert.That(game.LayWord("Victoria", TilePoint.Create('A', "H8")).ValidWord);
            game.DisallowWord();
            Assert.That(game.LayWord("Victoria", TilePoint.Create('A', "H8")).ValidWord);
        }

        private static void AssertPlayerNames(Game game, string previous, string current, string next)
        {
            Assert.That(game.PreviousPlayer.Name, Is.EqualTo(previous));
            Assert.That(game.CurrentPlayer.Name, Is.EqualTo(current));
            Assert.That(game.NextPlayer.Name, Is.EqualTo(next));
        }

        private static void LayTile(string playerName, Game game)
        {
            game.LayWord(playerName, TilePoint.Create('A', "H8"));
            game.AcceptWord();
        }
    }

    public class WhenPlayerLaysWord : WordsTest
    {
        public WhenPlayerLaysWord()
        {
            TilesToSelect = "HEABCDE" + "MANFGHI" + "FK" + "UOW";
        }

        [SetUp]
        public void CreateWordsAndScores()
        {
            Words = new[] {
                    new[] {
                        TilePoint.Create('H', "H8"),
                        TilePoint.Create('E', "H7")
                    },
                    new[] {
                        TilePoint.Create('M', "F6"), //TL
                        TilePoint.Create('A', "G6"),
                        TilePoint.Create('N', "H6")
                    }
                };

            ExpectedScores = new[] { 10, 17 };
        }

        [Test]
        public void ThenHas7Tiles()
        {
            for (var i = 0; i < Words.Count(); ++i)
            {
                ScrabbleGame.LayWord(i % 2 == 0 ? "Victoria" : "Albert", Words[i]);
                ScrabbleGame.AcceptWord();
            }

            foreach (var player in ScrabbleGame.Players)
            {
                Assert.That(player.Tiles.Count(), Is.EqualTo(7));
            }
            Assert.That(TileBag.TilesPicked, Is.EqualTo(19));
        }
    }

    public class WhenFewerTilesLeftThanWereLaid : WordsTest
    {
        public WhenFewerTilesLeftThanWereLaid()
        {
            TilesToSelect = "HEABCDE" + "MANFGHI" + "F";
        }

        [SetUp]
        public void CreateWordsAndScores()
        {
            Words = new[] {
                    new[] {
                        TilePoint.Create('H', "H8"),
                        TilePoint.Create('E', "H7")
                    },
                    new[] {
                        TilePoint.Create('M', "F6"), //TL
                        TilePoint.Create('A', "G6"),
                        TilePoint.Create('N', "H6")
                    }
                };

            ExpectedScores = new[] { 10, 17 };
        }

        [Test]
        public void ThenPlayerReceivesAll()
        {
            for (var i = 0; i < Words.Count(); ++i)
            {
                ScrabbleGame.LayWord(i % 2 == 0 ? "Victoria" : "Albert", Words[i]);
                ScrabbleGame.AcceptWord();
            }
            Assert.That(ScrabbleGame.Players[0].Tiles.Count(), Is.EqualTo(6));
            Assert.That(ScrabbleGame.Players[1].Tiles.Count(), Is.EqualTo(4));
            Assert.That(TileBag.TilesPicked, Is.EqualTo(15));
        }
    }


    public class WhenFewerThan7TilesInBag
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void ThenTilesAreNotSwapped(int numberOfTilesRemaining)
        {
            var tilesToSelect = "ABCDEFG" + "HIJKLMN" + "OPQRST".Substring(0, numberOfTilesRemaining);
            var tileBag = FakeTileBag.Create(tilesToSelect);
            var scrabbleGame = Game.Create(GameTests.GetPlayers(), Board.Create(), tileBag);
            var exception = Assert.Throws<TooFewTilesRemainingException>(() => scrabbleGame.SwapCurrentPlayerTiles('A', 'B', 'C', 'D'));
            Assert.That(exception.Message, Is.EqualTo("You can't swap tiles, there are fewer than 7 tiles remaining in the bag"));
        }
    }
}