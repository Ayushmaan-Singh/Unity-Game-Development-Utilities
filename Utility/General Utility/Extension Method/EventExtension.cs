using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace AstekUtility
{
	public static class EventExtension
	{
		#region AllTrue And Variation

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

		#region Validation and Cleaning
		
		/// <summary>
		/// Removes delegates with destroyed Unity object targets or invalid references.
		/// Usage: myEvent = myEvent.CleanDelegates();
		/// </summary>
		public static T CleanDelegates<T>(this T source) where T : Delegate
		{
			if (source == null) return null;

			List<Delegate> validDelegates = new List<Delegate>();
			Delegate[] invocationList = source.GetInvocationList();

			Parallel.ForEach(invocationList, del =>
			{
				if (IsDelegateValid(del))
				{
					validDelegates.Add(del);
				}
			});
			
			// Rebuild the delegate from valid entries
			return Delegate.Combine(validDelegates.ToArray()) as T;
		}

		// Improved delegate validation
		private static bool IsDelegateValid(Delegate del)
		{
			if (del == null) return false;

			// Check for Unity object targets (explicit destroyed check)
			if (del.Target is UnityEngine.Object unityTarget)
			{
				// Use Unity's explicit null check for destroyed objects
				return !ReferenceEquals(unityTarget, null) && unityTarget != null;
			}

			// For non-Unity targets or static methods, assume valid
			return true;
		}

		#endregion
	}
}