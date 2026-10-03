using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Astek.DesignPattern.ServiceLocatorTool
{
    public enum ServiceScope
    {
        Local,
        Scene,
        Global
    }

    /// <summary>
    /// Hierarchical service locator. Resolution order: this -> nearest parent locator -> scene locator -> global.
    /// Main-thread only.
    /// </summary>
    public sealed class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator _global;
        // Value null = "scene scanned, no scene locator" (negative cache). Cleared on scene unload.
        static Dictionary<Scene, ServiceLocator> _sceneContainers = new Dictionary<Scene, ServiceLocator>();
        static List<GameObject> _tmpSceneGameObjects = new List<GameObject>();
        // Bumped on any topology change (configure/destroy/enable/disable/reparent/scene unload). Per-instance caches compare against it.
        static int _version;
        static bool _quitting;

        readonly ServiceManager _services = new ServiceManager();

        ServiceLocator _next;
        int _nextVersion = -1;
        Scene _configuredScene;
        bool _isSceneContainer;

        const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

        // Only affects console log context (click-to-ping); registration before Awake still works, just without context.
        void Awake() => _services!.Owner = this;
        void Reset() => _services!.Owner = this;

        /// <summary>Local, Scene or Global. Local until configured otherwise by a bootstrapper.</summary>
        public ServiceScope Scope => ReferenceEquals(this, _global) ? ServiceScope.Global
            : _isSceneContainer ? ServiceScope.Scene
            : ServiceScope.Local;

        /// <summary>Call after reparenting an ancestor chain this locator depends on, if the automatic invalidation missed it.</summary>
        public static void InvalidateCaches() => _version++;

        #region Configure as Scene or Global

        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (ReferenceEquals(_global, this))
            {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            }
            else if (_global != null)
            {
                Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
            }
            else
            {
                _global = this;
                _version++;

                if (dontDestroyOnLoad)
                {
                    // DontDestroyOnLoad only works on roots.
                    if (transform.parent != null) transform.SetParent(null, true);
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        internal void ConfigureForScene()
        {
            Scene scene = gameObject.scene;

            // Existing entry only blocks when it is a live locator; null/destroyed entries are stale or negative cache.
            if (_sceneContainers.TryGetValue(scene, out ServiceLocator existing) && existing != null)
            {
                Debug.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
                return;
            }

            _sceneContainers[scene] = this;
            _configuredScene        = scene;
            _isSceneContainer       = true;
            _version++;
        }

        #endregion

        /// <summary>
        /// Gets the global ServiceLocator instance. Creates new if none exists (play mode only).
        /// Returns null while the application is quitting.
        /// </summary>
        public static ServiceLocator Global
        {
            get
            {
                if (_global != null) return _global;
                if (_quitting) return null;

                if (FindAnyObjectByType<ServiceLocatorGlobal>(FindObjectsInactive.Include) is { } found)
                {
                    found.BootstrapOnDemand();
                    return _global != null ? _global : null;
                }

                if (Application.isPlaying)
                {
                    GameObject container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                    container.AddComponent<ServiceLocatorGlobal>()!.BootstrapOnDemand();
                }

                return _global != null ? _global : null;
            }
        }

        /// <summary>
        /// Returns the <see cref="ServiceLocator"/> configured for the scene of a Component. Falls back to the global instance.
        /// If the component is itself the scene locator, returns the global instance.
        /// </summary>
        public static ServiceLocator ForSceneOf(Component cmt)
        {
            if (cmt == null) throw new ArgumentNullException(nameof(cmt));

            ServiceLocator sceneLocator = FindSceneLocator(cmt.gameObject.scene);
            if (sceneLocator != null && sceneLocator != cmt) return sceneLocator;

            return Global;
        }

        /// <inheritdoc cref="ForSceneOf(Component)"/>
        public static ServiceLocator ForSceneOf(MonoBehaviour mb) => ForSceneOf((Component)mb);

        /// <summary>
        /// Gets the closest ServiceLocator to the provided Component in hierarchy, the ServiceLocator for its scene, or the global ServiceLocator.
        /// </summary>
        public static ServiceLocator For(Component cmt)
        {
            if (cmt == null) throw new ArgumentNullException(nameof(cmt));

            ServiceLocator local = cmt.GetComponentInParent<ServiceLocator>();
            return local != null ? local : ForSceneOf(cmt);
        }

        /// <inheritdoc cref="For(Component)"/>
        public static ServiceLocator For(MonoBehaviour mb) => For((Component)mb);

        static ServiceLocator FindSceneLocator(Scene scene)
        {
            if (_sceneContainers.TryGetValue(scene, out ServiceLocator cached))
                return cached != null ? cached : null;

            if (!scene.IsValid()) return null;

            // Cold path, once per scene. Covers a scene locator whose Awake has not run yet.
            // Shared list is released before bootstrapping: Bootstrap() overrides may re-enter.
            _tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(_tmpSceneGameObjects);

            ServiceLocatorScene bootstrapper = null;
            for (int i = 0; i < _tmpSceneGameObjects.Count; i++)
            {
                bootstrapper = _tmpSceneGameObjects[i].GetComponentInChildren<ServiceLocatorScene>(true);
                if (bootstrapper != null) break;
            }
            _tmpSceneGameObjects.Clear();

            if (bootstrapper != null) bootstrapper.BootstrapOnDemand(); // ConfigureForScene fills the cache

            if (_sceneContainers.TryGetValue(scene, out ServiceLocator found) && found != null) return found;

            _sceneContainers[scene] = null; // negative cache until a scene locator configures itself
            return null;
        }

        #region Registration

        /// <summary>Registers a service using typeof(T). Duplicate logs an error and keeps the existing one.</summary>
        public ServiceLocator Register<T>(T service)
        {
            _services.Register(service);
            return this;
        }

        /// <summary>Registers a service using a specific type.</summary>
        public ServiceLocator Register(Type type, object service)
        {
            _services.Register(type, service);
            return this;
        }

        /// <summary>Registers silently; returns false if already registered at this level.</summary>
        public bool TryRegister<T>(T service) => _services.TryRegister(service);

        /// <summary>Registers, replacing any existing registration at this level.</summary>
        public ServiceLocator Replace<T>(T service)
        {
            _services.Replace(service);
            return this;
        }

        /// <summary>Registers a factory invoked on first resolve; the result is cached.</summary>
        public ServiceLocator RegisterLazy<T>(Func<T> factory) where T : class
        {
            _services.RegisterLazy(factory);
            return this;
        }

        /// <summary>Removes whatever is registered under T at this level.</summary>
        public bool Unregister<T>() => _services.Unregister<T>();

        /// <summary>Removes the registration under T at this level only if it is this exact instance.</summary>
        public bool Unregister<T>(T service) => _services.Unregister(service);

        public bool Unregister(Type type) => _services.Unregister(type);

        /// <summary>Removes the instance from every type it is registered under at this level.</summary>
        public int UnregisterInstance(object service) => _services.UnregisterInstance(service);

        /// <summary>True if T is registered at this level (or, with inherit, anywhere up the chain). Never instantiates lazy services.</summary>
        public bool IsRegistered<T>(bool inherit = false)
        {
            ServiceLocator current = this;
            do
            {
                if (current._services.IsRegistered<T>()) return true;
            } while (inherit && current.TryGetNextInHierarchy(out current));

            return false;
        }

        #endregion

        #region Resolution

        /// <summary>Retrieves a service, walking up the chain. Throws <see cref="ServiceNotFoundException"/> if absent.</summary>
        public T Get<T>() where T : class
        {
            ServiceLocator current = this;
            do
            {
                if (current._services.TryGet(out T service)) return service;
            } while (current.TryGetNextInHierarchy(out current));

            throw new ServiceNotFoundException(typeof(T),
                $"ServiceLocator.Get: Could not resolve type '{typeof(T).FullName}'. Searched: {DescribeChain()}");
        }

        /// <summary>Tries to get a service, walking up the chain.</summary>
        public bool TryGet<T>(out T service) where T : class
        {
            ServiceLocator current = this;
            do
            {
                if (current._services.TryGet(out service)) return true;
            } while (current.TryGetNextInHierarchy(out current));

            service = null;
            return false;
        }

        // Cached: one int compare on the hot path instead of GetComponentInParent + scene scan per miss.
        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (ReferenceEquals(this, _global))
            {
                container = null;
                return false;
            }

            if (_nextVersion != _version)
            {
                _next        = ResolveNext();
                _nextVersion = _version;
            }

            container = _next;
            return (object)container != null;
        }

        ServiceLocator ResolveNext()
        {
            Transform parent = transform.parent;
            ServiceLocator next = parent != null ? parent.GetComponentInParent<ServiceLocator>() : null;

            if (next == null) next = ForSceneOf(this);

            return next != null && next != this ? next : null;
        }

        string DescribeChain()
        {
            StringBuilder sb = new StringBuilder();
            ServiceLocator current = this;
            int guard = 0;

            do
            {
                if (sb.Length > 0) sb.Append(" -> ");
                sb.Append(current.name).Append(" (").Append(current.Scope).Append(')');
            } while (guard++ < 32 && current.TryGetNextInHierarchy(out current));

            return sb.ToString();
        }

        #endregion

        #region Lifecycle / topology invalidation

        void OnEnable() => _version++;
        void OnDisable() => _version++;
        void OnTransformParentChanged() => _version++;

        private void OnDestroy()
        {
            if (ReferenceEquals(_global, this))
            {
                _global = null;
            }

            if (_isSceneContainer
                && _sceneContainers.TryGetValue(_configuredScene, out ServiceLocator registered)
                && ReferenceEquals(registered, this))
            {
                _sceneContainers.Remove(_configuredScene);
            }

            _services.Clear();
            _version++;
        }

        static void OnSceneUnloaded(Scene scene)
        {
            _sceneContainers.Remove(scene);
            _version++;
        }

        static void OnQuitting() => _quitting = true;

        // https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _global              = null;
            _quitting            = false;
            _sceneContainers     = new Dictionary<Scene, ServiceLocator>();
            _tmpSceneGameObjects = new List<GameObject>();
            _version++;

            // -= first: with domain reload disabled this runs every play session.
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Application.quitting       -= OnQuitting;
            Application.quitting       += OnQuitting;
        }

        #endregion

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal() => CreateInEditor<ServiceLocatorGlobal>(k_globalServiceLocatorName);

        [MenuItem("GameObject/ServiceLocator/Add Global", true)]
        static bool ValidateAddGlobal() => FindFirstObjectByType<ServiceLocatorGlobal>(FindObjectsInactive.Include) == null;

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene() => CreateInEditor<ServiceLocatorScene>(k_sceneServiceLocatorName);

        [MenuItem("GameObject/ServiceLocator/Add Scene", true)]
        static bool ValidateAddScene() => FindFirstObjectByType<ServiceLocatorScene>(FindObjectsInactive.Include) == null;

        static void CreateInEditor<T>(string objectName) where T : Component
        {
            GameObject go = new GameObject(objectName, typeof(T));
            Undo.RegisterCreatedObjectUndo(go, "Create " + objectName);
            Selection.activeGameObject = go;
        }
#endif
    }
}