using System;

namespace OthelloAI
{
    public class RandomHelper
    {
        public static Random rand = new Random();

        public static bool RandomBool()
        {
            int i = rand.Next(2);
            return (i == 1);
        }

        public static float SlightMutationProbability = 0.7f;
        public static float HeavyMutationProbability = 0.2f;
    }
}
