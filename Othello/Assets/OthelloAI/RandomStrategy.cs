using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OthelloAI
{
    public class RandomStrategy : IPositionScoreStrategy
    {
        static Random rand = new Random();

        public float GetPositionScore(ulong opponentsDiscs, ulong currentPlayersDiscs)
        {
            return (float)rand.NextDouble();
        }
    }
}
