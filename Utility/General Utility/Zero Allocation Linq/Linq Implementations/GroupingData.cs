using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Astek.ZeroAllocLinqInternal
{
    // IGrouping interface (same as .NET)
    public interface IGrouping<out TKey, out TElement> : IEnumerable<TElement>
    {
        TKey Key { get; }
    }

    // Grouping implementation
    public readonly struct Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; }
        private readonly IEnumerable<TElement> _elements;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => $"Key: {Key}, Count: {GetCount()}";

        private int GetCount()
        {
            if (_elements is ICollection<TElement> collection)
                return collection.Count;

            int count = 0;
            foreach (var _ in _elements) count++;
            return count;
        }
    }

    //Variation 1: GroupBy(keySelector)
    public readonly struct GroupByEnumerable1<TSource, TKey> : IZeroAllocEnumerable<IGrouping<TKey, TSource>>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GroupByEnumerable1(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            _source = source;
            _keySelector = keySelector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<IGrouping<TKey, TSource>> GetEnumerator()
            => new ZeroAllocEnumerator<IGrouping<TKey, TSource>>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<IGrouping<TKey, TSource>> IEnumerable<IGrouping<TKey, TSource>>.GetEnumerator()
            => GetEnumeratorInternal();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<IGrouping<TKey, TSource>> GetEnumeratorInternal()
        {
            Dictionary<TKey, List<TSource>> groups = new Dictionary<TKey, List<TSource>>();

            foreach (TSource item in _source)
            {
                TKey key = _keySelector(item);
                if (!groups.TryGetValue(key, out List<TSource> group))
                {
                    group = new List<TSource>();
                    groups[key] = group;
                }
                group.Add(item);
            }

            foreach (KeyValuePair<TKey, List<TSource>> kvp in groups)
            {
                yield return new Grouping<TKey, TSource>(kvp.Key, kvp.Value);
            }
        }
    }

    //Variation 2: Same as Variation 1, just uses the comparer parameter

    //Variation 3: GroupBy(keySelector, elementSelector)
    public readonly struct GroupByEnumerable3<TSource, TKey, TElement> : IZeroAllocEnumerable<IGrouping<TKey, TElement>>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GroupByEnumerable3(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            _source = source;
            _keySelector = keySelector;
            _elementSelector = elementSelector;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
            => new ZeroAllocEnumerator<IGrouping<TKey, TElement>>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<IGrouping<TKey, TElement>> IEnumerable<IGrouping<TKey, TElement>>.GetEnumerator()
            => GetEnumeratorInternal();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<IGrouping<TKey, TElement>> GetEnumeratorInternal()
        {
            Dictionary<TKey, List<TElement>> groups = new Dictionary<TKey, List<TElement>>(_comparer);

            foreach (var item in _source)
            {
                TKey key = _keySelector(item);
                TElement element = _elementSelector(item);

                if (!groups.TryGetValue(key, out List<TElement> group))
                {
                    group = new List<TElement>();
                    groups[key] = group;
                }
                group.Add(element);
            }

            foreach (KeyValuePair<TKey, List<TElement>> kvp in groups)
            {
                yield return new Grouping<TKey, TElement>(kvp.Key, kvp.Value);
            }
        }
    }

    //Variation 4: Same as Variation 3, just uses the comparer parameter

    //Variation 5: GroupBy(keySelector, resultSelector)
    public readonly struct GroupByEnumerable5<TSource, TKey, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TKey, IEnumerable<TSource>, TResult> _resultSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GroupByEnumerable5(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            _source = source;
            _keySelector = keySelector;
            _resultSelector = resultSelector;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
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
            Dictionary<TKey, List<TSource>> groups = new Dictionary<TKey, List<TSource>>(_comparer);

            foreach (TSource item in _source)
            {
                TKey key = _keySelector(item);
                if (!groups.TryGetValue(key, out List<TSource> group))
                {
                    group = new List<TSource>();
                    groups[key] = group;
                }
                group.Add(item);
            }

            foreach (KeyValuePair<TKey, List<TSource>> kvp in groups)
            {
                yield return _resultSelector(kvp.Key, kvp.Value);
            }
        }
    }

    //Variation 6: Same as Variation 5, just uses the comparer parameter

    //Variation 7: GroupBy(keySelector, elementSelector, resultSelector)
    public readonly struct GroupByEnumerable7<TSource, TKey, TElement, TResult> : IZeroAllocEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GroupByEnumerable7(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            _source = source;
            _keySelector = keySelector;
            _elementSelector = elementSelector;
            _resultSelector = resultSelector;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
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
            Dictionary<TKey, List<TElement>> groups = new Dictionary<TKey, List<TElement>>(_comparer);

            foreach (TSource item in _source)
            {
                TKey key = _keySelector(item);
                TElement element = _elementSelector(item);

                if (!groups.TryGetValue(key, out List<TElement> group))
                {
                    group = new List<TElement>();
                    groups[key] = group;
                }
                group.Add(element);
            }

            foreach (KeyValuePair<TKey, List<TElement>> kvp in groups)
            {
                yield return _resultSelector(kvp.Key, kvp.Value);
            }
        }
    }

    //Variation 8: Same as Variation 7, just uses the comparer parameter

    #region Lookup

    public readonly struct Lookup<TKey, TElement> : IZeroAllocEnumerable<IGrouping<TKey, TElement>>
    {
        private readonly Dictionary<TKey, List<TElement>> _groups;
        private readonly IEqualityComparer<TKey> _comparer;

        public int Count => _groups.Count;
        public IEnumerable<TElement> this[TKey key] =>
            _groups.TryGetValue(key, out List<TElement> group) ? group : Array.Empty<TElement>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lookup(
            IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _groups = new Dictionary<TKey, List<TElement>>(_comparer);

            foreach (TElement item in source)
            {
                TKey key = keySelector(item);
                if (!_groups.TryGetValue(key, out List<TElement> group))
                {
                    group = new List<TElement>();
                    _groups[key] = group;
                }
                group.Add(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Lookup(Dictionary<TKey, List<TElement>> groups, IEqualityComparer<TKey> comparer)
        {
            _groups = groups;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TKey key) => _groups.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
            => new ZeroAllocEnumerator<IGrouping<TKey, TElement>>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<IGrouping<TKey, TElement>> IEnumerable<IGrouping<TKey, TElement>>.GetEnumerator()
            => GetEnumeratorInternal();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

        private IEnumerator<IGrouping<TKey, TElement>> GetEnumeratorInternal()
        {
            foreach (KeyValuePair<TKey, List<TElement>> kvp in _groups)
            {
                yield return new Grouping<TKey, TElement>(kvp.Key, kvp.Value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCount(TKey key)
        {
            return _groups.TryGetValue(key, out List<TElement> group) ? group.Count : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetGroup(TKey key, out IEnumerable<TElement> group)
        {
            if (_groups.TryGetValue(key, out var list))
            {
                group = list;
                return true;
            }
            group = Array.Empty<TElement>();
            return false;
        }
    }

    #endregion
}