#nullable enable
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility
{
	public interface IObjectPool { }

	public class ObjectPool<T> : IObjectPool where T : class?
	{
		private readonly List<T?> _objectQueue;

		public ObjectPool(uint maxPoolSize)
		{
			_objectQueue = new List<T?>((int)maxPoolSize);
		}

		public ObjectPool()
		{
			_objectQueue = new List<T?>();
		}

		public bool CanAddItemToPool(T? obj)
		{
			return _objectQueue.Count < _objectQueue.Capacity && !_objectQueue.Contains(obj);
		}

		public void AddItemToPool(T? obj)
		{
			if (CanAddItemToPool(obj))
			{
				_objectQueue.Add(obj);
			}
			else
			{
				Debug.Log("Pooling Utility : Amount Exceeded Cannot Add More Item");
			}
		}

		public int GetPooledItemCount()
		{
			return _objectQueue.Count;
		}

		public T? GetItemFromPool()
		{
			if (_objectQueue.Count > 0)
			{
				T? val = _objectQueue[0];
				_objectQueue.RemoveAt(0);
				return val;
			}
			return null;
		}

		public bool TryGetItemFromPool(out T? val)
		{
			if (_objectQueue.Count > 0)
			{
				val = _objectQueue[0];
				_objectQueue.Remove(val);
				return true;
			}
			val = null;
			return false;
		}
	}
}