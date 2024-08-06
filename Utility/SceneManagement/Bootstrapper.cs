using UnityEngine;
using UnityEngine.SceneManagement;

namespace AstekUtility.SceneManagement
{
	public class Bootstrapper : PersistentSingleton<Bootstrapper>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static async void Init()
		{
			Debug.Log("Bootstrapper........");
			await SceneManager.LoadSceneAsync("GameMaster", LoadSceneMode.Single).AsTask();
		}
	}
}