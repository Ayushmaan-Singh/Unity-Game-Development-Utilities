using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Astek.ZeroAllocLinqInternal
{
    // Intersect operation
    public readonly struct IntersectEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _first;
        private readonly IEnumerable<TSource> _second;
        private readonly IEqualityComparer<TSource> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntersectEnumerable(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            _first = first;
            _second = second;
            _comparer = comparer ?? EqualityComparer<TSource>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TSource> GetEnumerator()
            => new ZeroAllocEnumerator<TSource>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<TSource> GetEnumeratorInternal()
        {
            HashSet<TSource> set = new HashSet<TSource>(_comparer);
            foreach (TSource item in _second)
                set.Add(item);

            HashSet<TSource> yielded = new HashSet<TSource>(_comparer);

            foreach (TSource item in _first)
            {
                if (set.Contains(item) && yielded.Add(item))
                {
                    yield return item;
                }
            }
        }
    }

    // Union operation
    public readonly struct UnionEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _first;
        private readonly IEnumerable<TSource> _second;
        private readonly IEqualityComparer<TSource> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnionEnumerable(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            _first = first;
            _second = second;
            _comparer = comparer ?? EqualityComparer<TSource>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TSource> GetEnumerator()
            => new ZeroAllocEnumerator<TSource>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<TSource> GetEnumeratorInternal()
        {
            HashSet<TSource> set = new HashSet<TSource>(_comparer);

            foreach (TSource item in _first)
            {
                if (set.Add(item))
                    yield return item;
            }

            foreach (TSource item in _second)
            {
                if (set.Add(item))
                    yield return item;
            }
        }
    }

    // Except operation
    public readonly struct ExceptEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _first;
        private readonly IEnumerable<TSource> _second;
        private readonly IEqualityComparer<TSource> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExceptEnumerable(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            _first = first;
            _second = second;
            _comparer = comparer ?? EqualityComparer<TSource>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TSource> GetEnumerator()
            => new ZeroAllocEnumerator<TSource>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<TSource> GetEnumeratorInternal()
        {
            HashSet<TSource> set = new HashSet<TSource>(_comparer);
            foreach (TSource item in _second)
                set.Add(item);

            foreach (TSource item in _first)
            {
                if (set.Add(item))
                {
                    yield return item;
                }
            }
        }
    }
}