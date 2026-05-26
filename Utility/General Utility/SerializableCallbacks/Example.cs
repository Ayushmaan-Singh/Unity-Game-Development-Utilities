#if UNITY_EDITOR
using UnityEngine;

namespace Astek.SerializableMethods.Editor
{
    public class Example:MonoBehaviour
    {
        [SerializeField] private SerializedCallback<int> testCalc;

        public int TestCalcMethod(int a1, int a2, Vector3 a3)
        {
            return a1 + a2+(int)(a1*a3).x;
        }
    }
}

#endif