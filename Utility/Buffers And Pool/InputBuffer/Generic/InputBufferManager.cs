using System;
using System.Collections.Generic;

namespace AstekUtility.InputBuffer.Generic
{
	public static class InputBufferManager
	{
		private static readonly List<IInputBuffer> _inputBuffers = new List<IInputBuffer>();
		
		public static void RegisterInputBuffer<T>(InputBuffer<T> buffer) => _inputBuffers.Add(buffer);
		public static void DeregisterInputBuffer<T>(InputBuffer<T> buffer) => _inputBuffers.Remove(buffer);
		
		public static void UpdateBuffers()
		{
			foreach (IInputBuffer inputBuffer in new List<IInputBuffer>(_inputBuffers))
			{
				inputBuffer.Update();
			}
		}
		
		public static void Clear() => _inputBuffers.Clear();
	}
}