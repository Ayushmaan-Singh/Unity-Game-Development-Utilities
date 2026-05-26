using UnityEngine;
namespace Astek
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		private static T _instance;
		public static bool HasInstance
		{
			get
			{
				return _instance != null;
			}
		}
		public static T TryGetInstance
		{
			get
			{
				return HasInstance ? _instance : null;
			}
		}
		public static T Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = FindAnyObjectByType<T>();
					if (!_instance)
					{
						GameObject obj = new GameObject
						{
							name = $"{typeof(T).Name}_AutoCreated"
						};
						_instance = obj.AddComponent<T>();
					}
				}

				return _instance;
			}
		}

		protected virtual void Awake()
		{
			InitializeSingleton();
		}

		private void InitializeSingleton()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			_instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
	}
}