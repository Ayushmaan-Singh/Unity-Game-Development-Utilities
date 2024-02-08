using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility
{
    /// <summary>
    /// This is a variable size Object pool
    /// If we pass the max size of object pool at initializtion then the pools net size is limited
    /// If we don't pass any size to object pool at initialization then the pool is infinite sized
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T>
    {
        private Queue<T> _objectQueue;
        private uint? _maxPoolSize;

        /// <summary>
        /// pools net size is limited
        /// </summary>
        /// <param name="maxPoolSize"></param>
        public ObjectPool(uint maxPoolSize)
        {
            _objectQueue = new Queue<T>();
            this._maxPoolSize = maxPoolSize;
        }

        /// <summary>
        /// pool is infinite sized
        /// </summary>
        public ObjectPool()
        {
            _objectQueue = new Queue<T>();
        }

        /// <summary>
        /// Check if items can be added to pool
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CanAddItemToPool(T obj)
        {
            return (_maxPoolSize == null || _objectQueue.Count < _maxPoolSize) && !_objectQueue.Contains(obj);
        }

        /// <summary>
        /// Store items in pool
        /// </summary>
        /// <param name="obj"></param>
        public void AddItemToPool(T obj)
        {
            if (CanAddItemToPool(obj))
            {
                _objectQueue.Enqueue(obj);
            }
            else
            {
                Debug.Log("Pooling Utility : Amount Exceeded Cannot Add More Item");
            }
        }

        /// <summary>
        /// Get count of item in pool
        /// </summary>
        /// <returns></returns>
        public int GetPooledItemCount()
        {
            return _objectQueue.Count;
        }

        /// <summary>
        /// Get item from pool in the order it was saved in
        /// </summary>
        /// <returns></returns>
        public T GetItemFromPool()
        {
            if (_objectQueue.Count > 0)
                return _objectQueue.Dequeue();
            else
                return default;
        }
    }
}
