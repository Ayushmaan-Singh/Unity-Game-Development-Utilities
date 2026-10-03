using UnityEngine;

namespace Astek.DesignPattern.ServiceLocatorTool
{
    /// <summary>
    /// Base for ServiceLocatorGlobal / ServiceLocatorScene. Derive from either and override Bootstrap()
    /// (call base.Bootstrap() first) to register services at startup.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    [DefaultExecutionOrder(-10000)] // locators must exist before any gameplay Awake() looks them up
    public abstract class Bootstrapper : MonoBehaviour
    {
        private ServiceLocator _container;
        protected internal ServiceLocator Container => _container != null ? _container : (_container = GetComponent<ServiceLocator>());
 
        bool _hasBeenBootstrapped;
 
        void Awake() => BootstrapOnDemand();
 
        public void BootstrapOnDemand()
        {
            if (_hasBeenBootstrapped) return;
            _hasBeenBootstrapped = true;
            Bootstrap();
        }
 
        protected abstract void Bootstrap();
    }
}