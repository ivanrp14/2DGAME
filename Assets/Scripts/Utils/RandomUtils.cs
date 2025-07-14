using System.Collections.Generic;
using UnityEngine;
namespace Utils
{
    public static class RandomUtils
    {
        public static List<T> ChooseRandomElements<T>(List<T> list, int count)
        {
            List<T> copy = new List<T>(list);
            List<T> result = new List<T>();

            // Limitar si la lista tiene menos elementos que el count
            int finalCount = Mathf.Min(count, copy.Count);

            for (int i = 0; i < finalCount; i++)
            {
                int index = Random.Range(0, copy.Count);
                result.Add(copy[index]);
                copy.RemoveAt(index);
            }

            return result;
        }
    }
}