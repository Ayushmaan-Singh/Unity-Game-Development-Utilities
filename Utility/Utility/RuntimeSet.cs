using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.RuntimeSetArchitecture
{
	/// <summary>
	/// Can be used to create SO of object that is used by several object to be accessed by any object possesing this scriptable object
	/// Example. Enemies in scene
	/// </summary>
	public abstract class RuntimeSet<T> : ScriptableObject
	{
		[HideInInspector] public RuntimeDict<T> Items { get; private set; } = new RuntimeDict<T>();

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

	public class RuntimeDict<T> : UnitySerializedDictionary<string, T> { }
}