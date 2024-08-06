using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class AIData
	{
		public Dictionary<string, Collider[]> AvoidedObjectCollection = null;

		public Transform currentTarget;
		public List<Transform> Targets = null;

		public int GetTargetsCount()
		{
			return Targets == null ? 0 : Targets.Count;
		}
	}
}