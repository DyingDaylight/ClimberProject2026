using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    public class GameRandom
    {
        private static System.Random random = new();

        
        public static void SetSeed(int seed)
        {
            random = new System.Random(seed);
        }

        public static int Range(int minInclusive, int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }

        public static float Range(float minInclusive, float maxInclusive)
        {
            return Mathf.Lerp(
                minInclusive,
                maxInclusive,
                (float)random.NextDouble());
        }

        public static bool Chance(float probability)
        {
            return random.NextDouble() < probability;
        }

        public static float Value()
        {
            return (float)random.NextDouble();
        }

        public static T Choose<T>(IReadOnlyList<T> collection)
        {
            return collection[Range(0, collection.Count)];
        }
    }
}