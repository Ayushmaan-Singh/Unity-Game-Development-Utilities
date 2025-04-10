﻿using System;
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

		/// <summary>
		/// Gets a service of a specific type. If no service of the required type is found, an error is thrown.
		/// </summary>
		/// <param name="service">Service of type T to get.</param>  
		/// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
		/// <returns>The ServiceLocator instance after attempting to retrieve the service.</returns>
		public ServiceLocator Get<T>(out T service) where T : class
		{
			if (TryGetService(out service)) return this;

			if (TryGetNextInHierarchy(out ServiceLocator container))
			{
				container.Get(out service);
				return this;
			}

			throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
		}

		/// <summary>
		/// Allows retrieval of a service of a specific type. An error is thrown if the required service does not exist.
		/// </summary>
		/// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
		/// <returns>Instance of the service of type T.</returns>
		public T Get<T>() where T : class
		{
			Type type = typeof(T);

			if (TryGetService(type, out T service)) return service;

			if (TryGetNextInHierarchy(out ServiceLocator container))
				return container.Get<T>();

			throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
		}

		/// <summary>
		/// Tries to get a service of a specific type. Returns whether the process is successful.
		/// </summary>
		/// <param name="service">Service of type T to get.</param>  
		/// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
		/// <returns>True if the service retrieval was successful, false otherwise.</returns>
		public bool TryGet<T>(out T service) where T : class
		{
			Type type = typeof(T);
			service = null;

			if (TryGetService(type, out service))
				return true;

			return TryGetNextInHierarchy(out ServiceLocator container) && container.TryGet(out service);
		}

		private bool TryGetService<T>(out T service) where T : class
		{
			return _services.TryGet(out service);
		}

		private bool TryGetService<T>(Type type, out T service) where T : class
		{
			return _services.TryGet(out service);
		}

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

		private void OnApplicationQuit()
		{
			_isApplicationGettingClosed = true;
		}
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
}