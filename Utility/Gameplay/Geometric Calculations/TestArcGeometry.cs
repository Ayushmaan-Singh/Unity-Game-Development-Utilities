using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
namespace AstekUtility.Gameplay.Geometric_Calculations
{
	public class TestArcGeometry : MonoBehaviour
	{
		[SerializeField] private CalculatePositionsOnArc2D arcGeo;

		[ShowInInspector] private readonly List<Transform> spawned = new List<Transform>();

		[Button]
		private void TestWithCardSpawn(GameObject obj)
		{
			for (int i = 0; i < spawned.Count; i++)
			{
				Transform go = spawned[0];
				spawned.Remove(go);
				DestroyImmediate(spawned[0].gameObject);
			}

			if (arcGeo._position.Count < arcGeo.PointsOnArc)
				return;

			for (int i = 0; i < arcGeo.PointsOnArc; i++)
			{
				GameObject instance = Instantiate(obj);
				spawned.Add(instance.transform);
				instance.transform.position = ((Vector3)arcGeo._position[i]).With(z:i * 0.1f);
				instance.transform.up = arcGeo._up[i];
			}
		}

		private void OnDrawGizmos()
		{
			arcGeo.OnDrawGizmos();

			if (spawned.Count < arcGeo.PointsOnArc)
				return;

			for (int i = 0; i < spawned.Count; i++)
			{
				spawned[i].position = ((Vector3)arcGeo._position[i]).With(z:i * 0.1f);
				spawned[i].up = arcGeo._up[i];
			}
		}
	}
}