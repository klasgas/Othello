using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;

namespace OthelloAI
{
    [DataContract]
    public class WeightedStrategy : IPositionScoreStrategy
    {
        static Random rand = new Random();

        // for tournament
        //[DataMember]
        //public Guid guid;

        //[DataMember]
//        public float tournamentScore;

        private const int weightsCount = 4;

        [DataMember]
        private float WeightPotentialFlips;

        [DataMember]
        private float WeightPotentialMoves;

        [DataMember]
        private float WeightDiscCount;

        [DataMember]
        private float WeightForCorners;

        public WeightedStrategy()
        {
            //guid = Guid.NewGuid();
        }

        public WeightedStrategy(float[] weights)
            : this()
        {
            SetWeights(weights);
        }

        public float GetPositionScore(UInt64 opponentsDiscs, UInt64 currentPlayersDiscs)
        {
            int opponentDiscCount = MoveFinder.PopulationCount(opponentsDiscs);
            int playerDiscCount = MoveFinder.PopulationCount(currentPlayersDiscs);

            UInt64 emptySquaresNextToOpponent = MoveFinder.EmptySquaresNextToOpponent(opponentsDiscs, currentPlayersDiscs);
            int potentialMovesCount;  
            int potentialFlipsCount = MoveFinder.PotententialFlipsCount(opponentsDiscs, currentPlayersDiscs, emptySquaresNextToOpponent, out potentialMovesCount);

            UInt64 emptySquaresNextToPlayer = MoveFinder.EmptySquaresNextToOpponent(currentPlayersDiscs, opponentsDiscs);
            int potentialMovesCountForOpponent;
            int potentialFlipsCountForOpponent = MoveFinder.PotententialFlipsCount(currentPlayersDiscs, opponentsDiscs, emptySquaresNextToPlayer, out potentialMovesCountForOpponent);

            if (potentialMovesCount == 0)
            {
                if(!MoveFinder.LegalMoveExists(currentPlayersDiscs, opponentsDiscs))
                {
                    if(opponentDiscCount > playerDiscCount)
                    {
                        return float.NegativeInfinity;
                    }
                    else if(opponentDiscCount < playerDiscCount)
                    {
                        return float.PositiveInfinity;
                    }
                    else
                    {
                        return 0; // ScoreForDraw 
                    }
                }
                else
                {
                    // player must pass
                }
            }

            int playerCornerCount = MoveFinder.CornerCount(currentPlayersDiscs);
            int opponentCornerCount = MoveFinder.CornerCount(currentPlayersDiscs);

            float score =
               playerDiscCount * WeightDiscCount
            - (opponentDiscCount * WeightDiscCount)
            + playerCornerCount * WeightForCorners
            - (opponentCornerCount * WeightForCorners)
            + potentialMovesCount * WeightPotentialMoves
            - (potentialMovesCountForOpponent * WeightPotentialMoves)
            + potentialFlipsCount * WeightPotentialFlips
            - (potentialFlipsCountForOpponent * WeightPotentialFlips);

            return score;
        }

        public void InitRandomWeights()
        {
            float[] weights = new float[weightsCount];
            for (int i = 0; i < weightsCount; i++)
            {
                weights[i] = (float)rand.NextDouble();
            }

            SetWeights(weights);
        }

        public void SetWeights(float[] weights)
        {
            WeightPotentialFlips = weights[0];
            WeightPotentialMoves = weights[1];
            WeightDiscCount = weights[2];
            WeightForCorners = weights[3];
        }

        public float[] GetWeights()
        {
            float[] weights = new float[weightsCount];
            weights[0] = WeightPotentialFlips;
            weights[1] = WeightPotentialMoves;
            weights[2] = WeightDiscCount;
            weights[3] = WeightForCorners;

            return weights;
        }

        public void Mutate()
        {
            float[] weights = GetWeights();
            // only mutate one weight
            int index = rand.Next(weightsCount);

            weights[index] = (float)rand.NextDouble();

            SetWeights(weights);
        }

        public void MutateSlightly()
        {
            float[] weights = GetWeights();
            // only mutate one weight
            int index = rand.Next(weightsCount);

            float value = weights[index];

            SetWeights(weights);

            // do tiny adjustments?
            //float value = weights[index];
            if (RandomHelper.RandomBool())
            {
                value += (float)rand.NextDouble() * 0.05f;
            }
            else
            {
                value -= (float)rand.NextDouble() * 0.05f;
            }

            // clip to interval 0..1
            value = Math.Max(0, value);
            value = Math.Min(1, value);

            weights[index] = value;

            SetWeights(weights);
        }

        public WeightedStrategy Crossover(WeightedStrategy other)
        {
            float[] weightsA = GetWeights();
            float[] weightsB = other.GetWeights();
            float[] weightsC = new float[weightsCount];

            for (int i = 0; i < weightsCount; i++)
            {
                if(RandomHelper.RandomBool())
                {
                    weightsC[i] = weightsA[i];
                }
                else
                {
                    weightsC[i] = weightsB[i];
                }
            }

            var child = new WeightedStrategy(weightsC);

            var randomValue = RandomHelper.rand.NextDouble();
            if (randomValue < RandomHelper.HeavyMutationProbability)
            {
                child.Mutate();
            }
            else if (randomValue < RandomHelper.SlightMutationProbability)
            {
                child.MutateSlightly();
            }

            return child;
        }
    }
}
