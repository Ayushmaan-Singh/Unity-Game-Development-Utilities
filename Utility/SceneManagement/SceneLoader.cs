using System;
using System.Threading.Tasks;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AstekUtility.SceneManagement
{
	public class SceneLoader : MonoBehaviour
	{
		[SerializeField, ToggleLeft, FoldoutGroup("Loading UI")] private bool useLoadingUI = false;
		[SerializeField, FoldoutGroup("Loading UI"), ShowIf("@useLoadingUI")] private Image loadingBar;
		[SerializeField, FoldoutGroup("Loading UI"), ShowIf("@useLoadingUI")] private float fillSpeed = 0.5f;
		[SerializeField, FoldoutGroup("Loading UI"), ShowIf("@useLoadingUI")] private Canvas loadingCanvas;
		[SerializeField, FoldoutGroup("Loading UI"), ShowIf("@useLoadingUI")] private Camera loadingCamera;
		[SerializeField] private SceneGroup[] sceneGroups;

		private float targetProgress;
		private bool isLoading;

		public readonly SceneGroupManager manager = new SceneGroupManager();

		private void Update()
		{
			if (!isLoading || !useLoadingUI)
				return;

			float currentFillAmount = loadingBar.fillAmount;
			float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

			float dynamicFillSpeed = progressDifference * fillSpeed;

			loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
		}

		/// <summary>
		/// Modify if we require a whole another scene for loading
		/// </summary>
		/// <param name="index"></param>
		public async Task LoadSceneGroup(int index)
		{
			if (useLoadingUI)
				loadingBar.fillAmount = 0;
			targetProgress = 1f;

			if (index < 0 || index >= sceneGroups.Length)
			{
				Debug.LogError($"Invalid Scene Group Index: {index}");
				return;
			}

			LoadingProgress progress = new LoadingProgress();
			progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

			EnableLoadingCanvas();
			await manager.LoadScenes(sceneGroups[index], progress);
			EnableLoadingCanvas(false);
		}

		private void EnableLoadingCanvas(bool enable = true)
		{
			if (!useLoadingUI)
				return;

			isLoading = enable;
			loadingCanvas.gameObject.SetActive(enable);
			loadingCamera.gameObject.SetActive(enable);
		}
	}

	public class LoadingProgress : IProgress<float>
	{
		public event Action<float> Progressed;

		private const float ratio = 1f;

		public void Report(float value)
		{
			Progressed?.Invoke(value / ratio);
		}
	}
}