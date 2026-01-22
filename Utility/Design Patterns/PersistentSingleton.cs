using UnityEngine;

namespace AstekUtility
{
	public class PersistentSingleton<T> : MonoBehaviour where T : Component
	{
		public bool AutoUnparentOnAwake = true;

		protected static T instance;
		private static readonly object _lock = new object();

		public static bool HasInstance => instance != null;
		public static T TryGetInstance() => HasInstance ? instance : null;

		private static bool _isQuitting;

		public static T Instance
		{
			get
			{
				// If the application is quitting, do not create a new instance
				if (_isQuitting)
				{
					Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
					return null;
				}

				lock (_lock)
				{
					if (instance == null)
					{
						instance = (T)FindObjectOfType(typeof(T));

						if (instance == null)
						{
							GameObject singleton = new GameObject();
							instance = singleton.AddComponent<T>();
							singleton.name = "(singleton) " + typeof(T);
						}
					}

					return instance;
				}
			}
		}

		/// <summary>
		/// Make sure to call base.Awake() in override if you need awake.
		/// </summary>
		protected virtual void Awake()
		{
			InitializeSingleton();
		}

		protected virtual void InitializeSingleton()
		{
			if (!Application.isPlaying) return;

			if (AutoUnparentOnAwake)
			{
				transform.SetParent(null);
			}

			if (instance == null)
			{
				instance = this as T;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				if (instance != this)
				{
					Destroy(gameObject);
				}
			}
		}

		private static void ApplicationStatus()
		{
			_isQuitting = false;
			Application.quitting -= IsQuitting;
			Application.quitting += IsQuitting;

			void IsQuitting() => _isQuitting = true;
		}
	}
}