﻿/*
ChessLib, a chess data structure library

MIT License

Copyright (c) 2017-2018 Rudy Alex Kohn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Rudz.Chess;

namespace ChessLibTest
{
    using NUnit.Framework;
    using Rudz.Chess.Enums;
    using Rudz.Chess.Types;

    [TestFixture]
    public class BitboardTests
    {

        [Test]
        public void MakeBitBoardTest()
        {
            // a few squares
            BitBoard b1 = BitBoards.MakeBitboard(ESquare.a1, ESquare.b1, ESquare.a2, ESquare.b2);
            BitBoard b2 = ESquare.a1.BitBoardSquare() | ESquare.b1.BitBoardSquare() | ESquare.a2.BitBoardSquare() | ESquare.b2.BitBoardSquare();
            Assert.AreEqual(b1, b2);
            
            // a single square (not needed, but still has to work in case of list of squares etc)
            BitBoard b3 = BitBoards.MakeBitboard(ESquare.h3);
            BitBoard b4 = ESquare.h3.BitBoardSquare();
            Assert.AreEqual(b3, b4);
        }
        
    }
}