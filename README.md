# Scrabble TDD kata
Contains an implementation of the word game Scrabble, but with no scoring implemented. That's your job. This repo is a cut-down version of a complete implementation of Scrabble I created, so domain objects (board, game, player, tile) are complete and not required to be changed as part of the kata.

## How it works
Existing tests are implemented as black box tests tests (https://en.wikipedia.org/wiki/Black-box_testing), mimicking a game from the initial move. The tests create a dummy letter bag which serves tiles in a predicatable sequence, and lays those tiles to form words.

Each test's inputs consists of the tiles to serve, a series of player turns (tiles laid and the squares they're laid on), and the expected scores for each of the turns.

## The exercise
Tests are defined in `Scrabble.Lib.Test.WordScoreTests`. The first one is the only one not commented out, and will fail. Tests are intended to be incremental and should be made to pass in their numbered order. Try to stick to the TDD principle of only doing enough code to make the current test pass, as the next one will build on it.

The scoring algorithm should be implemented in `Scrabble.Lib.BoardScoreCalculator`. The single method on the class takes two parameters; a collection of tuples of board squares and tiles to lay on those squares, and a collection of board squares representing the board before the new tiles have been laid. The code path taken in the tests already validates things like the number of tiles laid, whether the squares chosen are valid etc.

## Domain hints
### Tile
`Tile` defines the letter tokens as used in the UK version of Scrabble. There are 27 varieties, A-Z plus blank. `Tile.Value` stores the score for that letter.

### SquareType
The type `SquareType` defines whether the board square has a word or letter bonus. Square types are
* `Normal`
* `DoubleLetterScore`
* `TripleLetterScore`
* `DoubleWordScore`
* `TripleWordScore`
* `Centre`

Use the properties WordFactor and LetterFactor to determine the effect on the tile or word scores.

## TDD principles
The main principle of TDD is "Red, Green Refactor"
* verify the test fails (Red), and understand why it fails; 
* do the simplest thing to make it pass (Green), remembering to verify it passes; 
* Refactor the code, verify all tests still pass.
