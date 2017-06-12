using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GeneticAlgorithm;

namespace OthelloAI
{
    class Program
    {

        static void TestTournament()
        {
            var tournament = new GeneticTournament();

            tournament.Run();
        }


        static void Main(string[] args)
        {
            TestTournament();
            //TestCorners();
            return;

            string sOpponent =
                @"00000000
                00000000
                00000000
                00010000
                00001000
                00000000
                00000000
                00000000";

                        string sPlayer =
                @"00000000
                00000000
                00000000
                00001000
                00010000
                00000000
                00000000
                00000000";

            UInt64 bbOpponent = StringToBitboard(sOpponent);
            //Print(bbOpponent);

            UInt64 bbPlayer = StringToBitboard(sPlayer);
            //Print(bbPlayer);

            while(true)
            {
                PrintOthello(bbOpponent, bbPlayer);
                PlayGame(bbOpponent, bbPlayer);
                Thread.Sleep(1000);
            }
        }

        static void TestCorners()
        {
            string sOpponent =
                @"10000001
                00000000
                00000000
                00010000
                00001000
                00000000
                00000000
                10000001";

            UInt64 bbOpponent = StringToBitboard(sOpponent);

            int count = MoveFinder.CornerCount(bbOpponent);

            Console.WriteLine(count);

            Console.ReadKey();
        }

        private static void PlayGame(UInt64 black, UInt64 white)
        {
            bool blackFoundMove = true;
            bool whiteFoundMove = false;

            MoveFinder moveFinder = new MoveFinder(new RandomStrategy());

            while (blackFoundMove || whiteFoundMove)
            {
                UInt64 blackMove = moveFinder.FindMove(white, black);
                blackFoundMove = (blackMove != 0);
                if (blackFoundMove)
                {
                    MoveFinder.MakeMove(ref white, ref black, blackMove);
                    PrintOthello(black, white);
                }

                UInt64 whiteMove = moveFinder.FindMove(black, white);
                whiteFoundMove = (whiteMove != 0);
                if (whiteFoundMove)
                {
                    MoveFinder.MakeMove(ref black, ref white, whiteMove);
                    PrintOthello(black, white);
                }
            }
        }

        private static void PrintOthello(UInt64 black, UInt64 white)
        {
            string whitestring = BitboardToString(white);
            string blackstring = BitboardToString(black);

            var sb = new StringBuilder();
            for (int i = 0; i < blackstring.Length; i++)
            {
                char w = whitestring[i];
                char b = blackstring[i];

                if (w == '1')
                {
                    sb.Append("o");
                }
                else if (b == '1')
                {
                    sb.Append("x");
                }
                else if (b == '0')
                {
                    sb.Append("_");
                }
                else
                {
                    sb.Append(b);
                }
            }

            Console.Clear();
            Console.WriteLine(sb.ToString());
            Thread.Sleep(10);
        }

        private static void Print(UInt64 bitboard)
        {
            Console.WriteLine(BitboardToString(bitboard));
        }

        private static UInt64 StringToBitboard(string s)
        {
            s = Regex.Replace(s, @"\s", ""); // remove spaces (line breaks)
            return Convert.ToUInt64(s, 2);
        }

        private static string BitboardToString(UInt64 value)
        {
            string s = Convert.ToString((long)value, 2);
            s = s.PadLeft(64, '0');

            var sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                sb.AppendLine(s.Substring(i * 8, 8));
            }

            return sb.ToString();
        }
    }
}
 