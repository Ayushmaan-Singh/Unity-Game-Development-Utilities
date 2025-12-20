using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
namespace AstekUtility.Gameplay.Geometric_Calculations
{
    public class TestArcGeometry : MonoBehaviour
    {
        [SerializeField] private Arc2D arcGeo;

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

            if (arcGeo.ArcPointsTransform.Length < arcGeo.PointsOnArc)
                return;

            for (int i = 0; i < arcGeo.PointsOnArc; i++)
            {
                GameObject instance = Instantiate(obj);
                spawned.Add(instance.transform);
                instance.transform.position = ((Vector3)arcGeo.ArcPointsTransform[i].Position).With(z: i * 0.1f);
                instance.transform.rotation = Quaternion.Euler(0, 0, arcGeo.ArcPointsTransform[i].RotationDeg);
            }
        }

        private void OnDrawGizmos()
        {
            arcGeo.OnDrawGizmos();

            if (spawned.Count < arcGeo.PointsOnArc)
                return;

            for (int i = 0; i < spawned.Count; i++)
            {
                spawned[i].position = ((Vector3)arcGeo.ArcPointsTransform[i].Position).With(z: i * 0.1f);
                spawned[i].rotation = Quaternion.Euler(0, 0, arcGeo.ArcPointsTransform[i].RotationDeg);
            }
        }
    }
}