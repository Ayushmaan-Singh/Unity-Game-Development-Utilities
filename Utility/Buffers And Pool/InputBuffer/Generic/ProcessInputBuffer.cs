using System;
using System.Collections.Generic;
using System.Linq;
using AstekUtility;

namespace AstekUtility.InputBuffer.Generic
{
	public static class ProcessInputBuffer
	{
		#region Process InputBuffer

		public static (List<T> keyInputs, List<InputStateItem> stateItems) EarliestPressProcessesAmongAllBuffer<T>(this InputBuffer<T> bufferCollection, uint presspoint = 3)
		{
			List<T> keyInputs = new List<T>();
			List<InputStateItem> stateItems = new List<InputStateItem>();

			//-100 means it is uninitialized
			int earliestIndex = -100;

			bufferCollection.Buffer.Values.ForEach(inputBufferItem =>
			{
				InputStateItem earliestPressProcess = inputBufferItem.OldestPressProcess(presspoint);
				if (earliestIndex == -100)
				{
					if (earliestPressProcess != null)
					{
						keyInputs.Add(inputBufferItem.KeyInput);
						stateItems.Add(earliestPressProcess);
						earliestIndex = earliestPressProcess.StateItemIndexInBuffer;
					}
				}
				else
				{
					if (earliestPressProcess.StateItemIndexInBuffer < earliestIndex)
					{
						keyInputs = new List<T>()
						{
							inputBufferItem.KeyInput
						};
						stateItems = new List<InputStateItem>()
						{
							earliestPressProcess
						};
						earliestIndex = earliestPressProcess.StateItemIndexInBuffer;
					}
					else if (earliestPressProcess.StateItemIndexInBuffer == earliestIndex)
					{
						keyInputs.Add(inputBufferItem.KeyInput);
						stateItems.Add(earliestPressProcess);
					}
				}
			});
			return (keyInputs, stateItems);
		}

		public static (List<T> keyInputs, List<InputStateItem> stateItems) EarliestHoldProcessesAmongAllBuffer<T>(this InputBuffer<T> bufferCollection, uint presspoint = 3)
		{
			List<T> keyInputs = new List<T>();
			List<InputStateItem> stateItems = new List<InputStateItem>();

			//-100 means it is uninitialized
			int earliestIndex = -100;

			bufferCollection.Buffer.Values.ForEach(inputBufferItem =>
			{
				InputStateItem earliestHoldProcess = inputBufferItem.OldestHoldProcess(presspoint);
				if (earliestIndex == -100)
				{
					if (earliestHoldProcess != null)
					{
						keyInputs.Add(inputBufferItem.KeyInput);
						stateItems.Add(earliestHoldProcess);
						earliestIndex = earliestHoldProcess.StateItemIndexInBuffer;
					}
				}
				else
				{
					if (earliestHoldProcess.StateItemIndexInBuffer < earliestIndex)
					{
						keyInputs = new List<T>()
						{
							inputBufferItem.KeyInput
						};
						stateItems = new List<InputStateItem>()
						{
							earliestHoldProcess
						};
						earliestIndex = earliestHoldProcess.StateItemIndexInBuffer;
					}
					else if (earliestHoldProcess.StateItemIndexInBuffer == earliestIndex)
					{
						keyInputs.Add(inputBufferItem.KeyInput);
						stateItems.Add(earliestHoldProcess);
					}
				}
			});
			return (keyInputs, stateItems);
		}

		public static (List<T> keyInputs, List<InputStateItem> stateItems) EarliestReleaseProcessesAmongAllBuffer<T>(this InputBuffer<T> bufferCollection)
		{
			List<T> keyInputs = new List<T>();
			List<InputStateItem> stateItems = new List<InputStateItem>();

			//-100 means it is uninitialized
			int earliestIndex = -100;

			bufferCollection.Buffer.Values.ForEach(inputBufferItem =>
			{
				InputStateItem earliestReleaseProcess = inputBufferItem.OldestReleaseProcess();
				if (earliestIndex == -100)
				{
					if (earliestReleaseProcess != null)
					{
						keyInputs.Add(inputBufferItem.KeyInput);
						stateItems.Add(earliestReleaseProcess);
						earliestIndex = earliestReleaseProcess.StateItemIndexInBuffer;
					}
				}
				else
				{
					if (earliestReleaseProcess.StateItemIndexInBuffer < earliestIndex)
					{
						keyInputs = new List<T>()
						{
							inputBufferItem.KeyInput
						};
						stateItems = new List<InputStateItem>()
						{
							earliestReleaseProcess
						};
						earliestIndex = earliestReleaseProcess.StateItemIndexInBuffer;
					}
					else if (earliestReleaseProcess.StateItemIndexInBuffer == earliestIndex)
					{
						keyInputs.Add(inputBufferItem.KeyInput);
						stateItems.Add(earliestReleaseProcess);
					}
				}
			});
			return (keyInputs, stateItems);
		}

		#endregion

		#region Process InputBufferItem

		//Oldest i.e oldest to newest buffer check
		public static InputStateItem OldestPressProcess<T>(this InputBufferItem<T> keybuffer, uint presspoint = 3)
		{
			return keybuffer.Buffer.FirstOrDefault(inputStateItem => !inputStateItem.Used && inputStateItem.Hold > 0 && inputStateItem.Hold <= presspoint);
		}
		public static InputStateItem OldestHoldProcess<T>(this InputBufferItem<T> keybuffer, uint presspoint = 3)
		{
			return keybuffer.Buffer.FirstOrDefault(inputStateItem => !inputStateItem.Used && inputStateItem.Hold > presspoint);
		}
		public static InputStateItem OldestReleaseProcess<T>(this InputBufferItem<T> keybuffer)
		{
			return keybuffer.Buffer.FirstOrDefault(inputStateItem => !inputStateItem.Used && inputStateItem.Hold == -1);
		}

		//Latest i.e newest to oldest buffer check
		public static InputStateItem LatestPressProcess<T>(this InputBufferItem<T> keybuffer, uint presspoint = 3)
		{
			return keybuffer.Buffer.LastOrDefault(inputStateItem => !inputStateItem.Used && inputStateItem.Hold > 0 && inputStateItem.Hold <= presspoint);
		}
		public static InputStateItem LatestHoldProcess<T>(this InputBufferItem<T> keybuffer, uint presspoint = 3)
		{
			return keybuffer.Buffer.LastOrDefault(inputStateItem => !inputStateItem.Used && inputStateItem.Hold > presspoint);
		}
		public static InputStateItem LatestReleaseProcess<T>(this InputBufferItem<T> keybuffer)
		{
			return keybuffer.Buffer.LastOrDefault(inputStateItem => !inputStateItem.Used && inputStateItem.Hold == -1);
		}

		#endregion

		#region Process InputStateItem

		//Next state item if availale else return InputStateItem at index 0 so if index of state item is 0 then that means currentStateItem was element in buffer
		public static InputStateItem NextStateItem<T>(this InputBufferItem<T> buffer, InputStateItem currentStateItem)
		{
			return buffer.Buffer[currentStateItem.StateItemIndexInBuffer + 1 < buffer.Buffer.Capacity ? currentStateItem.StateItemIndexInBuffer + 1 : currentStateItem.StateItemIndexInBuffer];
		}

		#endregion

		public static void ConsumeInput<T>(this InputBufferItem<T> keyBuffer, InputStateItem stateItem)
		{
			if (stateItem == null)
				throw new ArgumentNullException("InputStateItem");
			if (keyBuffer == null)
				throw new ArgumentNullException("InputBufferItem");

			for (int i = stateItem.StateItemIndexInBuffer; i >= 0 && !keyBuffer.Buffer[i].Used; i--)
				keyBuffer.Buffer[i].Used = true;
		}
	}
}