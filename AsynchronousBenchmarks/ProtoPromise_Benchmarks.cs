using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Portability;
using Proto.Promises;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AsynchronousBenchmarks
{
    public readonly struct ProtoAsyncHelper(bool isPending)
    {
        private readonly Stack<Promise.Deferred> _deferreds = new(2);

        public void ResolvePending()
        {
            while (_deferreds.Count > 0)
            {
                _deferreds.Pop().Resolve();
            }
        }

        public Promise GetBasePromise()
        {
            if (!isPending)
            {
                return Promise.Resolved();
            }
            var deferred = Promise.NewDeferred();
            _deferreds.Push(deferred);
            return deferred.Promise;
        }
    }

    partial class AsyncAwait
    {
        // static to match ContinueWith
        private static ProtoAsyncHelper s_protoHelper;

        [GlobalSetup(Target = nameof(ProtoPromise))]
        public void ProtoPromiseSetup()
        {
            s_protoHelper = new ProtoAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(ProtoPromise))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void ProtoPromiseCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            ExecuteForSurvived();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;

            // Prevents Promise surviving on the stack during the measurement.
            [MethodImpl(MethodImplOptions.NoInlining | CodeGenHelper.AggressiveOptimizationOption)]
            static void ExecuteForSurvived()
            {
                // Not clearing the pool to stay as fair as possible with UniTask, which doesn't have an API to clear its object pool. We just run the method twice without resolving the deferreds instead.
                //Promise.Manager.ClearObjectPool();
                Proto_ExecuteAsync().Forget();
                Proto_ExecuteAsync().Forget();
                s_protoHelper.ResolvePending();
            }
        }

        [Benchmark]
        public void ProtoPromise()
        {
            Proto_ExecuteAsync().Forget();
            s_protoHelper.ResolvePending();
        }

        private static async Promise Proto_ExecuteAsync()
        {
            await Proto_GetAndConsumeValuesAsync<Struct32, object>();
            await Proto_GetAndConsumeValuesAsync<object, Struct32>();
        }

        private static async Promise Proto_GetAndConsumeValuesAsync<T1, T2>()
        {
            _ = await Proto_GetValueAsync<T1>();
            _ = await Proto_GetValueAsync<T2>();
        }

        private static async Promise<T?> Proto_GetValueAsync<T>()
        {
            await s_protoHelper.GetBasePromise();
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .Then callbacks don't need to capture `this`, which we are not interested in measuring.
        private static ProtoAsyncHelper s_protoHelper;

        [GlobalSetup(Target = nameof(ProtoPromise))]
        public void ProtoPromiseSetup()
        {
            s_protoHelper = new ProtoAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(ProtoPromise))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void ProtoPromiseCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            ExecuteForSurvived();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;

            // Prevents Promise surviving on the stack during the measurement.
            [MethodImpl(MethodImplOptions.NoInlining | CodeGenHelper.AggressiveOptimizationOption)]
            static void ExecuteForSurvived()
            {
                // Not clearing the pool to stay as fair as possible with UniTask, which doesn't have an API to clear its object pool. We just run the method twice without resolving the deferreds instead.
                //Promise.Manager.ClearObjectPool();
                Proto_ExecuteAsync().Forget();
                Proto_ExecuteAsync().Forget();
                s_protoHelper.ResolvePending();
            }
        }

        [Benchmark]
        public void ProtoPromise()
        {
            Proto_ExecuteAsync().Forget();
            s_protoHelper.ResolvePending();
        }

        private static Promise Proto_ExecuteAsync()
        {
            return Proto_GetAndConsumeValuesAsync<Struct32, object>()
                .Then(() => Proto_GetAndConsumeValuesAsync<object, Struct32>())
                .Then(() => { });
        }

        private static Promise Proto_GetAndConsumeValuesAsync<T1, T2>()
        {
            return Proto_GetValueAsync<T1>()
                .Then(_ => Proto_GetValueAsync<T2>())
                .Then(_ => { });
        }

        private static Promise<T?> Proto_GetValueAsync<T>()
        {
            return s_protoHelper.GetBasePromise()
                .Then(() => default(T));
        }
    }
}