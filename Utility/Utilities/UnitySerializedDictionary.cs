using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility
{
	public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		private List<TKey> keyData = new List<TKey>();
	
		[SerializeField, HideInInspector]
		private List<TValue> valueData = new List<TValue>();

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Clear();
			for (int i = 0; i < keyData.Count && i < this.valueData.Count; i++)
			{
				this[keyData[i]] = valueData[i];
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			keyData.Clear();
			valueData.Clear();

			foreach (KeyValuePair<TKey, TValue> item in this)
			{
				keyData.Add(item.Key);
				valueData.Add(item.Value);
			}
		}
	}
}