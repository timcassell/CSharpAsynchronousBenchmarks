using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using BenchmarkDotNet.Portability;
using System.Runtime.CompilerServices;

namespace AsynchronousBenchmarks
{
    public readonly struct UniTaskAsyncHelper(bool isPending)
    {
        private readonly Stack<AutoResetUniTaskCompletionSource> _completionSources = new(2);

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().TrySetResult();
            }
        }

        public UniTask GetBaseUniTask()
        {
            if (!isPending)
            {
                return UniTask.CompletedTask;
            }
            var completionSource = AutoResetUniTaskCompletionSource.Create();
            _completionSources.Push(completionSource);
            return completionSource.Task;
        }
    }

    partial class AsyncAwait
    {
        // static to match ContinueWith
        private static UniTaskAsyncHelper s_uniTaskHelper;

        [GlobalSetup(Target = nameof(UniTask))]
        public void UniTaskSetup()
        {
            s_uniTaskHelper = new UniTaskAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(UniTask))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void UniTaskCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            ExecuteForSurvived();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;

            // Prevents Task surviving on the stack during the measurement.
            [MethodImpl(MethodImplOptions.NoInlining | CodeGenHelper.AggressiveOptimizationOption)]
            static void ExecuteForSurvived()
            {
                // UniTask doesn't have an API to clear its object pool. We just run the method twice without resolving the completion sources instead.
                UniTask_ExecuteAsync().Forget();
                UniTask_ExecuteAsync().Forget();
                s_uniTaskHelper.ResolvePending();
            }
        }

        [Benchmark]
        public void UniTask()
        {
            UniTask_ExecuteAsync().Forget();
            s_uniTaskHelper.ResolvePending();
        }

        private static async UniTask UniTask_ExecuteAsync()
        {
            await UniTask_GetAndConsumeValuesAsync<Struct32, object>();
            await UniTask_GetAndConsumeValuesAsync<object, Struct32>();
        }

        private static async UniTask UniTask_GetAndConsumeValuesAsync<T1, T2>()
        {
            _ = await UniTask_GetValueAsync<T1>();
            _ = await UniTask_GetValueAsync<T2>();
        }

        private static async UniTask<T?> UniTask_GetValueAsync<T>()
        {
            await s_uniTaskHelper.GetBaseUniTask();
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .ContinueWith callbacks don't need to capture `this`, which we are not interested in measuring.
        private static UniTaskAsyncHelper s_uniTaskHelper;

        [GlobalSetup(Target = nameof(UniTask))]
        public void UniTaskSetup()
        {
            s_uniTaskHelper = new UniTaskAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(UniTask))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void UniTaskCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            ExecuteForSurvived();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;

            // Prevents Task surviving on the stack during the measurement.
            [MethodImpl(MethodImplOptions.NoInlining | CodeGenHelper.AggressiveOptimizationOption)]
            static void ExecuteForSurvived()
            {
                // UniTask doesn't have an API to clear its object pool. We just run the method twice without resolving the completion sources instead.
                UniTask_ExecuteAsync().Forget();
                UniTask_ExecuteAsync().Forget();
                s_uniTaskHelper.ResolvePending();
            }
        }

        [Benchmark]
        public void UniTask()
        {
            UniTask_ExecuteAsync().Forget();
            s_uniTaskHelper.ResolvePending();
        }

        private static UniTask UniTask_ExecuteAsync()
        {
            return UniTask_GetAndConsumeValuesAsync<Struct32, object>()
                .ContinueWith(() => UniTask_GetAndConsumeValuesAsync<object, Struct32>())
                .ContinueWith(() => { });
        }

        private static UniTask UniTask_GetAndConsumeValuesAsync<T1, T2>()
        {
            return UniTask_GetValueAsync<T1>()
                .ContinueWith(_ => UniTask_GetValueAsync<T2>())
                .ContinueWith(_ => { });
        }

        private static UniTask<T?> UniTask_GetValueAsync<T>()
        {
            return s_uniTaskHelper.GetBaseUniTask()
                .ContinueWith(() => default(T));
        }
    }
}