using System;

namespace OthelloAI
{
    public interface IPositionScoreStrategy
    {
        float GetPositionScore(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs);
    }
}
