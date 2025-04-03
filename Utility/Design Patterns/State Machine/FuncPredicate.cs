using System;
namespace AstekUtility.DesignPattern.StateMachine
{
	public interface IPredicate
	{
		bool Evaluate();
	}

	/// <summary>
	///     Evaluates given condition to identify if we can change state
	/// </summary>
	public class FuncPredicate : IPredicate
	{
		private readonly Func<bool> _func;

		public FuncPredicate(Func<bool> func)
		{
			_func = func;
		}

		public bool Evaluate() => _func.Invoke();

	}
}