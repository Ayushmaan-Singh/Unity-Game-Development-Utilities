using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace Astek
{
    public class ObjectPool<T>
    {
        private readonly LinkedList<T> FIFO = new LinkedList<T>();
        private readonly int _maxSize;
        private readonly bool _canExpand = true;

        public int Count => FIFO.Count;
        public bool CanBePooled => FIFO.Count < _maxSize || _canExpand;
        public bool CanRelease => FIFO.Count > 0;

        public event Action<T> OnPooled = delegate { };
        public event Action<T> OnRelease = delegate { };
        public event Action<T> OnClear = delegate { };

        public ObjectPool() => _canExpand = true;
        public ObjectPool(int maxSize = 1)
        {
            _maxSize = maxSize;
            _canExpand = false;
        }

        public void Pool(T obj)
        {
            if (!CanBePooled)
                return;
            FIFO.AddFirst(new LinkedListNode<T>(obj));
            OnPooled.Invoke(obj);
        }

        public T? Release()
        {
            if (!CanRelease)
                return default;

            T obj = FIFO.Last.Value;
            FIFO.RemoveLast();
            OnRelease.Invoke(obj);
            return obj;
        }

        public T? Peek() => FIFO.Last != null ? FIFO.Last.Value : default;
        public bool Remove(T obj) => FIFO.Remove(obj);
        public void Clear()
        {
            FIFO.ForEach(obj => OnClear.Invoke(obj));
            FIFO.Clear();
        }
    }
}