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

		private static bool _stopSpawningGlobal = false;

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

				if (Application.isPlaying)
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
				if (!go.TryGetComponent(out ServiceLocatorScene bootstrapper) || bootstrapper.Container == mb)
					continue;
				bootstrapper.BootstrapOnDemand();
				return bootstrapper.Container;
			}

			return Global;
		}

		/// <summary>
		/// Returns the <see cref="ServiceLocator"/> configured for the scene of a Renderer. Falls back to the global instance.
		/// </summary>
		public static ServiceLocator ForSceneOf(Renderer renderer)
		{
			Scene scene = renderer.gameObject.scene;

			if (_sceneContainers.TryGetValue(scene, out ServiceLocator container)
			    && renderer.GetComponents<MonoBehaviour>().Any(component => component == container))
			{
				return container;
			}

			_tmpSceneGameObjects.Clear();
			scene.GetRootGameObjects(_tmpSceneGameObjects);

			foreach (GameObject go in _tmpSceneGameObjects.Where(go => go.GetComponent<ServiceLocatorScene>()))
			{
				if (!go.TryGetComponent(out ServiceLocatorScene bootstrapper) || 
				    renderer.GetComponents<MonoBehaviour>().Any(component => component == bootstrapper))
					continue;
				bootstrapper.BootstrapOnDemand();
				return bootstrapper.Container;
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
		/// <param name="overrideService">Pass if we want to override with this value if another already exsists</param>
		/// <returns>The ServiceLocator instance after registering the service.</returns>
		public ServiceLocator Register<T>(T service, bool overrideService = false)
		{
			_services.Register(service, overrideService);
			return this;
		}
		/// <summary>
		/// Registers a service to the ServiceLocator using a specific type.
		/// </summary>
		/// <param name="type">The type to use for registration.</param>
		/// <param name="service">The service to register.</param>
		/// <param name="overrideService">Pass if we want to override with this value if another already exsists</param>
		/// <returns>The ServiceLocator instance after registering the service.</returns>
		public ServiceLocator Register(Type type, object service, bool overrideService = false)
		{
			_services.Register(type, service, overrideService);
			return this;
		}

		public T GetService<T>() where T : class
		{
			if (TryGetService(out T service))
				return service;

			ServiceLocator container = _sceneContainers.TryGetValue(gameObject.scene, out ServiceLocator sceneContainer)
				? sceneContainer == this ? _global : sceneContainer : _global;

			if (container && container.TryGetService(out service))
				return service;

			//If this is global then no place left to check
			//If we cannot find service registered anywhere
			throw new NullReferenceException($"Service Of Type:{typeof(T).FullName} Is Not Registered");
		}

		public bool TryGetService<T>(out T service) where T : class => _services.TryGet(out service);

		#endregion

		private void OnDestroy()
		{
			if (this == _global)
			{
				_stopSpawningGlobal = true;
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
			_sceneContainers = new Dictionary<Scene, ServiceLocator>();
			_tmpSceneGameObjects = new List<GameObject>();
			_stopSpawningGlobal = false;
		}
		private class ServiceManager
		{
			private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
			public IEnumerable<object> RegisteredServices => _services.Values;

			public ServiceManager() { }

			public bool TryGet<T>(out T service) where T : class
			{
				Type type = typeof(T);
				if (_services.TryGetValue(type, out object serviceObj))
				{
					service = serviceObj as T;
					return true;
				}
				service = null;
				return false;
			}

			public ServiceManager Register<T>(T service, bool overrideService)
			{
				Type type = typeof(T);
				if (!_services.TryAdd(type, service))
				{
					if (!overrideService)
						throw new Exception($"ServiceManager.Register: Service of type{type.FullName} already registered");
					else
						_services[type] = service;
				}

				return this;
			}
			public ServiceManager Register(Type type, object service, bool overrideService)
			{
				if (!type.IsInstanceOfType(service))
					throw new ArgumentException($"Type of service does not match type of service interface {nameof(service)}");

				if (!_services.TryAdd(type, service))
				{
					if (!overrideService)
						Debug.LogError($"ServiceManager.Register: Services of type {type.FullName} already registered");
					else
						_services[type] = service;
				}

				return this;
			}
		}

		#region Editor Only

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

		#endregion
	}
}