using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Util {
    namespace RandomUtil {
        public static class Extensions
        {
            public static T TakeRandom<T>(this IEnumerable<T> e) =>
                e.ElementAt(Random.Range(0, e.Count()));

            public static IEnumerable<T> TakeRandomN<T>(this IEnumerable<T> e, int n) =>
                e.OrderBy(_ => Random.value).Take(n);

            public static int RangeBetweenInclusive(int v1, int v2) =>
                Random.Range(Math.Min(v1, v2), Math.Max(v1, v2) - 1);

        }
    }
}