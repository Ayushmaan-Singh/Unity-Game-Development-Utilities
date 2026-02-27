using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace Astek
{
    [Serializable]
    public class ObjectPool<T>
    {
        private readonly LinkedList<T> FIFO = new LinkedList<T>();
        [SerializeField, Min(1)] private int maxSize;
        [SerializeField] private bool canExpand = true;

        public int Count=> FIFO.Count;
        public bool CanBePooled => FIFO.Count < maxSize || canExpand;
        public bool CanRelease => FIFO.Count > 0;

        public event Action<T> OnPooled = delegate { };
        public event Action<T> OnRelease = delegate { };
        public event Action<T> OnClear = delegate { };

        public ObjectPool() { }
        public ObjectPool(int maxSize) => this.maxSize = maxSize;
        public ObjectPool(bool canExpand) => this.canExpand = canExpand;

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