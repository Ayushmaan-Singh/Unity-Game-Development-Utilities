using System.Collections.Generic;

namespace AstekUtility
{
	public static class ListExtension
	{
		public static List<T[]> KCombinationOfSize<T>(this List<T> collection, int combinationSize)
		{
			List<T[]> results = new List<T[]>();
			List<T> copy = new List<T>(collection);
			PossibleCombinations(copy, new T[combinationSize], 0, collection.Count - 1, 0, combinationSize, results);
			return results;
		}

		private static void PossibleCombinations<T>(List<T> collection, T[] data, int start, int end, int index, int r, List<T[]> storage)
		{
			// Base case: if index equals r, print the combination
			if (index == r)
			{
				storage.Add((T[])data.Clone());
				return;
			}

			// Generate combinations recursively
			for (int i = start; i <= end && end - i + 1 >= r - index; i++)
			{
				data[index] = collection[i];                                               // Fill data [] with elements of arr [] for the current combination
				PossibleCombinations(collection, data, i + 1, end, index + 1, r, storage); // Recursively generate combinations
			}
		}
	}
}