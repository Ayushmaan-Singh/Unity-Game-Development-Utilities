using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Astek.ZeroAllocLinqInternal
{
    // Select operation implementation
    public readonly struct SelectEnumerable<TSource, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TResult> _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectEnumerable(IEnumerable<TSource> source, Func<TSource, TResult> selector)
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
            foreach (TSource item in _source)
            {
                yield return _selector(item);
            }
        }
    }

    // SelectMany operation
    public readonly struct SelectManyEnumerable<TSource, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, IEnumerable<TResult>> _selector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectManyEnumerable(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
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
            foreach (TSource item in _source)
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
    public readonly struct SelectManyEnumerable<TSource, TCollection, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, IEnumerable<TCollection>> _collectionSelector;
        private readonly Func<TSource, TCollection, TResult> _resultSelector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectManyEnumerable(
            IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
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
            foreach (TSource outerItem in _source)
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
}