using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Astek.ZeroAllocLinqInternal
{
    #region Order By And Then By

    public interface IOrderedEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        public int PrimaryComparer(TSource x, TSource y);
        public IOrderedEnumerable<TSource> ThenByMethod<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null);
        public IOrderedEnumerable<TSource> ThenByMethod(IComparer<TSource> comparer = null);
        public IOrderedEnumerable<TSource> ThenByDescendingMethod<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null);
        public IOrderedEnumerable<TSource> ThenByDescendingMethod(IComparer<TSource> comparer = null);
    }

    public readonly struct OrderedEnumerable<TSource, TKey> : IOrderedEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IComparer<TKey> _comparer;

        public OrderedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            _source = source;
            _keySelector = keySelector;
            _comparer = comparer ?? Comparer<TKey>.Default;
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
            IEnumerable<TSource> result;
            if (_source.Count() <= 200)
                result = _source.QuickSort(PrimaryComparer); // Use quick sort for small collections
            else
                result = _source.MergeSort(PrimaryComparer); // Use merge sort for larger collections

            foreach (TSource item in result)
                yield return item;
        }

        // ThenBy with KeySelector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByMethod<TKey2>(Func<TSource, TKey2> keySelector, IComparer<TKey2> comparer = null)
        {
            ThenByComparer<TSource, TKey2> newComparer = new ThenByComparer<TSource, TKey2>(this, keySelector, comparer: comparer);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }
        // ThenBy without Key Selector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByMethod(IComparer<TSource> comparer = null)
        {
            ThenByComparer<TSource> newComparer = new ThenByComparer<TSource>(this, comparer: comparer);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }
        // ThenByDescending with KeySelector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByDescendingMethod<TKey2>(Func<TSource, TKey2> keySelector, IComparer<TKey2> comparer = null)
        {
            ThenByComparer<TSource, TKey2> newComparer = new ThenByComparer<TSource, TKey2>(this, keySelector, comparer, true);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }
        // ThenByDescending without Key Selector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByDescendingMethod(IComparer<TSource> comparer = null)
        {
            ThenByComparer<TSource> newComparer = new ThenByComparer<TSource>(this, comparer, true);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }

        public int PrimaryComparer(TSource x, TSource y) => _comparer.Compare(_keySelector(x), _keySelector(y));
    }
    public readonly struct OrderedEnumerable<TSource> : IOrderedEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly IComparer<TSource> _comparer;

        public OrderedEnumerable(IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            _source = source;
            _comparer = comparer ?? Comparer<TSource>.Default;
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
            IEnumerable<TSource> result;
            if (_source.Count() <= 200)
                result = _source.QuickSort(PrimaryComparer); // Use quick sort for small collections
            else
                result = _source.MergeSort(PrimaryComparer); // Use merge sort for larger collections

            foreach (TSource item in result)
                yield return item;
        }

        // ThenBy with KeySelector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByMethod<TKey2>(Func<TSource, TKey2> keySelector, IComparer<TKey2> comparer = null)
        {
            ThenByComparer<TSource, TKey2> newComparer = new ThenByComparer<TSource, TKey2>(this, keySelector, comparer: comparer);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }
        // ThenBy without Key Selector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByMethod(IComparer<TSource> comparer = null)
        {
            ThenByComparer<TSource> newComparer = new ThenByComparer<TSource>(this, comparer: comparer);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }
        // ThenByDescending with KeySelector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByDescendingMethod<TKey2>(Func<TSource, TKey2> keySelector, IComparer<TKey2> comparer = null)
        {
            ThenByComparer<TSource, TKey2> newComparer = new ThenByComparer<TSource, TKey2>(this, keySelector, comparer, true);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }
        // ThenByDescending without Key Selector for chaining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IOrderedEnumerable<TSource> ThenByDescendingMethod(IComparer<TSource> comparer = null)
        {
            ThenByComparer<TSource> newComparer = new ThenByComparer<TSource>(this, comparer, true);
            return new OrderedEnumerable<TSource>(_source, newComparer);
        }

        public int PrimaryComparer(TSource x, TSource y) => _comparer.Compare(x, y);
    }

    internal readonly struct ThenByComparer<TSource, TKey> : IComparer<TSource>
    {
        private readonly IOrderedEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IComparer<TKey> _comparer;
        private readonly bool _descending;

        public ThenByComparer(IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null, bool descending = false)
        {
            _source = source;
            _keySelector = keySelector;
            _comparer = comparer ?? Comparer<TKey>.Default;
            _descending = descending;
        }

        public int Compare(TSource x, TSource y)
        {
            // Then use the new key selector
            int keyResult = _comparer.Compare(_keySelector(x), _keySelector(y));
            return _descending ? -keyResult : keyResult;
        }
    }
    internal readonly struct ThenByComparer<TSource> : IComparer<TSource>
    {
        private readonly IOrderedEnumerable<TSource> _source;
        private readonly IComparer<TSource> _comparer;
        private readonly bool _descending;

        public ThenByComparer(IOrderedEnumerable<TSource> source, IComparer<TSource> comparer = null, bool descending = false)
        {
            _source = source;
            _comparer = comparer ?? Comparer<TSource>.Default;
            _descending = descending;
        }

        public int Compare(TSource x, TSource y)
        {
            // Then use the new key selector
            int keyResult = _comparer.Compare(x, y);
            return _descending ? -keyResult : keyResult;
        }
    }

    #endregion

    #region Reverse

    // Reverse operation
    public readonly struct ReverseEnumerable<TSource> : IZeroAllocEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReverseEnumerable(IEnumerable<TSource> source)
        {
            _source = source;
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
            if (_source is IList<TSource> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    yield return list[i];
                }
                yield break;
            }

            Stack<TSource> stack = new Stack<TSource>();
            foreach (TSource item in _source)
                stack.Push(item);

            while (stack.Count > 0)
            {
                yield return stack.Pop();
            }
        }
    }

    #endregion
}