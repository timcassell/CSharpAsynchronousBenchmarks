using BenchmarkDotNet.Attributes;
using Helper;
using RSG;
using System.Collections.Generic;

namespace AsynchronousBenchmarks
{
    public class RsgAsyncHelper
    {
        public bool hasInvoked;
        private readonly bool _isPending;
        private readonly Stack<Promise> _promises;

        public RsgAsyncHelper(bool isPending)
        {
            _isPending = isPending;
            _promises = new Stack<Promise>(2);
        }

        public void ResolvePending()
        {
            while (_promises.Count > 0)
            {
                _promises.Pop().Resolve();
            }
        }

        public IPromise GetBasePromise()
        {
            if (!_isPending)
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
        private static RsgAsyncHelper _rsgHelper;

        [GlobalSetup(Target = nameof(RsgPromise))]
        public void RsgPromiseSetup()
        {
            _rsgHelper = new RsgAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            _rsgHelper.hasInvoked = true;
            RsgPromise();
            _rsgHelper.hasInvoked = false;
        }

        [Benchmark]
        public void RsgPromise()
        {
            Rsg_ExecuteAsync().Done();
            if (!_rsgHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _rsgHelper.hasInvoked = true;
                Rsg_ExecuteAsync().Done();
            }
            _rsgHelper.ResolvePending();
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

        private static IPromise<T> Rsg_GetValueAsync<T>()
        {
            // RSG promises don't support converting to another value directly without wrapping it in a promise.
            return _rsgHelper.GetBasePromise()
                .Then(() => Promise<T>.Resolved(default));
        }
    }
}