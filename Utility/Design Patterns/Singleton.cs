using UnityEngine;

namespace AstekUtility
{
    /// <summary>
    /// Generic singleton class providing basic functionalities of singleton
    /// Gets singleton if exsists, creates singleton if not
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;
        public static bool HasInstance => _instance != null;

        public static T TryGetInstance => HasInstance ? _instance : null;
        public static T Current => _instance;

        public static T Instance
        {
            get
            {
                if (!HasInstance)
                    _instance = FindFirstObjectByType<T>();
                if (!HasInstance)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name + "_Autocreated";
                    _instance = obj.AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
                return;
            _instance = this as T;
        }
    }
}
