using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AstekUtility
{
	public static class LINQExtension
	{
		/// <summary>
		/// Change this value to set at which threshold to perform operations in parallel
		/// </summary>
		public static int ThresholdForParallelExecution = 200;

		/// <summary>
		/// execute action on a IEnumerable of type T either sequentially or parallely, if count of item is less than 200 prefer sequentially
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="action"></param>
		/// <param name="executeSequentially"> wether to run the action sequentially or parallel</param>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="ArgumentNullException"></exception>
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action, bool executeSequentially = true)
		{
			if (sequence == null || action == null)
				throw new ArgumentNullException(sequence == null ? $"Collection:{nameof(sequence)}" : $"Condition:{nameof(action)}");

			if (executeSequentially)
			{
				foreach (T item in sequence)
				{
					action.Invoke(item);
				}
			}
			else
			{
				Parallel.ForEach(sequence, action.Invoke);
			}
		}

		// public static void For<T>(this IEnumerable<T> sequence, int startIndex, int count, ref int index, Action<int, T> action)
		// {
		// 	if (sequence == null || action == null)
		// 		throw new ArgumentNullException(sequence == null ? $"Collection:{nameof(sequence)}" : $"Condition:{nameof(action)}");
		//
		// 	List<T> collection = sequence.ToList();
		// 	for (index = startIndex; index < count; index++)
		// 	{
		// 		action?.Invoke(index, collection[index]);
		// 	}
		// }

		public static IEnumerable<T> Where<T>(this IEnumerable<T> sequence, Func<T, bool> action)
		{
			if (sequence == null || action == null)
				throw new ArgumentNullException(sequence == null ? $"Collection:{nameof(sequence)}" : $"Condition:{nameof(action)}");

			List<T> collection = sequence.ToList();
			ConcurrentBag<T> results = new ConcurrentBag<T>();
			collection.ForEach(item =>
			{
				if (action.Invoke(item))
					results.Add(item);
			}, executeSequentially:collection.Count < ThresholdForParallelExecution);
			return results;
		}

		public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
		{
			if (sequence == null || predicate == null)
				throw new ArgumentNullException(sequence == null ? $"Collection:{nameof(sequence)}" : $"Condition:{nameof(predicate)}");

			List<T> collection = sequence.ToList();
			List<T> toRemove = collection.Where(predicate).ToList();
			toRemove.ForEach(item =>
			{
				collection.Remove(item);
			}, executeSequentially:toRemove.Count < ThresholdForParallelExecution);
			return collection;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="predicate"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns> count of items removed</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static int RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate)
		{
			if (collection == null || predicate == null)
				throw new ArgumentNullException(collection == null ? $"Collection:{nameof(collection)}" : $"Condition:{nameof(predicate)}");

			int removedCount = 0;
			List<T> toRemove = collection.Where(predicate).ToList();
			toRemove.ForEach(item =>
			{
				collection.Remove(item);
				removedCount++;
			}, executeSequentially:toRemove.Count < ThresholdForParallelExecution);
			return removedCount;
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> sequence, Func<TSource, TResult> selector)
		{
			if (sequence == null || selector == null)
				throw new ArgumentNullException(sequence == null ? $"Collection:{nameof(sequence)}" : $"Condition:{nameof(selector)}");

			List<TSource> collection = sequence.ToList();
			ConcurrentBag<TResult> results = new ConcurrentBag<TResult>();
			collection.ForEach(item => results.Add(selector(item)), executeSequentially:collection.Count < ThresholdForParallelExecution);
			return results;
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source,
			Func<TSource, IEnumerable<TCollection>> collectionSelector,
			Func<TSource, TCollection, TResult> resultSelector)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
			if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

			List<TSource> collection = source.ToList();
			ConcurrentBag<TResult> results = new ConcurrentBag<TResult>();
			collection.ForEach(item =>
			{
				List<TCollection> collectionSource = collectionSelector(item).ToList();
				collectionSource.ForEach(collectionItem =>
				{
					results.Add(resultSelector(item, collectionItem));
				}, executeSequentially:collectionSource.Count < ThresholdForParallelExecution);
			}, executeSequentially:collection.Count < ThresholdForParallelExecution);
			return results;
		}

		public static bool All<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			foreach (T item in sequence)
			{
				if (!predicate.Invoke(item)) return false;
			}
			return true;
		}

		public static bool Any<T>(this IEnumerable<T> sequence, Func<T, bool> predicate = null)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			foreach (T item in sequence)
			{
				if (predicate == null) return true;
				if (predicate.Invoke(item)) return true;
			}
			return false;
		}

		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable<object> source)
		{
			List<object> collection = source.ToList();
			ConcurrentBag<TResult> results = new ConcurrentBag<TResult>();
			collection.ForEach(item =>
			{
				if (item is TResult result)
					results.Add(result);
			}, executeSequentially:collection.Count < ThresholdForParallelExecution);
			return results;
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			if (first == null || second == null)
				throw new ArgumentNullException(first == null ? nameof(first) : nameof(second));

			HashSet<T> set = new HashSet<T>(second);
			foreach (T item in first)
			{
				if (!set.Contains(item)) yield return item;
			}
		}

		public static List<T> ToList<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			List<T> collection = new List<T>();
			sequence.ForEach(item => collection.Add(item));
			return collection;
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			HashSet<T> collection = new HashSet<T>();
			sequence.ForEach(item => collection.Add(item));
			return collection;
		}

		public static T[] ToArray<T>(this IEnumerable<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			List<T> resultList = new List<T>();
			foreach (T item in sequence)
			{
				resultList.Add(item);
			}
			T[] resultArray = new T[resultList.Count];
			for (int i = 0; i < resultList.Count; i++)
			{
				resultArray[i] = resultList[i];
			}
			return resultArray;
		}

		public static T[] ToArray<T>(this ICollection<T> sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			T[] array = new T[sequence.Count];
			int i = 0;
			sequence.ForEach(item =>
			{
				array[i] = item;
				i++;
			});
			return array;
		}

		public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			List<T> collection = source.ToList();
			int count = collection.Count;
			for (int i = 0; i < count; i++)
			{
				if (predicate.Invoke(collection[i]))
				{
					return collection[i];
				}
			}

			return default(T);
		}

		public static T First<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			List<T> collection = source.ToList();
			return collection.Count > 0 ? collection[0] : default(T);
		}

		public static T LastOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			List<T> collection = source.ToList();
			int count = collection.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				if (predicate.Invoke(collection[i]))
				{
					return collection[i];
				}
			}

			return default(T);
		}

		public static T Last<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			List<T> collection = source.ToList();
			return collection.Count > 0 ? collection[^1] : default(T);
		}

		public static int FindIndex<T>(this ICollection<T> source, Func<T, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			for (int i = 0; i < System.Linq.Enumerable.Count(source); i++)
			{
				if (predicate(System.Linq.Enumerable.ElementAt(source, i))) { return i; }
			}
			return -1; // Return -1 if the element is not found
		}

		public static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));
			return source.Any(predicate);
		}

		public static List<T> Reverse<T>(this IEnumerable<T> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			List<T> collection = new List<T>();
			List<T> sourceToCollection = source.ToList();

			for (int i = sourceToCollection.Count - 1; i >= 0; i--)
				collection.Add(sourceToCollection[i]);

			return collection;
		}

		public static IEnumerable<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			if (first == null || second == null)
				throw new ArgumentNullException("Collections cannot be null");

			HashSet<T> seen = new HashSet<T>();

			foreach (T item in first)
			{
				if (seen.Add(item))
				{
					yield return item;
				}
			}

			foreach (T item in second)
			{
				if (seen.Add(item))
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<T> Intersection<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			if (first == null || second == null)
				throw new ArgumentNullException("Collections cannot be null");

			HashSet<T> secondSet = new HashSet<T>(second);
			HashSet<T> resultSet = new HashSet<T>();

			foreach (T item in first)
			{
				if (secondSet.Contains(item) && resultSet.Add(item))
				{
					yield return item;
				}
			}
		}
	}
}