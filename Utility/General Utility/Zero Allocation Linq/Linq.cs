using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AstekUtility.ZeroAllocLinqInternal;

namespace AstekUtility
{
    public static class ZeroAllocLinq
    {
        // Main entry point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZeroAllocEnumerable<TSource> AsZeroAlloc<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ZeroAllocEnumerable<TSource>(source);
        }

        // Array optimizations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZeroAllocEnumerable<TSource> AsZeroAlloc<TSource>(this TSource[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ZeroAllocEnumerable<TSource>(source);
        }

        // List optimizations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZeroAllocEnumerable<TSource> AsZeroAlloc<TSource>(this List<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ZeroAllocEnumerable<TSource>(source);
        }

        #region Filtering/Projection Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IZeroAllocEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            return new WhereEnumerable<TSource>(source, predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectEnumerable<TSource, TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new SelectEnumerable<TSource, TResult>(source, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectManyEnumerable<TSource, TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            return new SelectManyEnumerable<TSource, TResult>(source, selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectManyEnumerable<TSource, TCollection, TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (collectionSelector == null)
                throw new ArgumentNullException(nameof(collectionSelector));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));
            return new SelectManyEnumerable<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
        }

        #endregion

        #region First/Last Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource First<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).First(predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource FirstOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).FirstOrDefault(predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource Last<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).Last(predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource LastOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).LastOrDefault(predicate);
        }

        #endregion

        #region Contains Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TSource>(
            this IEnumerable<TSource> source,
            TSource value,
            IEqualityComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).Contains(value, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            return new ZeroAllocEnumerable<TSource>(source).Contains(predicate);
        }

        #endregion

        #region Any/All Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).Any(predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            return new ZeroAllocEnumerable<TSource>(source).All(predicate);
        }

        #endregion

        #region FindIndex/Find Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            return new ZeroAllocEnumerable<TSource>(source).FindIndex(predicate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource Find<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            return new ZeroAllocEnumerable<TSource>(source).Find(predicate);
        }

        #endregion

        #region Set Operation Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IZeroAllocEnumerable<TSource> Intersect<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer = null)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            return new ZeroAllocEnumerable<TSource>(first).Intersect(second, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IZeroAllocEnumerable<TSource> Union<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer = null)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            return new ZeroAllocEnumerable<TSource>(first).Union(second, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IZeroAllocEnumerable<TSource> Except<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer = null)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            return new ZeroAllocEnumerable<TSource>(first).Except(second, comparer);
        }

        #endregion

        #region Reverse Extension

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IZeroAllocEnumerable<TSource> Reverse<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).Reverse();
        }

        #endregion

        #region ForEach Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TSource>(
            this IEnumerable<TSource> source,
            Action<TSource> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            new ZeroAllocEnumerable<TSource>(source).ForEach(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TSource>(
            this IEnumerable<TSource> source,
            Action<TSource, int> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            new ZeroAllocEnumerable<TSource>(source).ForEach(action);
        }

        #endregion

        #region Count Extension

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).Count();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountBy<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).CountBy(predicate);
        }

        #endregion
        #region Sum Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource Sum<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).Sum();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Sum<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> resultSelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));
            return new ZeroAllocEnumerable<TSource>(source).Sum(resultSelector);
        }

        #endregion
        #region Aggregate Extension

        public static TSource Aggregate<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return new ZeroAllocEnumerable<TSource>(source).Aggregate(func);
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            return new ZeroAllocEnumerable<TSource>(source).Aggregate(seed, func);
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));
            return new ZeroAllocEnumerable<TSource>(source).Aggregate(seed, func, resultSelector);
        }

        #endregion
        #region Min Extension

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource Min<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ZeroAllocEnumerable<TSource>(source).Min(comparer);
        }

        #endregion
        #region Max Extension

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource Max<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ZeroAllocEnumerable<TSource>(source).Max(comparer);
        }

        #endregion

        #region Array-Specific Optimizations

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<TSource>(
            this TSource[] array,
            Func<TSource, bool> predicate)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                    return i;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TSource>(
            this TSource[] array,
            TSource value,
            IEqualityComparer<TSource> comparer = null)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            comparer ??= EqualityComparer<TSource>.Default;
            for (int i = 0; i < array.Length; i++)
            {
                if (comparer.Equals(array[i], value))
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TSource>(
            this TSource[] array,
            Action<TSource> action)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TSource>(
            this TSource[] array,
            Action<TSource, int> action)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }
        }

        #endregion

        #region Conversion Extensions

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).ToList();
        }
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).ToArray();
        }

        #endregion

        #region ElementAt

        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new ZeroAllocEnumerable<TSource>(source).ElementAt(index);
        }

        #endregion

        #region GroupBy Extensions

        public static IZeroAllocEnumerable<ZeroAllocLinqInternal.IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            return new GroupByEnumerable1<TSource, TKey>(source, keySelector, comparer);
        }

        public static IZeroAllocEnumerable<ZeroAllocLinqInternal.IGrouping<TKey, TElement>> GroupBy<TSource, TElement, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return new GroupByEnumerable3<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
        }

        public static IZeroAllocEnumerable<TResult> GroupBy<TSource, TKey, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return new GroupByEnumerable5<TSource, TKey, TResult>(source, keySelector, resultSelector, comparer);
        }

        public static IZeroAllocEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return new GroupByEnumerable7<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer);
        }

        #endregion

        #region Lookup Extensions

        public static IZeroAllocEnumerable<ZeroAllocLinqInternal.IGrouping<TKey, TElement>> Lookup<TKey, TElement>(
            IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return new ZeroAllocLinqInternal.Lookup<TKey, TElement>(source, keySelector, comparer);
        }

        #endregion

        #region OrderBy Extension

        //Ascending
        public static ZeroAllocLinqInternal.IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new OrderedEnumerable<TSource>(source, comparer);
        }
        public static ZeroAllocLinqInternal.IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer);
        }

        //Descending
        public static ZeroAllocLinqInternal.IOrderedEnumerable<TSource> OrderByDescending<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new OrderedEnumerable<TSource>(source, comparer).ThenByDescendingMethod();
        }
        public static ZeroAllocLinqInternal.IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer).ThenByDescendingMethod();
        }

        #endregion

        #region ThenBy Extension

        public static ZeroAllocLinqInternal.IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this ZeroAllocLinqInternal.IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            return source.ThenByMethod(keySelector, comparer);
        }
        public static ZeroAllocLinqInternal.IOrderedEnumerable<TSource> ThenBy<TSource>(this ZeroAllocLinqInternal.IOrderedEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.ThenByMethod(comparer);
        }

        #endregion

        #region Take Extension

        public static IZeroAllocEnumerable<TSource> Take<TSource>(
            this IEnumerable<TSource> source,
            int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new TakeEnumerable<TSource>(source, count);
        }
        public static IZeroAllocEnumerable<TSource> TakeWhile<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new TakeWhileEnumerable<TSource>(source, predicate);
        }

        #endregion
        #region Skip Extension

        public static IZeroAllocEnumerable<TSource> Skip<TSource>(
            this IEnumerable<TSource> source,
            int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new SkipEnumerable<TSource>(source, count);
        }
        public static IZeroAllocEnumerable<TSource> SkipWhile<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new SkipWhileEnumerable<TSource>(source, predicate);
        }

        #endregion
    }
}