using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.SOAP
{
	//Use this so that value of scriptable object always resets
	public abstract class RuntimeScriptableObject : ScriptableObject
	{
		private static readonly List<RuntimeScriptableObject> Instances = new();

		private void OnEnable() => Instances.Add(this);
		private void OnDisable() => Instances.Remove(this);

		protected abstract void OnReset();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ResetAllInstances()
		{
			foreach (RuntimeScriptableObject instance in Instances)
				instance.OnReset();
		}
	}
}