using Sirenix.OdinInspector;
using UnityEngine;

namespace AstekUtility
{
	public class SerializedSingleton<T> : SerializedMonoBehaviour where T : Component
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
		public static T Current
		{
			get
			{
				return _instance;
			}
		}

		public static T Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = FindFirstObjectByType<T>();
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

		protected virtual void InitializeSingleton()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			_instance = this as T;
		}
	}
}