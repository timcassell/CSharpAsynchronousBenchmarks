using BenchmarkDotNet.Attributes;
using Helper;
using System.Collections.Generic;
using System;

namespace AsynchronousBenchmarks
{
    public class CallbackAsyncHelper
    {
        public bool hasInvoked;
        private readonly bool _isPending;
        private readonly Stack<Action<Exception>> _callbacks;

        public CallbackAsyncHelper(bool isPending)
        {
            _isPending = isPending;
            _callbacks = new Stack<Action<Exception>>(2);
        }

        public void ResolvePending()
        {
            while (_callbacks.Count > 0)
            {
                _callbacks.Pop().Invoke(null);
            }
        }

        public void OnBaseCallback(Action<Exception> callback)
        {
            if (!_isPending)
            {
                callback.Invoke(null);
                return;
            }
            _callbacks.Push(callback);
        }
    }

    partial class AsyncAwait
    {
        // static to match ContinueWith
        private static CallbackAsyncHelper _callbackHelper;

        [GlobalSetup(Target = nameof(Callback))]
        public void CallbackSetup()
        {
            _callbackHelper = new CallbackAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            _callbackHelper.hasInvoked = true;
            Callback();
            _callbackHelper.hasInvoked = false;
        }

        [Benchmark(Baseline = true)]
        public void Callback()
        {
            // Technically, callbacks don't support async/await. But this is used to measure the difference in time and allocations vs real async/await code.

            Callback_ExecuteAsync(null);
            if (!_callbackHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _callbackHelper.hasInvoked = true;
                Callback_ExecuteAsync(null);
            }
            _callbackHelper.ResolvePending();
        }

        private static void Callback_ExecuteAsync(Action<Exception> callback)
        {
            Callback_GetAndConsumeValuesAsync<Struct32, object>(e1 =>
            {
                if (e1 != null)
                {
                    callback(e1);
                    return;
                }
                Callback_GetAndConsumeValuesAsync<object, Struct32>(e2 =>
                {
                    if (e2 != null)
                    {
                        callback(e2);
                        return;
                    }
                    callback?.Invoke(null);
                });
            });
        }

        private static void Callback_GetAndConsumeValuesAsync<T1, T2>(Action<Exception> callback)
        {
            Callback_GetValueAsync<T1>((t1, e1) =>
            {
                if (e1 != null)
                {
                    callback(e1);
                    return;
                }
                Callback_GetValueAsync<T2>((t2, e2) => 
                {
                    if (e2 != null)
                    {
                        callback(e2);
                        return;
                    }
                    callback?.Invoke(null);
                });
            });
            
        }

        private static void Callback_GetValueAsync<T>(Action<T, Exception> callback)
        {
            _callbackHelper.OnBaseCallback(e =>
            {
                if (e != null)
                {
                    callback?.Invoke(default, e);
                    return;
                }
                callback?.Invoke(default, null);
            });
        }
    }

    partial class ContinueWith
    {
        // The implementation is exactly the same, so we just call the other one.
        private AsyncAwait _asyncAwait;

        [GlobalSetup(Target = nameof(Callback))]
        public void CallbackSetup()
        {
            _asyncAwait = new AsyncAwait() { Pending = Pending };
            _asyncAwait.CallbackSetup();
        }

        [Benchmark(Baseline = true)]
        public void Callback()
        {
            // Technically, callbacks don't have a ContinueWith or Then API. But this is used to measure the difference in time and allocations vs real async/await code.
            _asyncAwait.Callback();
        }
    }
}