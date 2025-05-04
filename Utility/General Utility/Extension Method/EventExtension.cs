using System;
namespace AstekUtility
{
	public static class EventExtension
	{
		#region AllTrue And Variatiom

		public static bool AllTrue(this Func<bool> source)
		{
			bool result = false;
			source.GetInvocationList()?.Select(del => del as Func<bool>).ForEach(func => result &= func());
			return result;
		}
		public static bool AllTrue<T>(this Func<T, bool> source, T args)
		{
			bool result = true;
			source.GetInvocationList()?.Select(del => del as Func<T, bool>).ForEach(func => result &= func(args));
			return result;
		}
		public static bool AllTrue<T1, T2>(this Func<T1, T2, bool> source, T1 args1, T2 args2)
		{
			bool result = false;
			source.GetInvocationList()?.Select(del => del as Func<T1, T2, bool>).ForEach(func => result &= func(args1, args2));
			return result;
		}

		#endregion
	
	}
}