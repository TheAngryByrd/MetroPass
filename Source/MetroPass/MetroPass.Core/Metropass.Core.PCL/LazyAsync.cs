using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Metropass.Core.PCL
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory)) { }

        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { }

        public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
    }


    public interface IAwaiter<out TResult> : INotifyCompletion // or ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }

        TResult GetResult();
    }

    public struct FuncAwaiter<TResult> : IAwaiter<TResult>
    {
        private readonly Task<TResult> task;

        public FuncAwaiter(Func<TResult> function)
        {
            this.task = new Task<TResult>(function);
            this.task.Start();
        }

        bool IAwaiter<TResult>.IsCompleted
        {
            get
            {
                return this.task.IsCompleted;
            }
        }

        TResult IAwaiter<TResult>.GetResult()
        {
            return this.task.Result;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            new Task(continuation).Start();
        }
    }


    //public class LazyAsync<T> 
    //{
    //    public LazyAsync(Func<T> valueFactory)
    //    { 
    //        Start(valueFactory);
    //    }

    //    private void Start(Func<T> valueFactory)
    //    {

    //        _value = Task.Factory.StartNew(valueFactory);
    //    }

    //    public bool IsValueCreated
    //    {
    //        get
    //        {
    //            return _value.IsCompleted;
    //        }
    //    }

        

    //    private Task<T> _value;
    //    public T Value
    //    {
    //        get 
    //        {                         
    //            return _value.Result;
    //        }
    //    }
    //}
}
