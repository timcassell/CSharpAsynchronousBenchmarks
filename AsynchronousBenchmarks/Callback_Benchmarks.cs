using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Portability;

namespace AsynchronousBenchmarks
{
    public readonly struct CallbackAsyncHelper(bool isPending)
    {
        private readonly Stack<Action<Exception?>> _callbacks = new(2);

        public void ResolvePending()
        {
            while (_callbacks.Count > 0)
            {
                _callbacks.Pop().Invoke(null);
            }
        }

        public void OnBaseCallback(Action<Exception?> callback)
        {
            if (!isPending)
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
        private static CallbackAsyncHelper s_callbackHelper;

        [GlobalSetup(Target = nameof(Callback))]
        public void CallbackSetup()
        {
            s_callbackHelper = new CallbackAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(Callback))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void CallbackCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            ExecuteForSurvived();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;

            // Not really necessary, but matches the shape of the other benchmarks.
            [MethodImpl(MethodImplOptions.NoInlining | CodeGenHelper.AggressiveOptimizationOption)]
            static void ExecuteForSurvived()
            {
                Callback_ExecuteAsync(null);
                Callback_ExecuteAsync(null);
                s_callbackHelper.ResolvePending();
            }
        }

        [Benchmark(Baseline = true)]
        public void Callback()
        {
            // Technically, callbacks don't support async/await. But this is used to measure the difference in time and allocations vs real async/await code.
            Callback_ExecuteAsync(null);
            s_callbackHelper.ResolvePending();
        }

        private static void Callback_ExecuteAsync(Action<Exception?>? callback)
        {
            Callback_GetAndConsumeValuesAsync<Struct32, object>(e1 =>
            {
                if (e1 != null)
                {
                    callback?.Invoke(e1);
                    return;
                }
                Callback_GetAndConsumeValuesAsync<object, Struct32>(e2 =>
                {
                    if (e2 != null)
                    {
                        callback?.Invoke(e2);
                        return;
                    }
                    callback?.Invoke(null);
                });
            });
        }

        private static void Callback_GetAndConsumeValuesAsync<T1, T2>(Action<Exception?>? callback)
        {
            Callback_GetValueAsync<T1>((t1, e1) =>
            {
                if (e1 != null)
                {
                    callback?.Invoke(e1);
                    return;
                }
                Callback_GetValueAsync<T2>((t2, e2) => 
                {
                    if (e2 != null)
                    {
                        callback?.Invoke(e2);
                        return;
                    }
                    callback?.Invoke(null);
                });
            });
            
        }

        private static void Callback_GetValueAsync<T>(Action<T?, Exception?>? callback)
        {
            s_callbackHelper.OnBaseCallback(e =>
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
        private AsyncAwait? _asyncAwait;

        [GlobalSetup(Target = nameof(Callback))]
        public void CallbackSetup()
        {
            _asyncAwait = new AsyncAwait() { Pending = Pending };
            _asyncAwait.CallbackSetup();
        }

        [GlobalCleanup(Target = nameof(Callback))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void CallbackCleanup()
        {
            _asyncAwait!.CallbackCleanup();
            _pooledMemory = _asyncAwait._pooledMemory;
        }

        [Benchmark(Baseline = true)]
        public void Callback()
        {
            // Technically, callbacks don't have a ContinueWith or Then API. But this is used to measure the difference in time and allocations vs real async/await code.
            _asyncAwait!.Callback();
        }
    }
}