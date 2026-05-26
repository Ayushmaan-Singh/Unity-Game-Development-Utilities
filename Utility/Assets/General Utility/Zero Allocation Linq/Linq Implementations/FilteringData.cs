using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Astek.ZeroAllocLinqInternal
{
    // Where operation implementation
    public readonly struct WhereEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource,bool> _predicate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WhereEnumerable(IEnumerable<TSource> source, Func<TSource,bool> predicate)
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
                    yield return item;
            }
        }
    }

    //OfType operation implementation
    public readonly struct OfTypeEnumerable<TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable _source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OfTypeEnumerable(IEnumerable source)
        {
            _source = source;
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
            foreach (object item in _source)
            {
                if(item is TResult result)
                    yield return result;
            }
        }
    }
}