using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace AstekUtility.SceneManagement
{
	public class Bootstrapper : Singleton<Bootstrapper>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static async void Init()
		{
			Debug.Log("Bootstrapper........");
			if (SceneManager.GetActiveScene().name != "GameManager")
				await Addressables.LoadSceneAsync("Assets/Scenes/GameManager.unity", LoadSceneMode.Single).Task;
		}
	}
}