using System;
using System.Collections.Generic;

namespace OthelloAI
{
    public class MoveFinder
    {
        public IPositionScoreStrategy _positionScoreStrategy;

        static UInt64 leftmostFileExcluded = 0x7f7f7f7f7f7f7f7f;

        static UInt64 rightmostFileExcluded = 0xfefefefefefefefe;

        static UInt64 corners = 0x8100000000000081;

        public static Random Random = new Random();

        public MoveFinder(IPositionScoreStrategy strategy)
        {
            _positionScoreStrategy = strategy;
        }

        public UInt64 FindMove(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs)
        {
            uint searchDepth = 3;
            UInt64 potentialMoves = EmptySquaresNextToOpponent(opponentsDiscs, currentPlayersDiscs);
            UInt64 move = 1;
            float bestScore = float.NegativeInfinity;

            List<UInt64> bestMoves = new List<UInt64>();

            while (move != 0)
            {
                if ((move & potentialMoves) != 0)
                {
                    UInt64 discsToFlip = GetDiscsToFlip(opponentsDiscs, currentPlayersDiscs, move);
                    // this is a legal move?
                    if (discsToFlip != 0)
                    {
                        UInt64 currentPlayersDiscsAfterMove = currentPlayersDiscs | discsToFlip | move;
                        UInt64 opponentsDiscsAfterMove = opponentsDiscs & ~discsToFlip;

                        float score = -MinMaxSearch(currentPlayersDiscsAfterMove, opponentsDiscsAfterMove, searchDepth - 1);

                        if(score == bestScore)
                        {
                            bestMoves.Add(move);
                        }
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMoves.Clear();
                            bestMoves.Add(move);
                        }
                    }
                }

                move = move << 1;
            }

            if(bestMoves.Count == 0)
            {
                return 0;
            }
            else
            {
                int randomIndex = RandomHelper.rand.Next(bestMoves.Count);
                return bestMoves[randomIndex];
            }
        }

        public float MinMaxSearch(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs, uint depth)
        {
            float bestScore = float.NegativeInfinity;

            if (depth == 0)
            {
                return _positionScoreStrategy.GetPositionScore(opponentsDiscs, currentPlayersDiscs);
            }
            else
            {
                UInt64 potentialMoves = EmptySquaresNextToOpponent(opponentsDiscs, currentPlayersDiscs);
                UInt64 move = 1;
                UInt64 bestMove = 0; // 0 means no move found = player must pass.

                while (move != 0)
                {
                    if ((move & potentialMoves) != 0)
                    {
                        UInt64 discsToFlip = GetDiscsToFlip(opponentsDiscs, currentPlayersDiscs, move);
                        // this is a legal move?
                        if (discsToFlip != 0)
                        {
                            UInt64 currentPlayersDiscsAfterMove = currentPlayersDiscs | discsToFlip | move;
                            UInt64 opponentsDiscsAfterMove = opponentsDiscs & ~discsToFlip;

                            float score = -MinMaxSearch(currentPlayersDiscsAfterMove, opponentsDiscsAfterMove, depth - 1);
                            if (score > bestScore)
                            {
                                bestScore = score;
                                bestMove = move;
                            }
                        }
                    }

                    move = move << 1;
                }
            }
            return bestScore;
        }

        public static int PopulationCount(UInt64 x)
        {
            // popcount64d from https://en.wikipedia.org/wiki/Hamming_weight
            // assuming most bits will be zero.

            int count;
            for (count = 0; x != 0; count++)
                x &= x - 1;
            return count;
        }


        private static void FindMoves(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs)
        {
            UInt64 potentialMoves = EmptySquaresNextToOpponent(opponentsDiscs, currentPlayersDiscs);
            UInt64 move = 1;

            while (move != 0)
            {
                if ((move & potentialMoves) != 0)
                {
                    UInt64 discsToFlip = GetDiscsToFlip(opponentsDiscs, currentPlayersDiscs, move);

                    if (discsToFlip != 0)
                    {
                        currentPlayersDiscs = currentPlayersDiscs | discsToFlip | move;
                        opponentsDiscs = opponentsDiscs & ~discsToFlip;
                    }
                }

                move = move << 1;
            }
        }

        public static bool LegalMoveExists(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs)
        {
            UInt64 potentialMoves = EmptySquaresNextToOpponent(opponentsDiscs, currentPlayersDiscs);
            UInt64 move = 1;

            while (move != 0)
            {
                if ((move & potentialMoves) != 0)
                {
                    UInt64 discsToFlip = GetDiscsToFlip(opponentsDiscs, currentPlayersDiscs, move);

                    if (discsToFlip != 0)
                    {
                        return true;
                    }
                }

                move = move << 1;
            }

            return false;
        }

        public static int PotententialFlipsCount(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs, UInt64 emptySquaresNextToOponent, out int actualMoveCount)
        {
            actualMoveCount = 0;
            UInt64 potentialFlips = 0;

            UInt64 move = 1;

            while (move != 0)
            {
                if ((move & emptySquaresNextToOponent) != 0)
                {
                    UInt64 discsToFlip = GetDiscsToFlip(opponentsDiscs, currentPlayersDiscs, move);

                    if (discsToFlip != 0)
                    {
                        potentialFlips = potentialFlips | discsToFlip;
                        actualMoveCount++;
                    }
                }

                move = move << 1;
            }

            return PopulationCount(potentialFlips);
        }

        public static UInt64 GetDiscsToFlip(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs, UInt64 move)
        {
            UInt64 discsToFlip = 0;

            // kan vi vända brickor åt höger?
            UInt64 square = move >> 1;
            {
                UInt64 discsToFlipToTheRight = 0;
                UInt64 opponentsDiscsExcudingLeftmostFile = opponentsDiscs & leftmostFileExcluded;

                bool foundOpponentToTheRight = false;
                while ((square & opponentsDiscsExcudingLeftmostFile) != 0)
                {
                    foundOpponentToTheRight = true;
                    discsToFlipToTheRight = discsToFlipToTheRight | square;
                    square = square >> 1;
                }

                if (foundOpponentToTheRight)
                {
                    // players disc after opponents disc(s) to the right?
                    if ((square & (currentPlayersDiscs & leftmostFileExcluded)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipToTheRight;
                        //Console.WriteLine("Jag kan vända brickor åt höger");
                    }
                }

            }

            // kan vi vända brickor neråthöger?
            {
                UInt64 discsToFlipSouthEast = 0;
                square = move >> 9;
                UInt64 opponentsDiscsExcudingLeftmostFile = opponentsDiscs & leftmostFileExcluded;

                bool foundOpponentToTheSouthEast = false;
                while ((square & opponentsDiscsExcudingLeftmostFile) != 0)
                {
                    foundOpponentToTheSouthEast = true;
                    discsToFlipSouthEast = discsToFlipSouthEast | square;
                    square = square >> 9;
                }

                if (foundOpponentToTheSouthEast)
                {
                    // players disc after opponents disc(s) to the right?
                    if ((square & (currentPlayersDiscs & leftmostFileExcluded)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipSouthEast;
                        //Console.WriteLine("Jag kan vända brickor neråthöger");
                    }
                }
            }

            // kan vi vända brickor neråtvänster? 
            {
                UInt64 discsToFlipSouthWest = 0;
                square = move >> 7;
                UInt64 opponentsDiscsExcudingRightmostFile = opponentsDiscs & rightmostFileExcluded;

                bool foundOpponentToTheSouthWest = false;
                while ((square & opponentsDiscsExcudingRightmostFile) != 0)
                {
                    foundOpponentToTheSouthWest = true;
                    discsToFlipSouthWest = discsToFlipSouthWest | square;
                    square = square >> 7;
                }

                if (foundOpponentToTheSouthWest)
                {
                    // players disc after opponents disc(s) to the right?
                    if ((square & (currentPlayersDiscs & rightmostFileExcluded)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipSouthWest;
                        //Console.WriteLine("Jag kan vända brickor neråtvänster");
                    }
                }
            }


            // kan vi vända brickor uppåtvänster?
            {
                UInt64 discsToFlipNorthWest = 0;
                square = move << 9;
                UInt64 opponentsDiscsExcudingRightmostFile = opponentsDiscs & rightmostFileExcluded;

                bool foundOpponentToTheNorthWest = false;
                while ((square & opponentsDiscsExcudingRightmostFile) != 0)
                {
                    foundOpponentToTheNorthWest = true;
                    discsToFlipNorthWest = discsToFlipNorthWest | square;
                    square = square << 9;
                }

                if (foundOpponentToTheNorthWest)
                {
                    // players disc after opponents disc(s) to the right?
                    if ((square & (currentPlayersDiscs & rightmostFileExcluded)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipNorthWest;
                        //Console.WriteLine("Jag kan vända brickor uppåtvänster");
                    }
                }
            }

            // kan vi vända brickor uppåthöger?
            {
                UInt64 discsToFlipNorthEast = 0;
                square = move << 7;
                UInt64 opponentsDiscsExcudingLeftmostFile = opponentsDiscs & leftmostFileExcluded;

                bool foundOpponentToTheNorthEast = false;
                while ((square & opponentsDiscsExcudingLeftmostFile) != 0)
                {
                    foundOpponentToTheNorthEast = true;
                    discsToFlipNorthEast = discsToFlipNorthEast | square;
                    square = square << 7;
                }

                if (foundOpponentToTheNorthEast)
                {
                    // players disc after opponents disc(s) to the northeast?
                    if ((square & (currentPlayersDiscs & leftmostFileExcluded)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipNorthEast;
                        //Console.WriteLine("Jag kan vända brickor uppåthöger");
                    }
                }
            }

            // kan vi vända brickor åt vänster?
            {
                UInt64 discsToFlipToTheLeft = 0;
                square = move << 1;
                UInt64 opponentsDiscsExcudingRightmostFile = opponentsDiscs & rightmostFileExcluded;

                bool foundOpponentToTheLeft = false;
                while ((square & opponentsDiscsExcudingRightmostFile) != 0)
                {
                    foundOpponentToTheLeft = true;
                    discsToFlipToTheLeft = discsToFlipToTheLeft | square;
                    square = square << 1;
                }

                if (foundOpponentToTheLeft)
                {
                    // players disc after opponents disc(s) to the right?
                    if ((square & (currentPlayersDiscs & rightmostFileExcluded)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipToTheLeft;
                        //Console.WriteLine("Jag kan vända brickor åt vänster");
                    }
                }
            }

            // Kan vi vända brickor uppåt?
            {
                UInt64 discsToFlipAbove = 0;
                square = move << 8;

                bool foundOpponentAbove = false;
                while ((square & opponentsDiscs) != 0)
                {
                    foundOpponentAbove = true;
                    discsToFlipAbove = discsToFlipAbove | square;
                    square = square << 8;
                }

                if (foundOpponentAbove)
                {
                    // players disc after opponents disc(s) above?
                    if ((square & (currentPlayersDiscs)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipAbove;
                        //Console.WriteLine("Jag kan vända brickor uppåt");
                    }
                }
            }

            // Kan vi vända brickor neråt?
            {
                UInt64 discsToFlipBelow = 0;
                square = move >> 8;

                bool foundOpponentBelow = false;
                while ((square & opponentsDiscs) != 0)
                {
                    foundOpponentBelow = true;
                    discsToFlipBelow = discsToFlipBelow | square;
                    square = square >> 8;
                }

                if (foundOpponentBelow)
                {
                    // players disc after opponents disc(s) below?
                    if ((square & (currentPlayersDiscs)) != 0)
                    {
                        discsToFlip = discsToFlip | discsToFlipBelow;
                        //Console.WriteLine("Jag kan vända brickor neråt");
                    }
                }
            }

            return discsToFlip;
        }

        public static int CornerCount(UInt64 discs)
        {
            UInt64 cornersWithDiscs = corners & discs;

            return PopulationCount(cornersWithDiscs);
        }

        public static UInt64 EmptySquaresNextToOpponent(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs)
        {
            UInt64 occupiedSquares = opponentsDiscs | currentPlayersDiscs;

            UInt64 emptySquares = ~occupiedSquares;

            UInt64 emptySquaresToTheRightOfOpponentDisc = ((opponentsDiscs & rightmostFileExcluded) >> 1) & emptySquares;

            UInt64 emptySquaresToTheLeftAndAboveOfOpponentDisc = ((opponentsDiscs & leftmostFileExcluded) << 9) & emptySquares;
            UInt64 emptySquaresToTheRightAndAboveOfOpponentDisc = ((opponentsDiscs & rightmostFileExcluded) << 7) & emptySquares;

            UInt64 emptySquaresToTheLeftOfOpponentDisc = ((opponentsDiscs & leftmostFileExcluded) << 1) & emptySquares;

            UInt64 emptySquaresAboveOpponentDisc = (opponentsDiscs << 8) & emptySquares;

            UInt64 emptySquaresBelowOpponentDisc = (opponentsDiscs >> 8) & emptySquares;

            UInt64 emptySquaresToTheRightAndBelowOfOpponentDisc = ((opponentsDiscs & rightmostFileExcluded) >> 9) & emptySquares;
            UInt64 emptySquaresToTheLeftAndBelowOfOpponentDisc = ((opponentsDiscs & leftmostFileExcluded) >> 7) & emptySquares;

            return emptySquaresToTheLeftOfOpponentDisc |
            emptySquaresToTheRightOfOpponentDisc |
            emptySquaresToTheLeftAndAboveOfOpponentDisc |
            emptySquaresAboveOpponentDisc |
            emptySquaresBelowOpponentDisc |
            emptySquaresToTheRightAndAboveOfOpponentDisc |
            emptySquaresToTheLeftAndBelowOfOpponentDisc |
            emptySquaresToTheRightAndBelowOfOpponentDisc;
        }

        public static void MakeMove(ref UInt64 opponentsDiscs, ref UInt64 currentPlayersDiscs, UInt64 move)
        {
            UInt64 discsToFlip = GetDiscsToFlip(opponentsDiscs, currentPlayersDiscs, move);
            // this is a legal move?
            if (discsToFlip != 0)
            {
                currentPlayersDiscs = currentPlayersDiscs | discsToFlip | move;
                opponentsDiscs = opponentsDiscs & ~discsToFlip;
            }
        }

        //private static bool IsEdgeSquare(UInt64 square)
        //{
        //    return (square & edgeSquares) != 0;
        //}
    }
}
