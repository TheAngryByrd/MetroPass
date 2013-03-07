using System;
using System.Threading;
using System.Threading.Tasks;

namespace Framework
{
    public static class TaskHelper
    {
        public static void StartFromCurrentUIContext(this TaskFactory factory, Action action)
        {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}