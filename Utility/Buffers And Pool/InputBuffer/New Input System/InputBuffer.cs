#if NEW_INPUT_SYSTEM_INPUT_BUFFER
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AstekUtility.InputBuffer.NewInputSystem
{
	public class InputBuffer : IDisposable
	{
		public int BufferSize { get; private set; } = 5;
		private Key[] inputKeys;
		public readonly Dictionary<Key, InputBufferItem> _buffer = new Dictionary<Key, InputBufferItem>();

		~InputBuffer()
		{
			Dispose(false);
		}

		public InputBuffer(params Key[] inputKeys)
		{
			InputBufferManager.RegisterInputBuffer(this);
			inputKeys.ForEach(inputKey =>
				_buffer.Add(inputKey, new InputBufferItem(inputKey, BufferSize)));
		}
		public InputBuffer(int bufferSize, params Key[] inputKeys)
		{
			InputBufferManager.RegisterInputBuffer(this);
			BufferSize = bufferSize;
			inputKeys.ForEach(inputKey =>
				_buffer.Add(inputKey, new InputBufferItem(inputKey, BufferSize)));
		}

		public void Update()
		{
			_buffer.ForEach(inputBufferItem =>
			{
				inputBufferItem.Value.ResolveCommand();
				//We do -1 because we already set the last one during ResolveCommand function
				for (int i = 0; i < BufferSize - 1; i++)
				{
					inputBufferItem.Value.Buffer[i].Hold = inputBufferItem.Value.Buffer[i + 1].Hold;
					inputBufferItem.Value.Buffer[i].Used = inputBufferItem.Value.Buffer[i + 1].Used;
				}
			});
		}

		#region Desposable

		private bool _disposed = false;
		public void Dispose()
		{
			Dispose(true);
			//Tells GC that no need to call the finalizer since all resources have been freed already
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				InputBufferManager.DeregisterInputBuffer(this);
				_buffer.Clear();
			}

			_disposed = true;
		}

		#endregion

		#if UNITY_EDITOR

		public void PrintBuffer()
		{
			int xSpace = 20;
			int ySpace = 25;

			int i = 0;
			_buffer.ForEach(inputBuffer =>
			{
				GUI.Label(new Rect(xSpace, i * ySpace, 100, 20), inputBuffer.Value.KeyInput + ": ");
				for (int j = 0; j < inputBuffer.Value.Buffer.Count; j++)
				{
					if (inputBuffer.Value.Buffer[j].Used)
						GUI.Label(new Rect(j * xSpace + 100, i * ySpace, 100, 20), inputBuffer.Value.Buffer[j].Hold + "*");
					else
						GUI.Label(new Rect(j * xSpace + 100, i * ySpace, 100, 20), inputBuffer.Value.Buffer[j].Hold.ToString());
				}
				i++;
			});
		}

		#endif
	}

	public class InputBufferItem
	{
		//embed this in player loop
		public Key KeyInput { get; private set; }
		public List<InputStateItem> Buffer { get; private set; }

		public InputBufferItem(Key keyInput, int bufferSize)
		{
			KeyInput = keyInput;
			Buffer = new List<InputStateItem>(bufferSize);
			for (int i = 0; i < bufferSize; i++)
			{
				Buffer.Add(new InputStateItem());
			}
		}

		public void ResolveCommand()
		{
			if (Keyboard.current[KeyInput].isPressed)
			{
				Buffer[^1].HoldUp();
			}
			else
			{
				Buffer[^1].ReleaseHold();
			}
		}

		/// <summary>
		/// Hold=0 i.e. no press or release
		/// Hold>=1 i.e. pressed
		/// Hold less than or equal to -1 i.e. released
		/// </summary>
		public class InputStateItem
		{
			public int Hold = 0;
			public bool Used = false;

			public bool CanExecute()
			{
				if (Hold == 1 && !Used)
					return true;
				else
					return false;
			}

			public void HoldUp()
			{
				if (Hold < 0)
					Hold = 1;
				else
					Hold += 1;
			}

			public void ReleaseHold()
			{
				if (Hold > 0)
				{
					Hold = -1;
					Used = false;
				}
				else
					Hold = 0;
			}
		}
	}
}
#endif