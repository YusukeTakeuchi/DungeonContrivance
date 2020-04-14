using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class MatrixUtil
    {

        // access two-dimensional arrays with a single index
        public class Flattened<T> : IEnumerable<T>
        {
            public T[,] data;

            public Flattened(T[,] matrix)
            {
                data = matrix;
            }

            public int FlattenIndex(int d1, int d2)
            {
                return data.GetLength(1) * d1 + d2;
            }

            public void UnflattenIndex(int idx, out int idxd1, out int idxd2)
            {
                int len = data.GetLength(1);
                idxd1 = idx / len;
                idxd2 = idx % len;
            }

            public int MaxIndex =>
                FlattenIndex(data.GetLength(0) - 1, data.GetLength(1) - 1);

            public void Set(int idx, T val)
            {
                int idxd1, idxd2;
                UnflattenIndex(idx, out idxd1, out idxd2);
                data[idxd1, idxd2] = val;
            }

            public T Get(int idx)
            {
                int idxd1, idxd2;
                UnflattenIndex(idx, out idxd1, out idxd2);
                return data[idxd1, idxd2];
            }

            public IEnumerator<T> GetEnumerator()
            {
                int max = MaxIndex;
                for (int i=0; i<=max; i++)
                {
                    yield return Get(i);
                }
            }

            IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        }
    }
}