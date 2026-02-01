using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace Astek
{
	[Serializable]
	public class ObjectPool<T>
	{
		private readonly Queue<T> _pool = new Queue<T>();
		[SerializeField, Min(1)] private int maxSize;
		[SerializeField] private bool canExpand = true;

		public bool CanBePooled => _pool.Count < maxSize || canExpand;
		public int Count => _pool.Count;
		public bool CanRelease => _pool.Count > 0;
		public T Peek => _pool.Peek();

		public event Action<T> OnPooled = delegate { };
		public event Action<T> OnRelease = delegate { };
		public event Action<T> OnClear = delegate { };

		public ObjectPool() { }
		public ObjectPool(int maxSize) => this.maxSize = maxSize;
		public ObjectPool(bool canExpand) => this.canExpand = canExpand;

		public void PoolObject(T obj)
		{
			if (!CanBePooled)
				return;

			_pool.Enqueue(obj);
			OnPooled.Invoke(obj);
		}
		public T? ReleaseObject()
		{
			if (!CanRelease)
				return default(T);

			T obj = _pool.Dequeue();
			OnRelease.Invoke(obj);
			return obj;
		}
		
		public void Clear()
		{
			_pool.ForEach(obj => OnClear.Invoke(obj));
			_pool.Clear();
		}
	}
}