using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Portability;
using Helper;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AsynchronousBenchmarks
{
    public readonly struct ValueTaskAsyncHelper(bool isPending)
    {
        private readonly Stack<TaskCompletionSource<bool>> _completionSources = new(2);

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().SetResult(true);
            }
        }

        public ValueTask GetBaseValueTask()
        {
            if (!isPending)
            {
                return new ValueTask();
            }
            var completionSource = new TaskCompletionSource<bool>();
            _completionSources.Push(completionSource);
            return new ValueTask(completionSource.Task);
        }
    }

    partial class AsyncAwait
    {
        // static to match ContinueWith
        private static ValueTaskAsyncHelper s_valueTaskHelper;

        [GlobalSetup(Target = nameof(ValueTask))]
        public void ValueTaskSetup()
        {
            s_valueTaskHelper = new ValueTaskAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(ValueTask))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void ValueTaskCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            _ = ValueTask_ExecuteAsync();
            _ = ValueTask_ExecuteAsync();
            s_valueTaskHelper.ResolvePending();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;
        }

        [Benchmark]
        public void ValueTask()
        {
            _ = ValueTask_ExecuteAsync();
            s_valueTaskHelper.ResolvePending();
        }

        private static async ValueTask ValueTask_ExecuteAsync()
        {
            await ValueTask_GetAndConsumeValuesAsync<Struct32, object>();
            await ValueTask_GetAndConsumeValuesAsync<object, Struct32>();
        }

        private static async ValueTask ValueTask_GetAndConsumeValuesAsync<T1, T2>()
        {
            _ = await ValueTask_GetValueAsync<T1>();
            _ = await ValueTask_GetValueAsync<T2>();
        }

        private static async ValueTask<T?> ValueTask_GetValueAsync<T>()
        {
            await s_valueTaskHelper.GetBaseValueTask();
            return default;
        }
    }

    partial class ContinueWith
    {
        [Benchmark]
        public void ValueTask()
        {
            // ValueTask does not have a ContinueWith or Then API, so we just throw NotImplementedException.
            throw new System.NotImplementedException();
        }
    }
}