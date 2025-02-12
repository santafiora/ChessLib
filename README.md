# ChessLib
A C# chess data library with complete move generation and all needed custom types.

[![Build status](https://ci.appveyor.com/api/projects/status/6dksl8dsq5s1n2uv/branch/master?svg=true)](https://ci.appveyor.com/project/rudzen/chesslib/branch/master)
[![Build & Test](https://github.com/rudzen/ChessLib/actions/workflows/test.yml/badge.svg)](https://github.com/rudzen/ChessLib/actions/workflows/test.yml)
![Nuget](https://img.shields.io/nuget/v/Rudzoft.ChessLib)

## Requirements

* .NET 6.0+

## What is this for?

This library contains all the data, types and structures for which to create a piece of
chess software. It does not contain any heuristics or search algorithms as these
are meant to be implemented separately.

It also contains KPK bit compact data to determine endgame draw.

## Can I use this as a starting point for my chess software?

Yes you can, it is designed with that in mind.

## Features

* Custom perft application which uses the library to calculate and compare results from custom positions
* Transposition Table
* Complete move generation with several types
  * Legal
  * Captures
  * Quiets
  * NonEvasions
  * Evasions
  * QuietChecks
* Custom compact and very efficient types with tons of operators and helper functionality
  * Bitboard
  * CastleRight
  * Depth
  * Direction
  * ExtMove (move + score)
  * File
  * HashKey
  * Move
  * Piece
  * PieceSquare (for UI etc)
  * PieceValue
  * Player
  * Rank
  * Score
  * Square
  * Value
* Bitboard use with piece attacks for all types, including lots of helper functions
* Very fast FEN handling with optional legality check
* Magic bitboard implementation Copyright (C) 2007 Pradyumna Kannan. Converted to C#
* FEN input and output supported
* Chess960 support
* Zobrist key support
* Basic UCI structure
* HiRes timer
* Draw by repetition detection
* Mate validation
* Notation generation
  * Coordinate
  * FAN
  * ICCF
  * LAN
  * RAN
  * SAN
  * SMITH
  * UCI
* Benchmark project for perft
* Custom MoveList data structure
* Pawn blockage algorithm
* Cuckoo repetition algorithm
* Polyglot book support
* Plenty of unit tests to see how it works

### Perft

Perft console test program approximate timings to depth 6 for normal start position

* AMD-FX 8350 = ~12.5 seconds. (without TT) (earlier version)
* Intel i7-8086k = ~3.3 seconds

### Transposition Table

ph

### Move Generator

Example

```c#
// generate all legal moves for current position
const string fen = "rnbqkbnr/1ppQpppp/p2p4/8/8/2P5/PP1PPPPP/RNB1KBNR b KQkq - 1 6";

var game = GameFactory.Create(fen);
var moveList = game.Pos.GenerateMoves();
// ..
```

## What is not included?

* Evaluation (except KPK)
* Search
* Communication using e.i. UCI (base parameter struct supplied though)

## Planned

* Basic chess engine (search + evaluation) w. UCI support

## More Detailed Documentation for Newbies and Beginners

Unit Test "FoolsCheckMateTests.cs"

 Implementation in a C# Console App 


```c#
using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;

namespace ConsoleAppFoolsCheckMateTests
{
    class Program
    {
        static void Main(string[] args)

            //public const string StartPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        {
            //var game = GameFactory.Create(Fen.StartPositionFen);
            var game = GameFactory.Create(Fen.StartPositionFen);
            var position = game.Pos;
            var state = new State();

            var moves = new[]
            {
                Move.Create(Square.F2, Square.F3),
                Move.Create(Square.E7, Square.E5),
                Move.Create(Square.G2, Square.G4),
                Move.Create(Square.D8, Square.H4)
            };

            foreach (var move in moves)
                position.MakeMove(move, state);

            if (position.InCheck)
            {
                var resultingMoves = position.GenerateMoves();
                if (resultingMoves.Length == 0)
                {
                    Console.WriteLine("Checkmate!");
                }
                else
                {
                    Console.WriteLine("In check, but not checkmate.");
                }
            }
            else
            {
                Console.WriteLine("Not in check.");
            }
            Console.ReadKey();
        }
    }
}
```

Unit Test "FenTests.cs"

 Implementation in a C# Console App 


```c#
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.Types;

namespace ConsoleAppFenTests.cs
{
    class Program
    {
        static void Main(string[] args)
        {
         
            /*  Test Fens To Use but you can Try many other
             *  rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
             *  q3k1nr/1pp1nQpp/3p4/1P2p3/4P3/B1PP1b2/B5PP/5K2 b k - 0 17
             */

            Console.WriteLine("Enter a FEN string: ");
            var fen = Console.ReadLine();

            var game = GameFactory.Create(fen);
            var actualFen = game.GetFen().ToString();

            Console.WriteLine("The FEN string for the current position is: " + actualFen);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
```
