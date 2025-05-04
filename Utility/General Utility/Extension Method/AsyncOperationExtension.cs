using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AstekUtility
{
	public static class AsyncOperationExtension
	{
		/// <summary>
		/// Extension method that converts an AsyncOperation into a Task.
		/// </summary>
		/// <returns>A Task that represents the completion of the AsyncOperation.</returns>
		public static Task AsTask(this AsyncOperation asyncOperation)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			asyncOperation.completed += _ => tcs.SetResult(true);
			return tcs.Task;
		}

		#region WaitWhile

		/// <summary>
		/// Return a task to await while a certain condition is true and checks every fixedupdate
		/// </summary>
		/// <param name="predicate"></param>
		/// <exception cref="ArgumentException"></exception>
		public static async Task WaitWhile_FixedUpdate(this Func<bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentException("Predicate is null");
			
			while (predicate.Invoke())
				await Awaitable.FixedUpdateAsync();
		}
		
		/// <summary>
		/// Return a task to await while a certain condition is true and check every update
		/// </summary>
		/// <param name="predicate"></param>
		/// <exception cref="ArgumentException"></exception>
		public static async Task WaitWhile_Update(this Func<bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentException("Predicate is null");
			
			while (predicate.Invoke())
				await Awaitable.NextFrameAsync();
		}
		
		/// <summary>
		/// Return a task to await while a certain condition is true and check every timeDelayInMilisec
		/// </summary>
		/// <param name="predicate"></param>
		/// <exception cref="ArgumentException"></exception>
		public static async Task WaitWhile(this Func<bool> predicate,int timeDelayInSec)
		{
			if (predicate == null)
				throw new ArgumentException("Predicate is null");
			
			while (predicate.Invoke())
				await Awaitable.WaitForSecondsAsync(timeDelayInSec);
		}

		#endregion

		#region WaitUntil

		/// <summary>
		/// Return a task to await while a certain condition is true and check every fixedupdate
		/// </summary>
		/// <param name="predicate"></param>
		/// <exception cref="ArgumentException"></exception>
		public static async Task WaitUntil_FixedUpdate(this Func<bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentException("Predicate is null");
			
			while (!predicate.Invoke())
				await Awaitable.FixedUpdateAsync();
		}
		
		/// <summary>
		/// Return a task to await while a certain condition is true and check every update
		/// </summary>
		/// <param name="predicate"></param>
		/// <exception cref="ArgumentException"></exception>
		public static async Task WaitUntil_Update(this Func<bool> predicate)
		{
			if (predicate == null)
				throw new ArgumentException("Predicate is null");
			
			while (!predicate.Invoke())
				await Awaitable.NextFrameAsync();
		}
		
		/// <summary>
		/// Return a task to await while a certain condition is true and check every timeDelayInSec
		/// </summary>
		/// <param name="predicate"></param>
		/// <exception cref="ArgumentException"></exception>
		public static async Task WaitUntil(this Func<bool> predicate,float timeDelayInSec)
		{
			if (predicate == null)
				throw new ArgumentException("Predicate is null");
			
			while (!predicate.Invoke())
				await Awaitable .WaitForSecondsAsync(timeDelayInSec);
		}

		#endregion
	}
}