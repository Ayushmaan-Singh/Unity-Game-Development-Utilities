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
	}
}