using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.RuntimeSetArchitecture
{
	public abstract class RuntimeSet<T> : ScriptableObject
	{
		public Dictionary<string,T> Items { get; private set; } = new Dictionary<string, T>();

		public void Add(string itemName, T item)
		{
			Items.TryAdd(itemName, item);
		}

		public void Remove(string itemName)
		{
			if (Items.ContainsKey(itemName))
				Items.Remove(itemName);
		}
	}
}