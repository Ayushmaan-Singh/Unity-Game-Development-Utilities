using System;
using System.Collections.Generic;

namespace AstekUtility
{
    public static class SortingAlgorithms
    {
        #region QuickSort with Custom Comparer (O(n log n) average)

        public static IEnumerable<T> QuickSort<T>(this IEnumerable<T> collection, Comparison<T> comparison)
        {
            List<T> list = collection.ToList();
            QuickSort(list, 0, list.Count - 1, comparison);
            return list;
        }

        private static void QuickSort<T>(IList<T> collection, int left, int right, Comparison<T> comparison)
        {
            if (left < right)
            {
                int pivot = Partition(collection, left, right, comparison);
                QuickSort(collection, left, pivot - 1, comparison);
                QuickSort(collection, pivot + 1, right, comparison);
            }
        }

        private static int Partition<T>(IList<T> collection, int left, int right, Comparison<T> comparison)
        {
            T pivot = collection[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (comparison(collection[j], pivot) <= 0)
                {
                    i++;
                    Swap(collection, i, j);
                }
            }

            Swap(collection, i + 1, right);
            return i + 1;
        }

        #endregion

        #region MergeSort with Custom Comparer (O(n log n) worst-case)

        public static IEnumerable<T> MergeSort<T>(this IEnumerable<T> collection, Comparison<T> comparison)
        {
            List<T> list = collection.ToList();
            MergeSort(list, comparison);
            return list;
        }

        private static void MergeSort<T>(IList<T> list, Comparison<T> comparison)
        {
            if (list.Count <= 1) return;

            int mid = list.Count / 2;
            T[] left = new T[mid];
            T[] right = new T[list.Count - mid];

            for (int i = 0; i < mid; i++)
                left[i] = list[i];

            for (int i = mid; i < list.Count; i++)
                right[i - mid] = list[i];

            MergeSort(left, comparison);
            MergeSort(right, comparison);
            Merge(list, left, right, comparison);
        }

        private static void Merge<T>(IList<T> collection, IList<T> left, IList<T> right, Comparison<T> comparison)
        {
            int i = 0, j = 0, k = 0;

            while (i < left.Count && j < right.Count)
            {
                if (comparison(left[i], right[j]) <= 0)
                    collection[k++] = left[i++];
                else
                    collection[k++] = right[j++];
            }

            while (i < left.Count)
                collection[k++] = left[i++];

            while (j < right.Count)
                collection[k++] = right[j++];
        }

        #endregion

        #region RadixSort with Key Selector (O(kn) for key-based sorting)

        public static void RadixSort<T>(this IList<T> collection, Func<T, int> keySelector)
        {
            if (collection == null || collection.Count == 0) return;

            // Extract keys
            int[] keys = new int[collection.Count];
            for (int i = 0; i < collection.Count; i++)
                keys[i] = keySelector(collection[i]);

            // Create item array for rearranging
            T[] items = new T[collection.Count];
            collection.CopyTo(items, 0);

            // Sort keys and items together
            RadixSortCore(keys, items);

            // Copy sorted items back
            for (int i = 0; i < collection.Count; i++)
                collection[i] = items[i];
        }

        private static void RadixSortCore<T>(int[] keys, T[] items)
        {
            // Separate negatives and positives
            var negativeIndices = new List<int>();
            var positiveIndices = new List<int>();

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] < 0) negativeIndices.Add(i);
                else positiveIndices.Add(i);
            }

            // Process negatives
            if (negativeIndices.Count > 0)
            {
                int[] negativeKeys = new int[negativeIndices.Count];
                T[] negativeItems = new T[negativeIndices.Count];

                for (int i = 0; i < negativeIndices.Count; i++)
                {
                    int idx = negativeIndices[i];
                    negativeKeys[i] = -keys[idx];
                    negativeItems[i] = items[idx];
                }

                RadixSortNonNegative(negativeKeys, negativeItems);
                Array.Reverse(negativeItems);

                // Copy back to original arrays
                for (int i = 0; i < negativeIndices.Count; i++)
                {
                    int idx = negativeIndices[i];
                    items[idx] = negativeItems[i];
                }
            }

            // Process positives
            if (positiveIndices.Count > 0)
            {
                int[] positiveKeys = new int[positiveIndices.Count];
                T[] positiveItems = new T[positiveIndices.Count];

                for (int i = 0; i < positiveIndices.Count; i++)
                {
                    int idx = positiveIndices[i];
                    positiveKeys[i] = keys[idx];
                    positiveItems[i] = items[idx];
                }

                RadixSortNonNegative(positiveKeys, positiveItems);

                // Copy back to original arrays
                for (int i = 0; i < positiveIndices.Count; i++)
                {
                    int idx = positiveIndices[i];
                    items[idx] = positiveItems[i];
                }
            }
        }
        private static void RadixSortNonNegative<T>(int[] keys, T[] items)
        {
            int max = keys.Length > 0 ? keys.Max() : 0;
            for (int exp = 1; max / exp > 0; exp *= 10)
                CountingSort(keys, items, exp);
        }
        private static void CountingSort<T>(int[] keys, T[] items, int exp)
        {
            T[] output = new T[items.Length];
            int[] count = new int[10];
            int[] keyOutput = new int[keys.Length];

            for (int i = 0; i < keys.Length; i++)
                count[(keys[i] / exp) % 10]++;

            for (int i = 1; i < 10; i++)
                count[i] += count[i - 1];

            for (int i = keys.Length - 1; i >= 0; i--)
            {
                int digit = (keys[i] / exp) % 10;
                int position = count[digit] - 1;

                output[position] = items[i];
                keyOutput[position] = keys[i];
                count[digit]--;
            }

            Array.Copy(output, items, items.Length);
            Array.Copy(keyOutput, keys, keys.Length);
        }

        #endregion

        #region Utility Methods

        private static void Swap<T>(IList<T> collection, int i, int j) => (collection[i], collection[j]) = (collection[j], collection[i]);

        #endregion
    }
}