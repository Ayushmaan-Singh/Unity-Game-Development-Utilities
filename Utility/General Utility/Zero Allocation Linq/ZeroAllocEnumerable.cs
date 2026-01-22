using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AstekUtility.ZeroAllocLinqInternal
{
    public readonly struct ZeroAllocEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerable(IEnumerable<T> source)
        {
            _source = source ?? Array.Empty<T>();
        }

        #region IEnumerable Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(_source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _source.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => _source.GetEnumerator();

        #endregion

        #region First/Last Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T First(Func<T, bool> predicate = null)
        {
            if (predicate == null)
            {
                using IEnumerator<T> enumerator = _source.GetEnumerator();
                if (enumerator.MoveNext())
                    return enumerator.Current;
                throw new InvalidOperationException("Sequence contains no elements");
            }

            foreach (T item in _source)
            {
                if (predicate(item))
                    return item;
            }
            throw new InvalidOperationException("Sequence contains no matching element");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T FirstOrDefault(Func<T, bool> predicate = null)
        {
            if (predicate == null)
            {
                using IEnumerator<T> enumerator = _source.GetEnumerator();
                return enumerator.MoveNext() ? enumerator.Current : default;
            }

            foreach (T item in _source)
            {
                if (predicate(item))
                    return item;
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Last(Func<T, bool> predicate = null)
        {
            T result = default;
            bool found = false;

            if (predicate == null)
            {
                foreach (T item in _source)
                {
                    result = item;
                    found = true;
                }
            }
            else
            {
                foreach (T item in _source)
                {
                    if (predicate(item))
                    {
                        result = item;
                        found = true;
                    }
                }
            }

            if (!found)
                throw new InvalidOperationException("Sequence contains no elements");

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T LastOrDefault(Func<T, bool> predicate = null)
        {
            T result = default;

            if (predicate == null)
            {
                foreach (T item in _source)
                {
                    result = item;
                }
            }
            else
            {
                foreach (T item in _source)
                {
                    if (predicate(item))
                    {
                        result = item;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Contains/Any/All Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T value, IEqualityComparer<T> comparer = null)
        {
            comparer ??= EqualityComparer<T>.Default;

            foreach (T item in _source)
            {
                if (comparer.Equals(item, value))
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Func<T, bool> predicate)
        {
            foreach (T item in _source)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any(Func<T, bool> predicate = null)
        {
            if (predicate == null)
            {
                using IEnumerator<T> enumerator = _source.GetEnumerator();
                return enumerator.MoveNext();
            }

            foreach (T item in _source)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool All(Func<T, bool> predicate)
        {
            foreach (T item in _source)
            {
                if (!predicate(item))
                    return false;
            }
            return true;
        }

        #endregion

        #region FindIndex Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in _source)
            {
                if (predicate(item))
                    return index;
                index++;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Find(Func<T, bool> predicate)
        {
            foreach (T item in _source)
            {
                if (predicate(item))
                    return item;
            }
            return default;
        }

        #endregion

        #region ElementAt

        public T ElementAt(int index)
        {
            if (_source == null) throw new ArgumentNullException(nameof(_source));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            // If _source is IList<T>, we can access directly
            if (_source is IList<T> list)
            {
                return list[index]; // will throw if out of range
            }

            // Otherwise, enumerate
            using IEnumerator<T> enumerator = _source.GetEnumerator();
            int count = 0;
            while (enumerator.MoveNext())
            {
                if (count == index)
                    return enumerator.Current;

                count++;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        #endregion

        #region Count Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            if (_source is ICollection<T> collection)
                return collection.Count;
            if (_source is ICollection nonGenericCollection)
                return nonGenericCollection.Count;

            int count = 0;
            foreach (T _ in _source) count++;
            return count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountBy(Func<T, bool> predicate = null)
        {
            if (predicate == null)
            {
                if (_source is ICollection<T> collection)
                    return collection.Count;
                if (_source is ICollection nonGenericCollection)
                    return nonGenericCollection.Count;

                int count = 0;
                foreach (T _ in _source) count++;
                return count;
            }

            int countWithPredicate = 0;
            foreach (T item in _source)
            {
                if (predicate(item))
                    countWithPredicate++;
            }
            return countWithPredicate;
        }

        #endregion
        #region Sum Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Sum()
        {
            if (typeof(T) == typeof(Int32))
            {
                Int32 sum = 0;
                foreach (T item in _source)
                    sum += (Int32)(object)item;
                return (T)(object)sum;
            }
            if (typeof(T) == typeof(Nullable<Int32>))
            {
                Int32 sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Int32>)(object)item ?? 0;
                return (T)(object)sum;
            }

            if (typeof(T) == typeof(Int64))
            {
                Int64 sum = 0;
                foreach (T item in _source)
                    sum += (Int64)(object)item;
                return (T)(object)sum;
            }
            if (typeof(T) == typeof(Nullable<Int64>))
            {
                Int64 sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Int64>)(object)item ?? 0;
                return (T)(object)sum;
            }

            if (typeof(T) == typeof(Single))
            {
                Single sum = 0;
                foreach (T item in _source)
                    sum += (Single)(object)item;
                return (T)(object)sum;
            }
            if (typeof(T) == typeof(Nullable<Single>))
            {
                Single sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Single>)(object)item ?? 0;
                return (T)(object)sum;
            }

            if (typeof(T) == typeof(Double))
            {
                Double sum = 0;
                foreach (T item in _source)
                    sum += (Double)(object)item;
                return (T)(object)sum;
            }
            if (typeof(T) == typeof(Nullable<Double>))
            {
                Double sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Double>)(object)item ?? 0;
                return (T)(object)sum;
            }

            if (typeof(T) == typeof(Decimal))
            {
                Decimal sum = 0;
                foreach (T item in _source)
                    sum += (Decimal)(object)item;
                return (T)(object)sum;
            }
            if (typeof(T) == typeof(Nullable<Decimal>))
            {
                Decimal sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Decimal>)(object)item ?? 0;
                return (T)(object)sum;
            }

            throw new Exception("Cannot calculate the sum of a non-number collection");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Sum<TResult>(Func<T, TResult> keySelector)
        {
            if (typeof(TResult) == typeof(Int32))
            {
                Int32 sum = 0;
                foreach (T item in _source)
                    sum += (Int32)(object)keySelector(item);
                return (TResult)(object)sum;
            }
            if (typeof(TResult) == typeof(Nullable<Int32>))
            {
                Int32 sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Int32>)(object)keySelector(item) ?? 0;
                return (TResult)(object)sum;
            }

            if (typeof(TResult) == typeof(Int64))
            {
                Int64 sum = 0;
                foreach (T item in _source)
                    sum += (Int64)(object)keySelector(item);
                return (TResult)(object)sum;
            }
            if (typeof(TResult) == typeof(Nullable<Int64>))
            {
                Int64 sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Int64>)(object)keySelector(item) ?? 0;
                return (TResult)(object)sum;
            }

            if (typeof(TResult) == typeof(Single))
            {
                Single sum = 0;
                foreach (T item in _source)
                    sum += (Single)(object)keySelector(item);
                return (TResult)(object)sum;
            }
            if (typeof(TResult) == typeof(Nullable<Single>))
            {
                Single sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Single>)(object)keySelector(item) ?? 0;
                return (TResult)(object)sum;
            }

            if (typeof(TResult) == typeof(Double))
            {
                Double sum = 0;
                foreach (T item in _source)
                    sum += (Double)(object)keySelector(item);
                return (TResult)(object)sum;
            }
            if (typeof(TResult) == typeof(Nullable<Double>))
            {
                Double sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Double>)(object)keySelector(item) ?? 0;
                return (TResult)(object)sum;
            }

            if (typeof(TResult) == typeof(Decimal))
            {
                Decimal sum = 0;
                foreach (T item in _source)
                    sum += (Decimal)(object)keySelector(item);
                return (TResult)(object)sum;
            }
            if (typeof(TResult) == typeof(Nullable<Decimal>))
            {
                Decimal sum = 0;
                foreach (T item in _source)
                    sum += (Nullable<Decimal>)(object)keySelector(item) ?? 0;
                return (TResult)(object)sum;
            }

            throw new Exception("Cannot calculate the sum of a non-number Key");
        }

        #endregion
        #region Min Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Min(Comparison<T> comparer = null)
        {
            if (typeof(T) == typeof(int))
            {
                int min = (int)(object)_source.First();
                foreach (T item in _source)
                {
                    int val = (int)(object)item;
                    if (min > val)
                        min = val;
                }
                return (T)(object)min;
            }
            if (typeof(T) == typeof(long))
            {
                long min = (long)(object)_source.First();
                foreach (T item in _source)
                {
                    long val = (long)(object)item;
                    if (min > val)
                        min = val;
                }
                return (T)(object)min;
            }
            if (typeof(T) == typeof(float))
            {
                float min = (float)(object)_source.First();
                foreach (T item in _source)
                {
                    float val = (float)(object)item;
                    if (min > val)
                        min = val;
                }
                return (T)(object)min;
            }
            if (typeof(T) == typeof(double))
            {
                double min = (double)(object)_source.First();
                foreach (T item in _source)
                {
                    double val = (double)(object)item;
                    if (min > val)
                        min = val;
                }
                return (T)(object)min;
            }
            if (comparer != null)
            {
                T min = _source.First();
                foreach (T item in _source)
                {
                    if (comparer(min, item) >= 0)
                        min = item;
                }
                return min;
            }

            throw new Exception("Cannot calculate the min of a non-number collection without a comparer");
        }

        #endregion
        #region Max Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Max(Comparison<T> comparer = null)
        {
            if (typeof(T) == typeof(int))
            {
                int max = (int)(object)_source.First();
                foreach (T item in _source)
                {
                    int val = (int)(object)item;
                    if (max < val)
                        max = val;
                }
                return (T)(object)max;
            }
            if (typeof(T) == typeof(long))
            {
                long max = (long)(object)_source.First();
                foreach (T item in _source)
                {
                    long val = (long)(object)item;
                    if (max < val)
                        max = val;
                }
                return (T)(object)max;
            }
            if (typeof(T) == typeof(float))
            {
                float max = (float)(object)_source.First();
                ;
                foreach (T item in _source)
                {
                    float val = (float)(object)item;
                    if (max < val)
                        max = val;
                }
                return (T)(object)max;
            }
            if (typeof(T) == typeof(double))
            {
                double max = (double)(object)_source.First();
                ;
                foreach (T item in _source)
                {
                    double val = (double)(object)item;
                    if (max < val)
                        max = val;
                }
                return (T)(object)max;
            }
            if (comparer != null)
            {
                T max = _source.First();
                ;
                foreach (T item in _source)
                {
                    if (comparer(max, item) < 0)
                        max = item;
                }
                return max;
            }

            throw new Exception("Cannot calculate the max of a non-number collection without a comparer");
        }

        #endregion

        #region Conversion Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> ToList()
        {
            if (_source == null)
                return new List<T>();

            if (_source is ICollection<T> collection)
                return new List<T>(collection);

            List<T> list = new List<T>();
            foreach (T item in _source)
                list.Add(item);
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            if (_source is ICollection<T> collection)
            {
                T[] array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }

            return ToList().ToArray();
        }

        #endregion
    }
}