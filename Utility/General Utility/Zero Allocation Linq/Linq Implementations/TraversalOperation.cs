using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace AstekUtility.ZeroAllocLinqInternal
{
    // Take operation implementation
    public readonly struct TakeEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TakeEnumerable(IEnumerable<TSource> source, int count)
        {
            _source = source;
            _count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TSource> GetEnumerator()
            => new ZeroAllocEnumerator<TSource>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        public IEnumerator<TSource> GetEnumeratorInternal()
        {
            int taken = 0;
            foreach (TSource item in _source)
            {
                if (taken >= _count)
                    yield break;

                yield return item;
                taken++;
            }
        }
    }

    // Skip operation implementation
    public readonly struct SkipEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SkipEnumerable(IEnumerable<TSource> source, int count)
        {
            _source = source;
            _count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<TSource> GetEnumerator()
            => new ZeroAllocEnumerator<TSource>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumeratorInternal();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        public IEnumerator<TSource> GetEnumeratorInternal()
        {
            int skipped = 0;
            foreach (TSource item in _source)
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