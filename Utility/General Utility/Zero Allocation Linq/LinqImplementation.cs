using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace AstekUtility.ZeroAllocLinqInternal
{
    // Where operation implementation
    public struct WhereEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly ValuePredicate<T> _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WhereEnumerable(IEnumerable<T> source, ValuePredicate<T> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<T> GetEnumeratorInternal()
        {
            foreach (T item in _source)
            {
                if (_predicate(item))
                    yield return item;
            }
        }
    }

    // Select operation implementation
    public struct SelectEnumerable<T, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<T> _source;
        private readonly ValueFunc<T, TResult> _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectEnumerable(IEnumerable<T> source, ValueFunc<T, TResult> selector)
        {
            _source = source;
            _selector = selector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TResult> GetEnumerator()
            => new ZeroAllocEnumerator<TResult>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        public IEnumerator<TResult> GetEnumeratorInternal()
        {
            foreach (T item in _source)
            {
                yield return _selector(item);
            }
        }
    }

    // SelectMany operation
    public readonly struct SelectManyEnumerable<T, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<T> _source;
        private readonly ValueFunc<T, IEnumerable<TResult>> _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectManyEnumerable(IEnumerable<T> source, ValueFunc<T, IEnumerable<TResult>> selector)
        {
            _source = source;
            _selector = selector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TResult> GetEnumerator()
            => new ZeroAllocEnumerator<TResult>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<TResult> GetEnumeratorInternal()
        {
            foreach (T item in _source)
            {
                IEnumerable<TResult> innerEnumerable = _selector(item);
                if (innerEnumerable != null)
                {
                    foreach (TResult innerItem in innerEnumerable)
                    {
                        yield return innerItem;
                    }
                }
            }
        }
    }

    // SelectMany with result selector
    public readonly struct SelectManyEnumerable<T, TCollection, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<T> _source;
        private readonly ValueFunc<T, IEnumerable<TCollection>> _collectionSelector;
        private readonly ValueFunc<T, TCollection, TResult> _resultSelector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectManyEnumerable(
            IEnumerable<T> source,
            ValueFunc<T, IEnumerable<TCollection>> collectionSelector,
            ValueFunc<T, TCollection, TResult> resultSelector)
        {
            _source = source;
            _collectionSelector = collectionSelector;
            _resultSelector = resultSelector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TResult> GetEnumerator()
            => new ZeroAllocEnumerator<TResult>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<TResult> GetEnumeratorInternal()
        {
            foreach (T outerItem in _source)
            {
                IEnumerable<TCollection> innerEnumerable = _collectionSelector(outerItem);
                if (innerEnumerable != null)
                {
                    foreach (TCollection innerItem in innerEnumerable)
                    {
                        yield return _resultSelector(outerItem, innerItem);
                    }
                }
            }
        }
    }

    // Intersect operation
    public readonly struct IntersectEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _first;
        private readonly IEnumerable<T> _second;
        private readonly IEqualityComparer<T> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntersectEnumerable(IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
        {
            _first = first;
            _second = second;
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<T> GetEnumeratorInternal()
        {
            HashSet<T> set = new HashSet<T>(_comparer);
            foreach (T item in _second)
                set.Add(item);

            HashSet<T> yielded = new HashSet<T>(_comparer);

            foreach (T item in _first)
            {
                if (set.Contains(item) && yielded.Add(item))
                {
                    yield return item;
                }
            }
        }
    }

    // Union operation
    public readonly struct UnionEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _first;
        private readonly IEnumerable<T> _second;
        private readonly IEqualityComparer<T> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnionEnumerable(IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
        {
            _first = first;
            _second = second;
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<T> GetEnumeratorInternal()
        {
            HashSet<T> set = new HashSet<T>(_comparer);

            foreach (T item in _first)
            {
                if (set.Add(item))
                    yield return item;
            }

            foreach (T item in _second)
            {
                if (set.Add(item))
                    yield return item;
            }
        }
    }

    // Except operation
    public readonly struct ExceptEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _first;
        private readonly IEnumerable<T> _second;
        private readonly IEqualityComparer<T> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExceptEnumerable(IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
        {
            _first = first;
            _second = second;
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<T> GetEnumeratorInternal()
        {
            HashSet<T> set = new HashSet<T>(_comparer);
            foreach (T item in _second)
                set.Add(item);

            foreach (T item in _first)
            {
                if (set.Add(item))
                {
                    yield return item;
                }
            }
        }
    }

    // Reverse operation
    public readonly struct ReverseEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReverseEnumerable(IEnumerable<T> source)
        {
            _source = source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<T> GetEnumeratorInternal()
        {
            if (_source is IList<T> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    yield return list[i];
                }
                yield break;
            }

            Stack<T> stack = new Stack<T>();
            foreach (T item in _source)
                stack.Push(item);

            while (stack.Count > 0)
            {
                yield return stack.Pop();
            }
        }
    }

    // Take operation implementation
    public struct TakeEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TakeEnumerable(IEnumerable<T> source, int count)
        {
            _source = source;
            _count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        public IEnumerator<T> GetEnumeratorInternal()
        {
            int taken = 0;
            foreach (T item in _source)
            {
                if (taken >= _count)
                    yield break;

                yield return item;
                taken++;
            }
        }
    }

    // Skip operation implementation
    public struct SkipEnumerable<T> : IZeroAllocEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SkipEnumerable(IEnumerable<T> source, int count)
        {
            _source = source;
            _count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<T> GetEnumerator()
            => new ZeroAllocEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        public IEnumerator<T> GetEnumeratorInternal()
        {
            int skipped = 0;
            foreach (T item in _source)
            {
                if (skipped < _count)
                {
                    skipped++;
                    continue;
                }

                yield return item;
            }
        }
    }
}