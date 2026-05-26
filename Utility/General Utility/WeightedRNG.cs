using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;
using UnityEngine;

namespace Astek
{
	/// <summary>
	///     Create a object that can be randomly selected from a set of other with a set chance of getting selected
	/// </summary>
	[Serializable]
	public class WeightedRNG<T>
	{
		[field:SerializeField] public T Value { get; private set; }
		[field:SerializeField] public int Probability { get; private set; }
		public int SetProbability
		{
			set => Probability = value;
		}

		public WeightedRNG() { }

		public WeightedRNG(T value, int probability)
		{
			Value = value;
			Probability = probability;
		}
	}

	[Serializable]
	public class WeightsCollection<T> : IList<WeightedRNG<T>>
	{
		private List<WeightedRNG<T>> _collection = new List<WeightedRNG<T>>();
		[ShowInInspector] public WeightedRNG<T> this[int index]
		{
			get => _collection[index];
			set
			{
				_collection.Add(value);
				_collection = _collection.OrderBy(w => w.Probability).ToList();
			}
		}
		public int Count => _collection.Count;
		public bool IsReadOnly => false;

		public int IndexOf(WeightedRNG<T> item) => _collection.IndexOf(item);
		public void Insert(int index, WeightedRNG<T> item) => _collection.Insert(index, item);
		public void RemoveAt(int index) => _collection.RemoveAt(index);
		public IEnumerator<WeightedRNG<T>> GetEnumerator() => _collection.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public void Add(WeightedRNG<T> item)
		{
			_collection.Add(item);
			_collection = _collection.OrderBy(w => w.Probability).ToList();
		}
		public void Clear() => _collection.Clear();
		public bool Contains(WeightedRNG<T> item) => _collection.Contains(item);
		public void CopyTo(WeightedRNG<T>[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);
		public bool Remove(WeightedRNG<T> item) => _collection.Remove(item);
	};

	/// <summary>
	///     Get a Random object from a list depending on the probability of selection
	/// </summary>
	public static class CalculateWeightedRNG
	{
		public static T GetRandomValue<T>(this WeightsCollection<T> collection)
		{
			int totalProbability = 0;
			foreach (WeightedRNG<T> item in collection)
			{
				totalProbability += item.Probability;
			}

			int rand = Random.Range(1, totalProbability);
			int currentProb = 0;

			foreach (WeightedRNG<T> selection in collection)
			{
				currentProb += selection.Probability;
				if (rand <= currentProb)
					return selection.Value;
			}
			return default(T);
		}
	}
}