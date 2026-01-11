# if UNITY_EDITOR
using System;
using UnityEngine;
namespace AstekUtility.Editor
{
    public class ExampleSerializableType : MonoBehaviour
    {
        [SerializeField, TypeFilter(typeof(ITest))] private SerializableType _test1;
        [SerializeField, TypeFilter(typeof(TestBase))] private SerializableType _test2;

        private void Awake()
        {
            Debug.Log(_test1.Type.BaseType);
            Debug.Log(_test2.Type.BaseType);
        }
    }

    public interface ITest { }
    public class Test1 : ITest { }
    public class Test2 : ITest { }
    public class Test3 : ITest { }

    public abstract class TestBase { }
    public class Test4 : TestBase { }
    public class Test5 : TestBase { }
    public class Test6 : TestBase { }
}
#endif