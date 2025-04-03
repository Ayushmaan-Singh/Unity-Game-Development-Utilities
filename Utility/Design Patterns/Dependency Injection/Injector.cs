using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AstekUtility.DesignPattern.DependencyInjection
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
	public sealed class InjectAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ProvideAttribute : Attribute { }

	public interface IDependencyProvider { }

	[DefaultExecutionOrder(-1000)]
	public class Injector : MonoBehaviour
	{
		private const BindingFlags k_bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private readonly Dictionary<Type, object> registry = new Dictionary<Type, object>();

		private void Awake()
		{
			MonoBehaviour[] monoBehaviours = FindMonoBehaviours();

			// Find all modules implementing IDependencyProvider and register the dependencies they provide
			IEnumerable<IDependencyProvider> providers = monoBehaviours.OfType<IDependencyProvider>();
			foreach (IDependencyProvider provider in providers)
			{
				Register(provider);
			}

			// Find all injectable objects and inject their dependencies
			IEnumerable<MonoBehaviour> injectables = monoBehaviours.Where(IsInjectable);
			foreach (MonoBehaviour injectable in injectables)
			{
				Inject(injectable);
			}
		}

		// Register an instance of a type outside the normal dependency injection process
		public void Register<T>(T instance)
		{
			registry[typeof(T)] = instance;
		}

		private void Inject(object instance)
		{
			Type type = instance.GetType();

			// Inject into fields
			IEnumerable<FieldInfo> injectableFields = type.GetFields(k_bindingFlags)
				.Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

			foreach (FieldInfo injectableField in injectableFields)
			{
				if (injectableField.GetValue(instance) != null)
				{
					Debug.LogWarning($"[Injector] Field '{injectableField.Name}' of class '{type.Name}' is already set.");
					continue;
				}
				Type fieldType = injectableField.FieldType;
				object resolvedInstance = Resolve(fieldType);
				if (resolvedInstance == null)
				{
					throw new Exception($"Failed to inject dependency into field '{injectableField.Name}' of class '{type.Name}'.");
				}

				injectableField.SetValue(instance, resolvedInstance);
			}

			// Inject into methods
			IEnumerable<MethodInfo> injectableMethods = type.GetMethods(k_bindingFlags)
				.Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

			foreach (MethodInfo injectableMethod in injectableMethods)
			{
				Type[] requiredParameters = injectableMethod.GetParameters()
					.Select(parameter => parameter.ParameterType)
					.ToArray();
				object[] resolvedInstances = requiredParameters.Select(Resolve).ToArray();
				if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
				{
					throw new Exception($"Failed to inject dependencies into method '{injectableMethod.Name}' of class '{type.Name}'.");
				}

				injectableMethod.Invoke(instance, resolvedInstances);
			}

			// Inject into properties
			IEnumerable<PropertyInfo> injectableProperties = type.GetProperties(k_bindingFlags)
				.Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
			foreach (PropertyInfo injectableProperty in injectableProperties)
			{
				Type propertyType = injectableProperty.PropertyType;
				object resolvedInstance = Resolve(propertyType);
				if (resolvedInstance == null)
				{
					throw new Exception($"Failed to inject dependency into property '{injectableProperty.Name}' of class '{type.Name}'.");
				}

				injectableProperty.SetValue(instance, resolvedInstance);
			}
		}

		public void Register(IDependencyProvider provider)
		{
			MethodInfo[] methods = provider.GetType().GetMethods(k_bindingFlags);

			foreach (MethodInfo method in methods)
			{
				if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

				Type returnType = method.ReturnType;
				object providedInstance = method.Invoke(provider, null);
				if (providedInstance != null)
				{
					registry.Add(returnType, providedInstance);
				}
				else
				{
					throw new Exception($"Provider method '{method.Name}' in class '{provider.GetType().Name}' returned null when providing type '{returnType.Name}'.");
				}
			}
		}

		public void ValidateDependencies()
		{
			MonoBehaviour[] monoBehaviours = FindMonoBehaviours();
			IEnumerable<IDependencyProvider> providers = monoBehaviours.OfType<IDependencyProvider>();
			HashSet<Type> providedDependencies = GetProvidedDependencies(providers);

			IEnumerable<string> invalidDependencies = monoBehaviours
				.SelectMany(mb => mb.GetType().GetFields(k_bindingFlags), (mb, field) => new
				{
					mb,
					field
				})
				.Where(t => Attribute.IsDefined(t.field, typeof(InjectAttribute)))
				.Where(t => !providedDependencies.Contains(t.field.FieldType) && t.field.GetValue(t.mb) == null)
				.Select(t => $"[Validation] {t.mb.GetType().Name} is missing dependency {t.field.FieldType.Name} on GameObject {t.mb.gameObject.name}");

			List<string> invalidDependencyList = invalidDependencies.ToList();

			if (!invalidDependencyList.Any())
			{
				Debug.Log("[Validation] All dependencies are valid.");
			}
			else
			{
				Debug.LogError($"[Validation] {invalidDependencyList.Count} dependencies are invalid:");
				foreach (var invalidDependency in invalidDependencyList)
				{
					Debug.LogError(invalidDependency);
				}
			}
		}

		private HashSet<Type> GetProvidedDependencies(IEnumerable<IDependencyProvider> providers)
		{
			HashSet<Type> providedDependencies = new HashSet<Type>();
			foreach (IDependencyProvider provider in providers)
			{
				MethodInfo[] methods = provider.GetType().GetMethods(k_bindingFlags);

				foreach (MethodInfo method in methods)
				{
					if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

					Type returnType = method.ReturnType;
					providedDependencies.Add(returnType);
				}
			}

			return providedDependencies;
		}

		public void ClearDependencies()
		{
			foreach (MonoBehaviour monoBehaviour in FindMonoBehaviours())
			{
				Type type = monoBehaviour.GetType();
				IEnumerable<FieldInfo> injectableFields = type.GetFields(k_bindingFlags)
					.Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

				foreach (FieldInfo injectableField in injectableFields)
				{
					injectableField.SetValue(monoBehaviour, null);
				}
			}

			Debug.Log("[Injector] All injectable fields cleared.");
		}

		private object Resolve(Type type)
		{
			registry.TryGetValue(type, out object resolvedInstance);
			return resolvedInstance;
		}

		private static MonoBehaviour[] FindMonoBehaviours()
		{
			return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
		}

		private static bool IsInjectable(MonoBehaviour obj)
		{
			MemberInfo[] members = obj.GetType().GetMembers(k_bindingFlags);
			return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
		}
	}
}