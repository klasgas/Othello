using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloAI
{
    interface IPositionScoreStrategy
    {
        float GetPositionScore(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs);
    }
}
