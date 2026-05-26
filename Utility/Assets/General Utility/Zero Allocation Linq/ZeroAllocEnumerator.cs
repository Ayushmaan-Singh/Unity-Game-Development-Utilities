using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Astek.ZeroAllocLinqInternal
{
    // Main interface for zero-allocation enumerables
    public interface IZeroAllocEnumerable<T> : IEnumerable<T>
    {
        new ZeroAllocEnumerator<T> GetEnumerator();
    }

    // Zero-allocation enumerator
    public struct ZeroAllocEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerable<T> _source;
        private IEnumerator<T> _enumerator;
        private T _current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ZeroAllocEnumerator(IEnumerable<T> source)
        {
            _source = source;
            _enumerator = null;
            _current = default;
        }

        public T Current => _current;
        object IEnumerator.Current => _current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            _enumerator ??= _source.GetEnumerator();

            if (_enumerator.MoveNext())
            {
                _current = _enumerator.Current;
                return true;
            }

            _current = default;
            return false;
        }

        public void Reset() => throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _enumerator?.Dispose();
    }
}