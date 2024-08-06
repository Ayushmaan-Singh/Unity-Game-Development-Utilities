using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AstekUtility;
using MoreMountains.Tools;
using UnityEngine;

namespace AstekUtility.Gameplay.DStarLite
{
	public class Node<T>
	{
		public T Data { get; set; }

		/// <summary>
		/// Calculate the "real" cost from one node to another
		/// </summary>
		public Func<Node<T>, Node<T>, float> Cost { get; set; }

		/// <summary>
		/// Estimate the cost from one to another node
		/// </summary>
		public Func<Node<T>, Node<T>, float> Heuristic { get; }

		public float G { get; set; }   //Real cost from start to this node
		public float RHS { get; set; } //Estimated cost from start to this node 
		public bool GEqualRHS => G.Approx(RHS);

		public List<Node<T>> Neighbours { get; set; } = new();

		public Node(T data, Func<Node<T>, Node<T>, float> cost, Func<Node<T>, Node<T>, float> heuristic)
		{
			Data = data;
			Cost = cost;
			Heuristic = heuristic;

			G = float.MaxValue;
			RHS = float.MaxValue;
		}
	}

	public readonly struct Key
	{
		private readonly float _k1;
		private readonly float _k2;

		public Key(float k1, float k2)
		{
			_k1 = k1;
			_k2 = k2;
		}
		public static bool operator <(Key a, Key b) => a._k1 < b._k1 || a._k1.Approx(b._k1) && a._k2 < b._k2;
		public static bool operator >(Key a, Key b) => a._k1 > b._k1 || a._k1.Approx(b._k1) && a._k2 > b._k2;

		public static bool operator ==(Key a, Key b) => a._k1.Approx(b._k1) && a._k2.Approx(b._k2);
		public static bool operator !=(Key a, Key b) => !(a == b);

		public override bool Equals(object obj) => obj is Key key && this == key;
		public override int GetHashCode() => HashCode.Combine(_k1, _k2);
		public override string ToString() => $"{_k1}, {_k2}";

	}

	public class DStarLite<T>
	{
		private readonly Node<T> _startNode;
		private readonly Node<T> _goalNode;
		private readonly List<Node<T>> _allNodes;
		private float _km; //key modifier

		class KeyNodeComparer : IComparer<(Key, Node<T>)>
		{
			public int Compare((Key, Node<T>) x, (Key, Node<T>) y) => x.Item1 < y.Item1 ? -1 : x.Item1 > y.Item1 ? 1 : 0;
		}

        // TODO Move to a custom priority queue implementation
		//SortedSet will add or remove elements in O(log n) time and fetch the minimum element in O(1) time
		private readonly SortedSet<(Key, Node<T>)> _openSet = new SortedSet<(Key, Node<T>)>();
		//Dictionary will add or remove elements in O(1) time and fetch the minimum element in O(1) time
		private readonly Dictionary<Node<T>, Key> _lookups = new Dictionary<Node<T>, Key>();

		public DStarLite(Node<T> start, Node<T> goal, List<Node<T>> allNodes)
		{
			_startNode = start;
			_goalNode = goal;
			_allNodes = allNodes;
		}


		private const int k_maxCycles = 1000;

		private void ComputeShortestPath()
		{
			int maxSteps = k_maxCycles;
			while (_openSet.Count > 0 && _openSet.Min.Item1 < CalculateKey(_startNode) || _startNode.RHS > _startNode.G)
			{
				if (maxSteps-- <= 0)
				{
					Debug.LogWarning("ComputeShortestPath error: max steps exceeded");
					break;
				}

				(Key, Node<T>) smallest = _openSet.Min;
				_openSet.Remove(smallest);
				_lookups.Remove(smallest.Item2);
				Node<T> node = smallest.Item2;

				//If we have a key value for this node then the value of the node has changed since we put it in the openset
				if (smallest.Item1 < CalculateKey(node))
				{
					Key newKey = CalculateKey(node);
					_openSet.Add((newKey, node));
					_lookups[node] = newKey;
				}
				//Found a more optimal path
				else if (node.G > node.RHS)
				{
					node.G = node.RHS;
					foreach (Node<T> predecessor in Predecessors(node))
					{
						if (predecessor != _goalNode)
						{
							predecessor.RHS = Mathf.Min(predecessor.RHS, predecessor.Cost(predecessor, node) + node.G);
						}

						UpdateVertex(predecessor);
					}
				}
				else
				{
					float gOld = node.G;
					node.G = float.MaxValue;
					foreach (Node<T> predecessor in Predecessors(node).Concat(new[]
					         {
						         node
					         }))
					{
						if (predecessor.RHS.Approx(predecessor.Cost(predecessor, node) + gOld))
						{
							if (predecessor != _goalNode)
							{
								predecessor.RHS = float.MaxValue;
							}

							foreach (Node<T> successor in Successors(node))
							{
								predecessor.RHS = Mathf.Min(predecessor.RHS, predecessor.Cost(predecessor, successor) + successor.G);
							}
						}
						UpdateVertex(predecessor);
					}
				}
			}

			_startNode.G = _startNode.RHS;

			#if UNITY_EDITOR
			Debug.Log($"Shortest Path Computed in {k_maxCycles - maxSteps} steps");
			#endif
		}

		public void RecalculateNode(Node<T> node)
		{
			_km += _startNode.Heuristic(_startNode, node);

			List<Node<T>> allConnectedNodes = Successors(node).Concat(Predecessors(node)).ToList();

			foreach (Node<T> s in allConnectedNodes)
			{
				if (s != _startNode)
				{
					s.RHS = Mathf.Min(s.RHS, s.Cost(s, node) + node.G);
				}

				UpdateVertex(s);
			}

			UpdateVertex(node);
			ComputeShortestPath();
		}

		/// <summary>
		/// May need to be more complex depending on Type T
		/// f. eks. reurn allNode.Where(n=>n.Neighbours.Contains(node))
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private IEnumerable<Node<T>> Predecessors(Node<T> node) => node.Neighbours;

		/// <summary>
		/// May need to be more complex depending on Type T
		/// f. eks. reurn allNode.Where(n=>n.Neighbours.Contains(node))
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private IEnumerable<Node<T>> Successors(Node<T> node) => node.Neighbours;


		private void UpdateVertex(Node<T> node)
		{
			Key key = CalculateKey(node);
			if (!node.GEqualRHS && !_lookups.ContainsKey(node))
			{
				_openSet.Add((key, node));
				_lookups[node] = key;
			}
			else if (node.GEqualRHS && _lookups.ContainsKey(node))
			{
				_openSet.Remove((_lookups[node], node));
				_lookups.Remove(node);
			}
			else if (_lookups.ContainsKey(node))
			{
				_openSet.Remove((_lookups[node], node));
				_openSet.Add((key, node));
				_lookups[node] = key;
			}
		}

		public void Initialize()
		{
			_openSet.Clear();
			_lookups.Clear();
			_km = 0;

			foreach (Node<T> node in _allNodes)
			{
				node.G = float.MaxValue;
				node.RHS = float.MaxValue;
			}

			_goalNode.RHS = 0;
			Key key = CalculateKey(_goalNode);
			_openSet.Add((key, _goalNode));
			_lookups[_goalNode] = key;
		}

		private Key CalculateKey(Node<T> node)
		{
			return new Key(
				Mathf.Min(node.G, node.RHS) + node.Heuristic(node, _startNode) + _km,
				Mathf.Min(node.G, node.RHS));
		}
	}
}