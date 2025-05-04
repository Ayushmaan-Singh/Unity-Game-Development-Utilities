using System;
using System.Reflection;
using UnityEngine;

namespace AstekUtility
{
	public static class TypeExtensions
	{
		public static void InvokeMethodByName<T>(this Type scriptToInvokeMethodFrom, T objectInstance, string functionName, params object[] parameters) where T : class
		{
			ParameterInfo[] parametersRequired = scriptToInvokeMethodFrom.GetMethod(functionName)?.GetParameters();
			if (parametersRequired != null && parametersRequired.Length == parameters.Length)
			{
				for (int i = 0; i < parametersRequired.Length; i++)
				{
					if (parametersRequired[i].ParameterType != parameters[i].GetType())
					{
						Debug.LogError($"Parameter Type Mismatch: {parametersRequired[i].GetType()} != {parameters[i].GetType()}");
						return;
					}
				}
			}
			scriptToInvokeMethodFrom.GetMethod(functionName)?.Invoke(objectInstance, parameters);
		}
	}
}