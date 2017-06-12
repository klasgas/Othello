using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace OthelloAI
{
    [DataContract]
    public class DoubleStrategy : IPositionScoreStrategy
    {
        // for tournament
        [DataMember]
        public Guid guid;
        [DataMember]
        public float tournamentScore;

        [DataMember]
        public int tournamentWins;


        [DataMember]
        private WeightedStrategy _openingStrategy;

        [DataMember]
        private WeightedStrategy _endgameStrategy;

        [DataMember]
        private int _endGameStartsAtDiscCount;


        public DoubleStrategy()
        {
            guid = Guid.NewGuid();
            _openingStrategy = new WeightedStrategy();
            _endgameStrategy = new WeightedStrategy();
        }

        public float GetPositionScore(ulong opponentsDiscs, ulong currentPlayersDiscs)
        {
            int totalDiscCount = MoveFinder.PopulationCount(opponentsDiscs | currentPlayersDiscs);

            if (totalDiscCount < _endGameStartsAtDiscCount)
            {
                return _openingStrategy.GetPositionScore(opponentsDiscs, currentPlayersDiscs);
            }
            else
            {
                return _endgameStrategy.GetPositionScore(opponentsDiscs, currentPlayersDiscs);
            }
        }

        public void InitRandomWeights()
        {
            _openingStrategy.InitRandomWeights();
            _endgameStrategy.InitRandomWeights();

            _endGameStartsAtDiscCount = (4 + RandomHelper.rand.Next(0, 60));
        }

        public DoubleStrategy Crossover(DoubleStrategy other)
        {
            var child = new DoubleStrategy();

            child._openingStrategy = _openingStrategy.Crossover(other._openingStrategy);
            child._endgameStrategy = _endgameStrategy.Crossover(other._endgameStrategy);

            if (RandomHelper.RandomBool())
            {
                child._endGameStartsAtDiscCount = this._endGameStartsAtDiscCount;
            }
            else
            {
                child._endGameStartsAtDiscCount = other._endGameStartsAtDiscCount;
            }

            return child;
        }

        public void Mutate()
        {
            var r = RandomHelper.rand.Next(3);
            switch(r)
            {
                case 0:
                    MutateSlightly(5);
                    break;
                case 1:
                    _openingStrategy.Mutate();
                    break;
                case 2:
                    _endgameStrategy.Mutate();
                    break;
            }
        }

        public void MutateSlightly(int amount = 1)
        {
            if(RandomHelper.RandomBool())
            {
                _endGameStartsAtDiscCount += amount;
            }
            else
            {
                _endGameStartsAtDiscCount -= amount;
            }

            _endGameStartsAtDiscCount = Math.Max(_endGameStartsAtDiscCount, 4);
            _endGameStartsAtDiscCount = Math.Min(_endGameStartsAtDiscCount, 42);
        }
    }

}