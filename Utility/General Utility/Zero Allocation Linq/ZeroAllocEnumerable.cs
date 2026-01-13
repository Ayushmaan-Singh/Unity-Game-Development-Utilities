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

        #region Core Filtering/Projection Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WhereEnumerable<T> Where(ValuePredicate<T> predicate)
            => new WhereEnumerable<T>(_source, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectEnumerable<T, TResult> Select<TResult>(ValueFunc<T, TResult> selector)
            => new SelectEnumerable<T, TResult>(_source, selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectManyEnumerable<T, TResult> SelectMany<TResult>(ValueFunc<T, IEnumerable<TResult>> selector)
            => new SelectManyEnumerable<T, TResult>(_source, selector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectManyEnumerable<T, TCollection, TResult> SelectMany<TCollection, TResult>(
            ValueFunc<T, IEnumerable<TCollection>> collectionSelector,
            ValueFunc<T, TCollection, TResult> resultSelector)
            => new SelectManyEnumerable<T, TCollection, TResult>(_source, collectionSelector, resultSelector);

        #endregion

        #region Set Operations

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntersectEnumerable<T> Intersect(IEnumerable<T> second, IEqualityComparer<T> comparer = null)
            => new IntersectEnumerable<T>(_source, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnionEnumerable<T> Union(IEnumerable<T> second, IEqualityComparer<T> comparer = null)
            => new UnionEnumerable<T>(_source, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExceptEnumerable<T> Except(IEnumerable<T> second, IEqualityComparer<T> comparer = null)
            => new ExceptEnumerable<T>(_source, second, comparer);

        #endregion

        #region Reverse Method

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReverseEnumerable<T> Reverse()
            => new ReverseEnumerable<T>(_source);

        #endregion

        #region ForEach Method

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<T> action)
        {
            foreach (T item in _source)
                action(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<T, int> action)
        {
            int index = 0;
            foreach (T item in _source)
            {
                action(item, index);
                index++;
            }
        }

        #endregion

        #region First/Last Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T First(ValuePredicate<T> predicate = null)
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
        public T FirstOrDefault(ValuePredicate<T> predicate = null)
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
        public T Last(ValuePredicate<T> predicate = null)
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
        public T LastOrDefault(ValuePredicate<T> predicate = null)
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
        public bool Contains(ValuePredicate<T> predicate)
        {
            foreach (T item in _source)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any(ValuePredicate<T> predicate = null)
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
        public bool All(ValuePredicate<T> predicate)
        {
            foreach (T item in _source)
            {
                if (!predicate(item))
                    return false;
            }
            return true;
        }

        #endregion

        #region Count Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count(ValuePredicate<T> predicate = null)
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

        #region FindIndex Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(ValuePredicate<T> predicate)
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
        public T Find(ValuePredicate<T> predicate)
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

        #region Sum Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Sum(ValueFunc<T, int> selector = null)
        {
            int sum = 0;
            if (selector == null)
            {
                if (typeof(T) != typeof(int))
                    throw new InvalidOperationException("Cannot calculate the sum of a non-integer number");

                foreach (T item in _source)
                    sum += (int)(object)item;
                return sum;
            }
            foreach (T item in _source)
                sum += selector(item);
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Sum(ValueFunc<T, float> selector = null)
        {
            float sum = 0;
            if (selector == null)
            {
                if (typeof(T) != typeof(float))
                    throw new InvalidOperationException("Cannot calculate the sum of a non-float number");

                foreach (T item in _source)
                    sum += (float)(object)item;
                return sum;
            }
            foreach (T item in _source)
                sum += selector(item);
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Sum(ValueFunc<T, double> selector = null)
        {
            double sum = 0;
            if (selector == null)
            {
                if (typeof(T) != typeof(double))
                    throw new InvalidOperationException("Cannot calculate the sum of a non-double number");

                foreach (T item in _source)
                    sum += (float)(object)item;
                return sum;
            }
            foreach (T item in _source)
                sum += selector(item);
            return sum;
        }

        #endregion

        #region Conversion Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> ToList()
        {
            if (_source is ICollection<T> collection)
                return new List<T>(collection);

            var list = new List<T>();
            foreach (var item in _source)
                list.Add(item);
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            if (_source is ICollection<T> collection)
            {
                var array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }

            return ToList().ToArray();
        }

        #endregion
    }
}