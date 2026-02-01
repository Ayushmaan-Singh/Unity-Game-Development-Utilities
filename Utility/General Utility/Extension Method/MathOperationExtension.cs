using System;
using System.Runtime.CompilerServices;
namespace Astek
{
	public static class MathOperationExtension
	{
		public enum MathOperation
		{
			Addition,
			Subtraction,
			PercentReduction,
			PercentIncrement
		}

		/// <summary>
		/// Perform operation on b and add to a
		/// </summary>
		/// <param name="a">value to which result of operation is affecting</param>
		/// <param name="operation">math operation to perform</param>
		/// <param name="b">value which is added to a after operation. In case of percent write in Percentage(i.e 60 means 60/100 not 0.6)</param>
		/// <returns></returns>
		/// <exception cref="SwitchExpressionException"></exception>
		public static float PerformOperation(this float a, MathOperation operation, float b)
		{
			switch (operation)
			{
				case MathOperation.Addition:
					return a + b;
				
				case MathOperation.Subtraction:
					return a - b;
				
				case MathOperation.PercentIncrement:
					return a + (a * (b / 100));
				
				case MathOperation.PercentReduction:
					return a - (a * (b / 100));
				
				default:
					throw new SwitchExpressionException($"Operation of type {operation} is not possible");
			}
		}
	}
}