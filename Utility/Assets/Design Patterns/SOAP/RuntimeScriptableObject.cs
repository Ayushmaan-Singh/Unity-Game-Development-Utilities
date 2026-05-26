using System;
using System.Collections.Generic;
using UnityEngine;

namespace Astek.SOAP
{
	//Use this so that value of scriptable object always resets
	public abstract class RuntimeScriptableObject : ScriptableObject
	{
		private static readonly List<RuntimeScriptableObject> Instances = new();

		protected virtual void OnEnable() => Instances.Add(this);
		protected virtual void OnDisable() => Instances.Remove(this);

		protected abstract void OnReset();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ResetAllInstances()
		{
			foreach (RuntimeScriptableObject instance in Instances)
				instance.OnReset();
		}
	}
}