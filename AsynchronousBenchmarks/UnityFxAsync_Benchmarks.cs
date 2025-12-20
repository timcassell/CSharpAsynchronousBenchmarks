using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Portability;
using Helper;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace AsynchronousBenchmarks
{
    public readonly struct UnityFxAsyncHelper(bool isPending)
    {
        private readonly Stack<AsyncCompletionSource> _completionSources = new(2);

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().TrySetCompleted();
            }
        }

        public AsyncResult GetBaseUnityFx()
        {
            if (!isPending)
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
        private static UnityFxAsyncHelper s_unityFxHelper;

        [GlobalSetup(Target = nameof(UnityFxAsync))]
        public void UnityFxSetup()
        {
            s_unityFxHelper = new UnityFxAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(UnityFx))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void UnityFxCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            UnityFx_ExecuteAsync().Done();
            UnityFx_ExecuteAsync().Done();
            s_unityFxHelper.ResolvePending();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;
        }

        [Benchmark]
        public void UnityFxAsync()
        {
            UnityFx_ExecuteAsync().Done();
            s_unityFxHelper.ResolvePending();
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

        private static async AsyncResult<T?> UnityFx_GetValueAsync<T>()
        {
            await s_unityFxHelper.GetBaseUnityFx();
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .ContinueWith callbacks don't need to capture `this`, which we are not interested in measuring.
        private static UnityFxAsyncHelper s_unityFxHelper;

        [GlobalSetup(Target = nameof(UnityFxAsync))]
        public void UnityFxSetup()
        {
            s_unityFxHelper = new UnityFxAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(UnityFx))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void UnityFxCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            UnityFx_ExecuteAsync().Done();
            UnityFx_ExecuteAsync().Done();
            s_unityFxHelper.ResolvePending();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;
        }

        [Benchmark]
        public void UnityFxAsync()
        {
            UnityFx_ExecuteAsync().Done();
            s_unityFxHelper.ResolvePending();
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

        private static IAsyncOperation<T?> UnityFx_GetValueAsync<T>()
        {
            return s_unityFxHelper.GetBaseUnityFx()
                .ContinueWith(_ => default(T), AsyncContinuationOptions.ExecuteSynchronously);
        }
    }
}