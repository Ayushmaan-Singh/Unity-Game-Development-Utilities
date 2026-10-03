using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Astek.DesignPattern.ServiceLocatorTool
{
    /// <summary>Thrown when a service cannot be resolved. Message contains the full lookup chain.</summary>
    public sealed class ServiceNotFoundException : InvalidOperationException
    {
        public Type ServiceType { get; }
        public ServiceNotFoundException(Type serviceType, string message) : base(message) => ServiceType = serviceType;
    }

    /// <summary>Process-wide dense integer id per service type. Lets every locator use array indexing instead of hashing.</summary>
    internal static class ServiceTypeId
    {
        // Single, atomic global counter. Guaranteed thread-safe by the CPU.
        private static int _globalCounter = -1;

        // Fallback registry for dynamic runtime Type lookups
        private static readonly ConcurrentDictionary<Type, int> _dynamicCache = new();

        /// <summary>
        /// FAST PATH: Used when the type is known at compile time. 
        /// Zero locks, zero dictionary lookups, near-instant speed.
        /// </summary>
        public static int Of<T>() => Cache<T>.Id;

        /// <summary>
        /// DYNAMIC PATH: Used when you only have a System.Type variable at runtime.
        /// Lock-free, highly concurrent, and optimized.
        /// </summary>
        public static int Of(Type type)
        {
            // 1. Check the dynamic cache first. If it exists, returns instantly without allocation.
            if (_dynamicCache.TryGetValue(type, out int id)) return id;

            // 2. If missing, insert atomically using an thread-safe factory function.
            return _dynamicCache.GetOrAdd(type, t => Interlocked.Increment(ref _globalCounter));
        }

        // The .NET Runtime compiles a completely separate version of this class for every Type T.
        // The static constructor runs exactly once per type, making it perfectly thread-safe.
        private static class Cache<T>
        {
            public static readonly int Id = Interlocked.Increment(ref _globalCounter);

            static Cache()
            {
                // Edge-case protection: Ensure the dynamic path also knows about this ID
                // in case the same type is queried via both Of<T>() and Of(Type) paths.
                _dynamicCache.TryAdd(typeof(T), Id);
            }
        }
    }

    public class ServiceManager
    {
        struct Slot
        {
            public object Instance;
            public Func<object> Factory;
            public bool IsUnityObject;
            public bool Resolving;
        }

        enum AddMode
        {
            Strict,
            Silent,
            Overwrite
        }

        Slot[] slots = new Slot[8];
        Object owner;

        public ServiceManager() { }

        /// <param name="owner">Used as log context so console messages ping the owning locator.</param>
        public ServiceManager(Object owner) => this.owner = owner;

        /// <summary>Log context. Assigned by the owning ServiceLocator in Awake/Reset.</summary>
        internal Object Owner { set => owner = value; }

        /// <summary>Live, already-instantiated services. Does not trigger lazy factories.</summary>
        public IEnumerable<object> RegisteredServices
        {
            get
            {
                Slot[] snapshot = slots;
                for (int i = 0; i < snapshot.Length; i++)
                {
                    if (IsLive(in snapshot[i])) yield return snapshot[i].Instance;
                }
            }
        }

        /// <summary>Number of registered services (live instances + pending lazy factories).</summary>
        public int Count
        {
            get
            {
                int count = 0;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].Factory != null || IsLive(in slots[i])) count++;
                }
                return count;
            }
        }

        static bool IsLive(in Slot s) => s.Instance != null && (!s.IsUnityObject || (Object)s.Instance != null);

        #region Retrieval

        public bool TryGet<T>(out T service) where T : class
        {
            int id = ServiceTypeId.Of<T>();
            Slot[] arr = slots;

            if ((uint)id < (uint)arr.Length)
            {
                ref Slot slot = ref arr[id];
                object obj = slot.Instance;

                if (obj != null)
                {
                    if (!slot.IsUnityObject || (Object)obj != null)
                    {
                        service = (T)obj;
                        return true;
                    }

                    slot = default; // destroyed UnityEngine.Object: self-heal
                }
                else if (slot.Factory != null)
                {
                    return TryResolveLazy(id, out service);
                }
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            if (TryGet(out T service)) return service;

            throw new ServiceNotFoundException(typeof(T),
                $"ServiceManager.Get: Service of type {typeof(T).FullName} not registered");
        }

        /// <summary>True if T is registered here (instance alive, or lazy factory pending). Never instantiates.</summary>
        public bool IsRegistered<T>()
        {
            int id = ServiceTypeId.Of<T>();
            if ((uint)id >= (uint)slots.Length) return false;
            ref Slot slot = ref slots[id];
            return slot.Factory != null || IsLive(in slot);
        }

        bool TryResolveLazy<T>(int id, out T service) where T : class
        {
            Func<object> factory = slots[id].Factory;

            if (slots[id].Resolving)
                throw new InvalidOperationException($"ServiceManager: Circular dependency while lazily creating {typeof(T).FullName}");

            slots[id].Resolving = true;
            object obj;
            try
            {
                obj = factory();
            }
            finally
            {
                slots[id].Resolving = false;
            } // array may have grown during factory; index stays valid

            if (obj == null)
                throw new InvalidOperationException($"ServiceManager: Lazy factory for {typeof(T).FullName} returned null");
            if (!(obj is T typed))
                throw new InvalidOperationException($"ServiceManager: Lazy factory for {typeof(T).FullName} returned {obj.GetType().FullName}");

            ref Slot slot = ref slots[id];
            slot.Instance      = obj;
            slot.IsUnityObject = obj is Object;
            slot.Factory       = null;

            service = typed;
            return true;
        }

        #endregion

        #region Registration

        /// <summary>Registers under typeof(T). Duplicate -> logs error, keeps the existing one.</summary>
        public ServiceManager Register<T>(T service)
        {
            Add(ServiceTypeId.Of<T>(), typeof(T), service, AddMode.Strict);
            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (service == null) throw new ArgumentNullException(nameof(service));

            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));
            }

            Add(ServiceTypeId.Of(type), type, service, AddMode.Strict);
            return this;
        }

        /// <summary>Like Register but silent: returns false instead of logging when already registered.</summary>
        public bool TryRegister<T>(T service) => Add(ServiceTypeId.Of<T>(), typeof(T), service, AddMode.Silent);

        /// <summary>Registers, replacing any existing instance or pending factory.</summary>
        public ServiceManager Replace<T>(T service)
        {
            Add(ServiceTypeId.Of<T>(), typeof(T), service, AddMode.Overwrite);
            return this;
        }

        /// <summary>Registers a factory invoked on first resolve. Result is cached.</summary>
        public ServiceManager RegisterLazy<T>(Func<T> factory) where T : class
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            int id = ServiceTypeId.Of<T>();
            if ((uint)id >= (uint)slots.Length) Array.Resize(ref slots, Math.Max(id + 1, slots.Length * 2));

            ref Slot slot = ref slots[id];
            if (slot.Factory != null || IsLive(in slot))
            {
                Debug.LogError($"ServiceManager.Register: Service of type {typeof(T).FullName} already registered", owner);
                return this;
            }

            slot.Instance      = null;
            slot.IsUnityObject = false;
            slot.Factory       = () => factory();
            return this;
        }

        bool Add(int id, Type type, object service, AddMode mode)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            bool isUnity = service is Object;
            if (isUnity && (Object)service == null)
            {
                if (mode != AddMode.Silent)
                    Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} is a destroyed object", owner);
                return false;
            }

            if ((uint)id >= (uint)slots.Length) Array.Resize(ref slots, Math.Max(id + 1, slots.Length * 2));

            ref Slot slot = ref slots[id];
            if (mode != AddMode.Overwrite && (slot.Factory != null || IsLive(in slot)))
            {
                if (mode == AddMode.Strict)
                    Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered", owner);
                return false;
            }

            slot.Instance      = service;
            slot.Factory       = null;
            slot.IsUnityObject = isUnity;
            return true;
        }

        #endregion

        #region Removal

        /// <summary>Removes whatever is registered under T.</summary>
        public bool Unregister<T>() => Remove(ServiceTypeId.Of<T>(), null);

        /// <summary>
        /// Removes the registration under T only if it is this exact instance. Safe to call from OnDestroy
        /// even if someone else has since replaced the service. T must be the type it was registered as.
        /// </summary>
        public bool Unregister<T>(T service) => service != null && Remove(ServiceTypeId.Of<T>(), service);

        public bool Unregister(Type type) => type != null && Remove(ServiceTypeId.Of(type), null);

        /// <summary>Removes the instance from every type it is registered under. O(slots). Returns removed count.</summary>
        public int UnregisterInstance(object service)
        {
            if (service == null) return 0;

            int removed = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(slots[i].Instance, service))
                {
                    slots[i] = default;
                    removed++;
                }
            }
            return removed;
        }

        public void Clear() => Array.Clear(slots, 0, slots.Length);

        bool Remove(int id, object expected)
        {
            if ((uint)id >= (uint)slots.Length) return false;

            ref Slot slot = ref slots[id];
            if (slot.Instance == null && slot.Factory == null) return false;
            if (expected != null && !ReferenceEquals(slot.Instance, expected)) return false;

            slot = default;
            return true;
        }

        #endregion
    }
}