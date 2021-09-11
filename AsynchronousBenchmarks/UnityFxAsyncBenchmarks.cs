using BenchmarkDotNet.Attributes;
using Helper;
using System.Collections.Generic;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace AsynchronousBenchmarks
{
    public class UnityFxAsyncHelper
    {
        public bool hasInvoked;
        private readonly bool _isPending;
        private readonly Stack<AsyncCompletionSource> _completionSources;

        public UnityFxAsyncHelper(bool isPending)
        {
            _isPending = isPending;
            _completionSources = new Stack<AsyncCompletionSource>(2);
        }

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().TrySetCompleted();
            }
        }

        public AsyncResult GetBaseUnityFx()
        {
            if (!_isPending)
            {
                return AsyncResult.CompletedOperation;
            }
            var completionSource = new AsyncCompletionSource(AsyncOperationStatus.Running);
            _completionSources.Push(completionSource);
            return completionSource;
        }
    }

    partial class AsyncAwait
    {
        // static to match ContinueWith
        private static UnityFxAsyncHelper _UnityFxHelper;

        [GlobalSetup(Target = nameof(UnityFxAsync))]
        public void UnityFxSetup()
        {
            _UnityFxHelper = new UnityFxAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            _UnityFxHelper.hasInvoked = true;
            UnityFxAsync();
            _UnityFxHelper.hasInvoked = false;
        }

        [Benchmark]
        public void UnityFxAsync()
        {
            UnityFx_ExecuteAsync().Done();
            if (!_UnityFxHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _UnityFxHelper.hasInvoked = true;
                UnityFx_ExecuteAsync().Done();
            }
            _UnityFxHelper.ResolvePending();
        }

        private static async AsyncResult UnityFx_ExecuteAsync()
        {
            await UnityFx_GetAndConsumeValuesAsync<Struct32, object>();
            await UnityFx_GetAndConsumeValuesAsync<object, Struct32>();
        }

        private static async AsyncResult UnityFx_GetAndConsumeValuesAsync<T1, T2>()
        {
            _ = await UnityFx_GetValueAsync<T1>();
            _ = await UnityFx_GetValueAsync<T2>();
        }

        private static async AsyncResult<T> UnityFx_GetValueAsync<T>()
        {
            await _UnityFxHelper.GetBaseUnityFx();
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .ContinueWith callbacks don't need to capture `this`, which we are not interested in measuring.
        private static UnityFxAsyncHelper _UnityFxHelper;

        [GlobalSetup(Target = nameof(UnityFxAsync))]
        public void UnityFxSetup()
        {
            _UnityFxHelper = new UnityFxAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            _UnityFxHelper.hasInvoked = true;
            UnityFxAsync();
            _UnityFxHelper.hasInvoked = false;
        }

        [Benchmark]
        public void UnityFxAsync()
        {
            UnityFx_ExecuteAsync().Done();
            if (!_UnityFxHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _UnityFxHelper.hasInvoked = true;
                UnityFx_ExecuteAsync().Done();
            }
            _UnityFxHelper.ResolvePending();
        }

        private static IAsyncOperation UnityFx_ExecuteAsync()
        {
            return UnityFx_GetAndConsumeValuesAsync<Struct32, object>()
                .ContinueWith(_ => UnityFx_GetAndConsumeValuesAsync<object, Struct32>(), AsyncContinuationOptions.ExecuteSynchronously)
                .ContinueWith(_ => { }, AsyncContinuationOptions.ExecuteSynchronously);
        }

        private static IAsyncOperation UnityFx_GetAndConsumeValuesAsync<T1, T2>()
        {
            return UnityFx_GetValueAsync<T1>()
                .ContinueWith(_ => UnityFx_GetValueAsync<T2>(), AsyncContinuationOptions.ExecuteSynchronously)
                .ContinueWith(_ => { }, AsyncContinuationOptions.ExecuteSynchronously);
        }

        private static IAsyncOperation<T> UnityFx_GetValueAsync<T>()
        {
            return _UnityFxHelper.GetBaseUnityFx()
                .ContinueWith(_ => default(T), AsyncContinuationOptions.ExecuteSynchronously);
        }
    }
}