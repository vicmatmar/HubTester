using System;
using System.Threading;
using System.Threading.Tasks;

namespace Centralite.Common.Utilities
{
    public static class BackgroundQueue
    {
        private static Task previousTask = Task.FromResult(true);
        private static object key = new object();
        public static Task QueueTask(Action action)
        {
            lock (key)
            {
                previousTask = previousTask.ContinueWith(t => action()
                    , CancellationToken.None
                    , TaskContinuationOptions.ExecuteSynchronously
                    , TaskScheduler.Default);
                return previousTask;
            }
        }

        public static Task<T> QueueTask<T>(Func<T> work)
        {
            lock (key)
            {
                var task = previousTask.ContinueWith(t => work()
                    , CancellationToken.None
                    , TaskContinuationOptions.ExecuteSynchronously
                    , TaskScheduler.Default);
                previousTask = task;
                return task;
            }
        }
    }
}
