#if NEW_INPUT_SYSTEM_INPUT_BUFFER

using AstekUtility.Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace AstekUtility.InputBuffer.NewInputSystem
{
	public static class InputBufferBootstrapper
	{
		private static PlayerLoopSystem _INPUTBUFFERSYSTEM;

		#if UNITY_EDITOR
		private static bool SHOWPLAYERLOOP = true;
        #endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		internal static void Initialize()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

			if (!InsertInputBufferManager<Update>(ref currentPlayerLoop,0))
			{
				Debug.LogWarning("Improved Input Buffer not initialized, unable to register InputBufferManager into the update loop");
				return;
			}
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
			
			#if UNITY_EDITOR
			
			if (SHOWPLAYERLOOP)
				PlayerLoopUtils.PrintPlayerLoop(currentPlayerLoop);

			EditorApplication.playModeStateChanged -= OnPlayModeState;
			EditorApplication.playModeStateChanged += OnPlayModeState;
			
			static void OnPlayModeState(PlayModeStateChange state)
			{
				if (state == PlayModeStateChange.ExitingPlayMode)
				{
					PlayerLoopSystem currentPlayerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
					RemoveInputBufferManager<Update>(ref currentPlayerLoopSystem);
					PlayerLoop.SetPlayerLoop(currentPlayerLoopSystem);
					InputBufferManager.Clear();
				}
			}
			#endif
		}
		
		public static void RemoveInputBufferManager<T>(ref PlayerLoopSystem loop)
		{
			PlayerLoopUtils.RemoveSystem<T>(ref loop, in _INPUTBUFFERSYSTEM);
		}

		public static bool InsertInputBufferManager<T>(ref PlayerLoopSystem loop, int index)
		{
			_INPUTBUFFERSYSTEM = new PlayerLoopSystem()
			{
				type = typeof(InputBufferManager),
				updateDelegate = InputBufferManager.UpdateBuffers,
				subSystemList = null
			};

			return PlayerLoopUtils.InsertSystem<T>(ref loop, in _INPUTBUFFERSYSTEM, index);
		}
	}
}

#endif