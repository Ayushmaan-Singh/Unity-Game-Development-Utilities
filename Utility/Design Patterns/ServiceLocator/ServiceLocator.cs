using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.ServiceLocatorTool
{
    /// <summary>
    /// Simple service locator for <see cref="IGameService"/> instances.
    /// </summary>
    public sealed class ServiceLocator
    {
        public static ServiceLocator Instance { get; private set; } = new ServiceLocator();

        //Making default constructor private prevents initialization
        private ServiceLocator() { }

        private readonly Dictionary<Type, IGameService> services = new Dictionary<Type, IGameService>();

        /// <summary>
        /// Gets the service instance of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to lookup.</typeparam>
        /// <returns>The service instance.</returns>
        public T Get<T>() where T : IGameService
        {
            if (!services.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Service Locator: {typeof(T).Name} not registered");
                throw new InvalidOperationException();
            }

            return (T)services[typeof(T)];
        }

        /// <summary>
        /// Registers the service with the current service locator.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="service">Service instance.</param>
        public void Register<T>(T service) where T : IGameService
        {
            if (services.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Service Locator: Attempted to register service of type {typeof(T).Name} which is already registered.");
                return;
            }

            services.Add(typeof(T), service);
        }

        /// <summary>
        /// Unregisters the service from the current service locator.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        public void Unregister<T>() where T : IGameService
        {
            if (!services.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Service Locator: Attempted to unregister service of type {typeof(T).Name} which is not registered.");
                return;
            }

            services.Remove(typeof(T));
        }

        /// <summary>
        /// Uregisters all the services registered at any given moment
        /// </summary>
        public void UnregisterAll()
        {
            foreach(Type service in services.Keys)
            {
                services.Remove(service);
            }

            Debug.Log("Service Locator: All services are unregistered");
        }

#if UNITY_EDITOR

        /// <summary>
        /// This is Used so that static variables are refreshed once the play mode is over when having domain reload disabled
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetStatic()
        {
            Instance = new ServiceLocator();
        }

#endif
    }
}
