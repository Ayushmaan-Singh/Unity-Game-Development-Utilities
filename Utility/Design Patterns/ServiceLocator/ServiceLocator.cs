using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstekUtility.DesignPattern.ServiceLocatorTool
{
	public sealed class ServiceLocator : MonoBehaviour
	{
		private static ServiceLocator _global;
		private static Dictionary<Scene, ServiceLocator> _sceneContainers;
		private static List<GameObject> _tmpSceneGameObjects;

		private readonly ServiceManager _services = new ServiceManager();

		private const string GLOBALSERVICELOCATORNAME = "ServiceLocator [Global]";
		private const string SCENESERVICELOCATORNAME = "ServiceLocator [Scene]";

		private static bool _isApplicationGettingClosed;

		#region Service Locator

		internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
		{
			if (_global == this)
			{
				Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
			}
			else if (_global)
			{
				Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
			}
			else
			{
				_global = this;
				if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
			}
		}
		internal void ConfigureForScene()
		{
			Scene scene = gameObject.scene;

			if (_sceneContainers.ContainsKey(scene))
			{
				Debug.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
				return;
			}

			_sceneContainers.Add(scene, this);
		}

		/// <summary>
		/// Gets the global ServiceLocator instance. Creates new if none exists.
		/// </summary>        
		public static ServiceLocator Global
		{
			get
			{
				if (_global) return _global;

				if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
				{
					found.BootstrapOnDemand();
					return _global;
				}

				if (!_isApplicationGettingClosed)
				{
					GameObject container = new GameObject(GLOBALSERVICELOCATORNAME, typeof(ServiceLocator));
					container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();
				}
				return _global.OrNull();
			}
		}

		/// <summary>
		/// Returns the <see cref="ServiceLocator"/> configured for the scene of a MonoBehaviour. Falls back to the global instance.
		/// </summary>
		public static ServiceLocator ForSceneOf(MonoBehaviour mb)
		{
			Scene scene = mb.gameObject.scene;

			if (_sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != mb)
			{
				return container;
			}

			_tmpSceneGameObjects.Clear();
			scene.GetRootGameObjects(_tmpSceneGameObjects);

			foreach (GameObject go in _tmpSceneGameObjects.Where(go => go.GetComponent<ServiceLocatorScene>()))
			{
				if (go.TryGetComponent(out ServiceLocatorScene bootstrapper) && bootstrapper.Container != mb)
				{
					bootstrapper.BootstrapOnDemand();
					return bootstrapper.Container;
				}
			}

			return Global;
		}

		/// <summary>
		/// Gets the closest ServiceLocator instance to the provided 
		/// MonoBehaviour in hierarchy, the ServiceLocator for its scene, or the global ServiceLocator.
		/// </summary>
		public static ServiceLocator For(MonoBehaviour mb)
		{
			return mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
		}

		#endregion

		#region Service

		/// <summary>
		/// Registers a service to the ServiceLocator using the service's type.
		/// </summary>
		/// <param name="service">The service to register.</param>  
		/// <typeparam name="T">Class type of the service to be registered.</typeparam>
		/// <returns>The ServiceLocator instance after registering the service.</returns>
		public ServiceLocator Register<T>(T service)
		{
			_services.Register(service);
			return this;
		}
		/// <summary>
		/// Registers a service to the ServiceLocator using a specific type.
		/// </summary>
		/// <param name="type">The type to use for registration.</param>
		/// <param name="service">The service to register.</param>  
		/// <returns>The ServiceLocator instance after registering the service.</returns>
		public ServiceLocator Register(Type type, object service)
		{
			_services.Register(type, service);
			return this;
		}

		public bool TryGet<T>(out T service) where T : class => _services.TryGet(out service);
		private bool TryGetNextInHierarchy(out ServiceLocator container)
		{
			if (this == _global)
			{
				container = null;
				return false;
			}

			container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
			return container.OrNull();
		}

		#endregion

		private void OnApplicationQuit() => _isApplicationGettingClosed = true;
		private void OnDestroy()
		{
			if (this == _global)
			{
				_global = null;
			}
			else if (_sceneContainers.ContainsValue(this))
			{
				_sceneContainers.Remove(gameObject.scene);
			}
		}

		// https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void ResetStatics()
		{
			_global = null;
			_isApplicationGettingClosed = false;
			_sceneContainers = new Dictionary<Scene, ServiceLocator>();
			_tmpSceneGameObjects = new List<GameObject>();
		}

#if UNITY_EDITOR
		[MenuItem("GameObject/ServiceLocator/Add Global")]
		static void AddGlobal()
		{
			GameObject go = new GameObject(GLOBALSERVICELOCATORNAME, typeof(ServiceLocatorGlobal));
		}

		[MenuItem("GameObject/ServiceLocator/Add Scene")]
		static void AddScene()
		{
			GameObject go = new GameObject(SCENESERVICELOCATORNAME, typeof(ServiceLocatorScene));
		}
#endif
	}

	public static class ServiceLocatorExtensionMethods
	{
		public static ServiceLocator LocalServiceLocator(this MonoBehaviour self) => ServiceLocator.For(self);
		public static ServiceLocator SceneServiceLocator(this MonoBehaviour self) => ServiceLocator.ForSceneOf(self);
		public static T GetService<T>(this MonoBehaviour self) where T : class =>
			self.LocalServiceLocator() && self.LocalServiceLocator().TryGet(out T service) ? service
			: self.SceneServiceLocator() && self.SceneServiceLocator().TryGet(out service) ? service
			: ServiceLocator.Global.TryGet(out service) ? service
			: throw new NullReferenceException($"Cannot Find Service Of Type {typeof(T)}");
		public static bool TryGetService<T>(this MonoBehaviour self, out T service) where T : class =>
			(self.LocalServiceLocator() && self.LocalServiceLocator().TryGet(out service))
			|| (self.SceneServiceLocator() && self.SceneServiceLocator().TryGet(out service))
			|| ServiceLocator.Global.TryGet(out service)
				? true : false;
	}
}