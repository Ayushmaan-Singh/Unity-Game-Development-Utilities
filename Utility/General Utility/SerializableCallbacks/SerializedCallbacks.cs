using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Astek.SerializableMethods
{
    [Serializable]
    public class SerializedCallback<TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField] private Object targetObject;
        [SerializeField] private string methodName;
        [SerializeField] private AnyValue[] parameters;

        [NonSerialized] private Delegate _cachedDelegate;
        [NonSerialized] private bool _isDelegateRebuilt;

        public TReturn Invoke()
        {
            return Invoke(parameters);
        }

        public TReturn Invoke(params AnyValue[] args)
        {
            if (!_isDelegateRebuilt) BuildDelegate();

            if (_cachedDelegate != null)
            {
                var result = _cachedDelegate.DynamicInvoke(ConvertParameters(args));
                return (TReturn)Convert.ChangeType(result, typeof(TReturn));
            }

            Debug.LogWarning($"Unable to invoke method {methodName} on {targetObject}");
            return default;
        }

        object[] ConvertParameters(AnyValue[] args)
        {
            if (args == null || args.Length == 0) return Array.Empty<object>();

            var convertedParams = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                convertedParams[i] = args[i].ConvertValue<object>();
            }

            return convertedParams;
        }

        void BuildDelegate()
        {
            _cachedDelegate = null;

            if (targetObject == null || string.IsNullOrEmpty(methodName))
            {
                Debug.LogWarning("Target object or method name is null, cannot rebuild delegate.");
                return;
            }

            Type targetType = targetObject.GetType();
            MethodInfo methodInfo = targetType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                Debug.LogWarning($"Method {methodName} not found on {targetObject}");
                return;
            }

            Type[] parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            if (parameters.Length != parameterTypes.Length)
            {
                Debug.LogWarning($"Parameter mismatch for method {methodName}");
                return;
            }

            Type delegateType = Expression.GetDelegateType(parameterTypes.Append(methodInfo.ReturnType).ToArray());
            _cachedDelegate = methodInfo.CreateDelegate(delegateType, targetObject);
            _isDelegateRebuilt = true;
        }

        public void OnBeforeSerialize()
        {
            // noop
        }

        public void OnAfterDeserialize()
        {
            _isDelegateRebuilt = false;
        }
    }
}