#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.InterfaceReferenceTest
{
	public class Example : MonoBehaviour,ITestable
	{
		public InterfaceReference<ITestable> interfaceTest;
		public InterfaceReference<ITestable, MonoBehaviour> interfaceOnObjectTest;

		public InterfaceReference<ITestable>[] arrayInterfaceTest;
		public List<InterfaceReference<ITestable>> listInterfaceTest;
	}

	public interface ITestable { }
}

#endif