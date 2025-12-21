using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Portability;
using RSG;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AsynchronousBenchmarks
{
    public readonly struct RsgAsyncHelper(bool isPending)
    {
        private readonly Stack<Promise> _promises = new(2);

        public void ResolvePending()
        {
            while (_promises.Count > 0)
            {
                _promises.Pop().Resolve();
            }
        }

        public IPromise GetBasePromise()
        {
            if (!isPending)
            {
                return Promise.Resolved();
            }
            var promise = new Promise();
            _promises.Push(promise);
            return promise;
        }
    }

    partial class AsyncAwait
    {
        [Benchmark]
        public void RsgPromise()
        {
            // RSG promises don't support async/await, so we just throw NotImplementedException.
            throw new System.NotImplementedException();
        }
    }

    partial class ContinueWith
    {
        // Static so .Then callbacks don't need to capture `this`, which we are not interested in measuring.
        private static RsgAsyncHelper s_rsgHelper;

        [GlobalSetup(Target = nameof(RsgPromise))]
        public void RsgPromiseSetup()
        {
            s_rsgHelper = new(Pending);
        }

        [GlobalCleanup(Target = nameof(RsgPromise))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void RsgPromiseCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            ExecuteForSurvived();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;

            // Prevents Promise surviving on the stack during the measurement.
            [MethodImpl(MethodImplOptions.NoInlining | CodeGenHelper.AggressiveOptimizationOption)]
            static void ExecuteForSurvived()
            {
                Rsg_ExecuteAsync().Done();
                Rsg_ExecuteAsync().Done();
                s_rsgHelper.ResolvePending();
            }
        }

        [Benchmark]
        public void RsgPromise()
        {
            Rsg_ExecuteAsync().Done();
            s_rsgHelper.ResolvePending();
        }

        private static IPromise Rsg_ExecuteAsync()
        {
            return Rsg_GetAndConsumeValuesAsync<Struct32, object>()
                .Then(() => Rsg_GetAndConsumeValuesAsync<object, Struct32>())
                .Then(() => { });
        }

        private static IPromise Rsg_GetAndConsumeValuesAsync<T1, T2>()
        {
            return Rsg_GetValueAsync<T1>()
                .Then(_ => Rsg_GetValueAsync<T2>())
                .Then(_ => { });
        }

        private static IPromise<T?> Rsg_GetValueAsync<T>()
        {
            // RSG promises don't support converting to another value directly without wrapping it in a promise.
            return s_rsgHelper.GetBasePromise()
                .Then(() => Promise<T?>.Resolved(default));
        }
    }
}