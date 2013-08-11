using System;
using System.Threading.Tasks;

namespace Xamarin.Juice
{
	public static class TaskExtensions
	{
		public static Task Continue<T> (this Task<T> self, Action<Task<T>> act)
		{
			return self.ContinueWith (act, TaskScheduler.FromCurrentSynchronizationContext ());
		}
	}
}