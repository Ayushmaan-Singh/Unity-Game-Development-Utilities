#if NEW_INPUT_SYSTEM_INPUT_BUFFER
using System;
using System.Collections.Generic;

namespace AstekUtility.InputBuffer.NewInputSystem
{
	public class InputBufferManager
	{
		private static readonly HashSet<InputBuffer> _inputBuffers = new HashSet<InputBuffer>();
		
		public static void RegisterInputBuffer(InputBuffer buffer) => _inputBuffers.Add(buffer);
		public static void DeregisterInputBuffer(InputBuffer buffer) => _inputBuffers.Remove(buffer);
		
		public static void UpdateBuffers()
		{
			foreach (InputBuffer inputBuffer in new List<InputBuffer>(_inputBuffers))
			{
				inputBuffer.Update();
			}
		}
		
		public static void Clear() => _inputBuffers.Clear();
	}
}
#endif