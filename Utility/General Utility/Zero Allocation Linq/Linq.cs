using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AstekUtility.ZeroAllocLinqInternal;

namespace AstekUtility
{
    public static class ZeroAllocLinq
    {
        // Main entry point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZeroAllocEnumerable<T> AsZeroAlloc<T>(this IEnumerable<T> source)
            => new ZeroAllocEnumerable<T>(source);

        // Array optimizations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZeroAllocEnumerable<T> AsZeroAlloc<T>(this T[] source)
            => new ZeroAllocEnumerable<T>(source);

        // List optimizations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZeroAllocEnumerable<T> AsZeroAlloc<T>(this List<T> source)
            => new ZeroAllocEnumerable<T>(source);

        #region Filtering/Projection Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static WhereEnumerable<T> Where<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate)
            => new WhereEnumerable<T>(source, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectEnumerable<T, TResult> Select<T, TResult>(
            this IEnumerable<T> source,
            ValueFunc<T, TResult> selector)
            => new SelectEnumerable<T, TResult>(source, selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectManyEnumerable<T, TResult> SelectMany<T, TResult>(
            this IEnumerable<T> source,
            ValueFunc<T, IEnumerable<TResult>> selector)
            => new SelectManyEnumerable<T, TResult>(source, selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectManyEnumerable<T, TCollection, TResult> SelectMany<T, TCollection, TResult>(
            this IEnumerable<T> source,
            ValueFunc<T, IEnumerable<TCollection>> collectionSelector,
            ValueFunc<T, TCollection, TResult> resultSelector)
            => new SelectManyEnumerable<T, TCollection, TResult>(source, collectionSelector, resultSelector);

        #endregion

        #region First/Last Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T First<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate = null)
            => new ZeroAllocEnumerable<T>(source).First(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FirstOrDefault<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate = null)
            => new ZeroAllocEnumerable<T>(source).FirstOrDefault(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate = null)
            => new ZeroAllocEnumerable<T>(source).Last(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LastOrDefault<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate = null)
            => new ZeroAllocEnumerable<T>(source).LastOrDefault(predicate);

        #endregion

        #region Contains Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(
            this IEnumerable<T> source,
            T value,
            IEqualityComparer<T> comparer = null)
            => new ZeroAllocEnumerable<T>(source).Contains(value, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate)
            => new ZeroAllocEnumerable<T>(source).Contains(predicate);

        #endregion

        #region Any/All Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate = null)
            => new ZeroAllocEnumerable<T>(source).Any(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate)
            => new ZeroAllocEnumerable<T>(source).All(predicate);

        #endregion

        #region Count Extension

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate = null)
            => new ZeroAllocEnumerable<T>(source).Count(predicate);

        #endregion

        #region FindIndex/Find Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate)
            => new ZeroAllocEnumerable<T>(source).FindIndex(predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Find<T>(
            this IEnumerable<T> source,
            ValuePredicate<T> predicate)
            => new ZeroAllocEnumerable<T>(source).Find(predicate);

        #endregion

        #region Set Operation Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntersectEnumerable<T> Intersect<T>(
            this IEnumerable<T> first,
            IEnumerable<T> second,
            IEqualityComparer<T> comparer = null)
            => new IntersectEnumerable<T>(first, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UnionEnumerable<T> Union<T>(
            this IEnumerable<T> first,
            IEnumerable<T> second,
            IEqualityComparer<T> comparer = null)
            => new UnionEnumerable<T>(first, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExceptEnumerable<T> Except<T>(
            this IEnumerable<T> first,
            IEnumerable<T> second,
            IEqualityComparer<T> comparer = null)
            => new ExceptEnumerable<T>(first, second, comparer);

        #endregion

        #region Reverse Extension

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReverseEnumerable<T> Reverse<T>(
            this IEnumerable<T> source)
            => new ReverseEnumerable<T>(source);

        #endregion

        #region ForEach Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T> action)
            => new ZeroAllocEnumerable<T>(source).ForEach(action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T, int> action)
            => new ZeroAllocEnumerable<T>(source).ForEach(action);

        #endregion

        #region Sum Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sum<T>(
            this IEnumerable<T> source,
            ValueFunc<T, int> selector = null)
            => new ZeroAllocEnumerable<T>(source).Sum(selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum<T>(
            this IEnumerable<T> source,
            ValueFunc<T, float> selector = null)
            => new ZeroAllocEnumerable<T>(source).Sum(selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sum<T>(
            this IEnumerable<T> source,
            ValueFunc<T, double> selector = null)
            => new ZeroAllocEnumerable<T>(source).Sum(selector);

        #endregion

        #region Array-Specific Optimizations

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindIndex<T>(
            this T[] array,
            ValuePredicate<T> predicate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                    return i;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(
            this T[] array,
            T value,
            IEqualityComparer<T> comparer = null)
        {
            comparer ??= EqualityComparer<T>.Default;
            for (int i = 0; i < array.Length; i++)
            {
                if (comparer.Equals(array[i], value))
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(
            this T[] array,
            Action<T> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(
            this T[] array,
            Action<T, int> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }
        }

        #endregion

        #region Conversion Extensions

        public static List<T> ToList<T>(this IEnumerable<T> source) => new ZeroAllocEnumerable<T>().ToList();
        public static T[] ToArray<T>(this IEnumerable<T> source) => new ZeroAllocEnumerable<T>().ToArray();

        #endregion

        #region ElementAt

        public static T ElementAt<T>(this IEnumerable<T> source, int index) => new ZeroAllocEnumerable<T>().ElementAt(index);

        #endregion
    }
}