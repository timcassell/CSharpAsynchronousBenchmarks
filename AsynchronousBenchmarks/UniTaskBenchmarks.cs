using BenchmarkDotNet.Attributes;
using Helper;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace AsynchronousBenchmarks
{
    public class UniTaskAsyncHelper
    {
        public bool hasInvoked;
        private readonly bool _isPending;
        private readonly Stack<AutoResetUniTaskCompletionSource> _completionSources;

        public UniTaskAsyncHelper(bool isPending)
        {
            _isPending = isPending;
            _completionSources = new Stack<AutoResetUniTaskCompletionSource>(2);
        }

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().TrySetResult();
            }
        }

        public UniTask GetBaseUniTask()
        {
            if (!_isPending)
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
        private static UniTaskAsyncHelper _uniTaskHelper;

        [GlobalSetup(Target = nameof(UniTask))]
        public void UniTaskSetup()
        {
            _uniTaskHelper = new UniTaskAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            // UniTask doesn't have an API to clear its object pool, so we set hasInvoked instead.
            _uniTaskHelper.hasInvoked = true;
            UniTask();
            _uniTaskHelper.hasInvoked = false;
        }

        [Benchmark]
        public void UniTask()
        {
            UniTask_ExecuteAsync().Forget();
            if (!_uniTaskHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _uniTaskHelper.hasInvoked = true;
                UniTask_ExecuteAsync().Forget();
            }
            _uniTaskHelper.ResolvePending();
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

        private static async UniTask<T> UniTask_GetValueAsync<T>()
        {
            await _uniTaskHelper.GetBaseUniTask();
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .ContinueWith callbacks don't need to capture `this`, which we are not interested in measuring.
        private static UniTaskAsyncHelper _uniTaskHelper;

        [GlobalSetup(Target = nameof(UniTask))]
        public void UniTaskSetup()
        {
            _uniTaskHelper = new UniTaskAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            // UniTask doesn't have an API to clear its object pool, so we set hasInvoked instead.
            _uniTaskHelper.hasInvoked = true;
            UniTask();
            _uniTaskHelper.hasInvoked = false;
        }

        [Benchmark]
        public void UniTask()
        {
            UniTask_ExecuteAsync().Forget();
            if (!_uniTaskHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _uniTaskHelper.hasInvoked = true;
                UniTask_ExecuteAsync().Forget();
            }
            _uniTaskHelper.ResolvePending();
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

        private static UniTask<T> UniTask_GetValueAsync<T>()
        {
            return _uniTaskHelper.GetBaseUniTask()
                .ContinueWith(() => default(T));
        }
    }
}