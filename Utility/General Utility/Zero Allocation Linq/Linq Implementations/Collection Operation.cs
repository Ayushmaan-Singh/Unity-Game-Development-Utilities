using System;
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
    // TakeWhile operation implementation
    public readonly struct TakeWhileEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, bool> _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TakeWhileEnumerable(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
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
            foreach (TSource item in _source)
            {
                if (!_predicate(item))
                    yield break;

                yield return item;
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
    // SkipWhile operation implementation
    public readonly struct SkipWhileEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, bool> _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SkipWhileEnumerable(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
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
            foreach (TSource item in _source)
            {
                if (_predicate(item))
                    continue;

                yield return item;
            }
        }
    }
}