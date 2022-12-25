using BenchmarkDotNet.Attributes;
using Helper;
using Proto.Promises;
using System.Collections.Generic;

namespace AsynchronousBenchmarks
{
    public class ProtoAsyncHelper
    {
        public bool hasInvoked;
        private readonly bool _isPending;
        private readonly Stack<Promise.Deferred> _deferreds;

        public ProtoAsyncHelper(bool isPending)
        {
            _isPending = isPending;
            _deferreds = new Stack<Promise.Deferred>(2);
        }

        public void ResolvePending()
        {
            while (_deferreds.Count > 0)
            {
                _deferreds.Pop().Resolve();
            }
        }

        public Promise GetBasePromise()
        {
            if (!_isPending)
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
        private static ProtoAsyncHelper _protoHelper;

        [GlobalSetup(Target = nameof(ProtoPromise))]
        public void ProtoPromiseSetup()
        {
            _protoHelper = new ProtoAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            // Not clearing the pool to stay as fair as possible with UniTask, which doesn't have an API to clear its object pool. Set hasInvoked instead.
            _protoHelper.hasInvoked = true;
            ProtoPromise();
            _protoHelper.hasInvoked = false;
            //Promise.Manager.ClearObjectPool();
        }

        [Benchmark]
        public void ProtoPromise()
        {
            Proto_ExecuteAsync().Forget();
            if (!_protoHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _protoHelper.hasInvoked = true;
                Proto_ExecuteAsync().Forget();
            }
            _protoHelper.ResolvePending();
        }

        private static async Promise Proto_ExecuteAsync()
        {
            await Proto_GetAndConsumeValuesAsync<Struct32, object>();//.AwaitWithProgress(0.5f);
            await Proto_GetAndConsumeValuesAsync<object, Struct32>();//.AwaitWithProgress(1f);
        }

        private static async Promise Proto_GetAndConsumeValuesAsync<T1, T2>()
        {
            _ = await Proto_GetValueAsync<T1>();//.AwaitWithProgress(0.5f);
            _ = await Proto_GetValueAsync<T2>();//.AwaitWithProgress(1f);
        }

        private static async Promise<T> Proto_GetValueAsync<T>()
        {
            await _protoHelper.GetBasePromise();//.AwaitWithProgress(1f);
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .Then callbacks don't need to capture `this`, which we are not interested in measuring.
        private static ProtoAsyncHelper _protoHelper;

        [GlobalSetup(Target = nameof(ProtoPromise))]
        public void ProtoPromiseSetup()
        {
            _protoHelper = new ProtoAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            // Not clearing the pool to stay as fair as possible with UniTask, which doesn't have an API to clear its object pool. Set hasInvoked instead.
            _protoHelper.hasInvoked = true;
            ProtoPromise();
            _protoHelper.hasInvoked = false;
            //Promise.Manager.ClearObjectPool();
        }

        [Benchmark]
        public void ProtoPromise()
        {
            Proto_ExecuteAsync().Forget();
            if (!_protoHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _protoHelper.hasInvoked = true;
                Proto_ExecuteAsync().Forget();
            }
            _protoHelper.ResolvePending();
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

        private static Promise<T> Proto_GetValueAsync<T>()
        {
            return _protoHelper.GetBasePromise()
                .Then(() => default(T));
        }
    }
}