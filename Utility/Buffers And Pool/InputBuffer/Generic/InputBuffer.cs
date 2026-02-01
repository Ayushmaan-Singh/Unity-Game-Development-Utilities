using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Astek.InputBuffer.Generic
{
	public interface IInputBuffer
	{
		void Update();
	}

	public class InputBuffer<T> : IInputBuffer, IDisposable
	{
		public int BufferSize { get; private set; } = 5;
		private T[] inputKeys;
		private readonly Dictionary<T, InputBufferItem<T>> _buffer = new Dictionary<T, InputBufferItem<T>>();

		private readonly HashSet<T> _rawBuffer = new HashSet<T>();
		public Dictionary<T, InputBufferItem<T>> Buffer => _buffer;
		public InputBufferItem<T> this[T key] => _buffer[key];

		~InputBuffer()
		{
			Dispose(false);
		}

		public InputBuffer(params T[] inputKeys)
		{
			InputBufferManager.RegisterInputBuffer(this);
			inputKeys.ForEach(inputKey =>
				_buffer.Add(inputKey, new InputBufferItem<T>(inputKey, BufferSize)));
		}
		public InputBuffer(int bufferSize, params T[] inputKeys)
		{
			InputBufferManager.RegisterInputBuffer(this);
			BufferSize = bufferSize;
			inputKeys.ForEach(inputKey =>
				_buffer.Add(inputKey, new InputBufferItem<T>(inputKey, BufferSize)));
		}

		public void BufferRawInput(T key)
		{
			_rawBuffer.Add(key);
		}

		public void Update()
		{
			_buffer.Values.ForEach(inputBufferItem =>
			{
				//We do -1 because the last index is set by resolvecommand
				for (int i = 0; i < BufferSize - 1; i++)
				{
					inputBufferItem.Buffer[i].Hold = inputBufferItem.Buffer[i + 1].Hold;
					inputBufferItem.Buffer[i].Used = inputBufferItem.Buffer[i + 1].Used;
				}
				inputBufferItem.ResolveCommand(_rawBuffer.Contains(inputBufferItem.KeyInput));
				_rawBuffer.Remove(inputBufferItem.KeyInput);
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
				InputBufferManager.DeregisterInputBuffer(this);

			_disposed = true;
		}

		#endregion


		#if UNITY_EDITOR

		public void PrintBuffer()
		{
			int xSpace = 20;
			int ySpace = 25;
			int index = 0;
			Buffer.Values.ForEach(inputStateItem =>
			{
				GUI.Label(new Rect(xSpace, index * ySpace, 100 * 2, 20 * 2), inputStateItem.KeyInput + ":");
				for (int j = 0; j < BufferSize; j++)
				{
					if (inputStateItem.Buffer[j].Used)
					{
						GUI.Label(new Rect(j * xSpace + 100 * 2, index * ySpace, 100 * 2, 20 * 2), $"{inputStateItem.Buffer[j].Hold}*");
					}
					else
					{
						GUI.Label(new Rect(j * xSpace + 100 * 2, index * ySpace, 100 * 2, 20 * 2), $"{inputStateItem.Buffer[j].Hold}");
					}
				}
				index++;
			});
		}

		#endif
	}

	public class InputBufferItem<T>
	{
		//embed this in player loop
		public T KeyInput { get; private set; }
		public List<InputStateItem> Buffer { get; private set; }
		public InputStateItem this[int index]
		{
			get => Buffer[index];
			set => Buffer[index] = value;
		}

		public InputBufferItem(T keyInput, int bufferSize)
		{
			KeyInput = keyInput;
			Buffer = new List<InputStateItem>(bufferSize);
			for (int i = 0; i < bufferSize; i++)
			{
				Buffer.Add(new InputStateItem(i));
			}
		}

		public void ResolveCommand(bool pressed = false)
		{
			if (pressed)
			{
				Buffer[^1].HoldUp();
			}
			else
			{
				Buffer[^1].ReleaseHold();
			}
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
		public int StateItemIndexInBuffer { get; private set; } = -1;

		public InputStateItem(int stateItemIndexInBuffer)
		{
			StateItemIndexInBuffer = stateItemIndexInBuffer;
		}

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