using BenchmarkDotNet.Attributes;
using Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsynchronousBenchmarks
{
    public class TaskAsyncHelper
    {
        public bool hasInvoked;
        private readonly bool _isPending;
        private readonly Stack<TaskCompletionSource<bool>> _completionSources;

        public TaskAsyncHelper(bool isPending)
        {
            _isPending = isPending;
            _completionSources = new Stack<TaskCompletionSource<bool>>(2);
        }

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().SetResult(true);
            }
        }

        public Task GetBaseTask()
        {
            if (!_isPending)
            {
                return Task.CompletedTask;
            }
            var completionSource = new TaskCompletionSource<bool>();
            _completionSources.Push(completionSource);
            return completionSource.Task;
        }
    }

    partial class AsyncAwait
    {
        // static to match ContinueWith
        private static TaskAsyncHelper _taskHelper;

        [GlobalSetup(Target = nameof(Task))]
        public void TaskSetup()
        {
            _taskHelper = new TaskAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            _taskHelper.hasInvoked = true;
            Task();
            _taskHelper.hasInvoked = false;
        }

        [Benchmark]
        public void Task()
        {
            _ = Task_ExecuteAsync();
            if (!_taskHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _taskHelper.hasInvoked = true;
                _ = Task_ExecuteAsync();
            }
            _taskHelper.ResolvePending();
        }

        private static async Task Task_ExecuteAsync()
        {
            await Task_GetAndConsumeValuesAsync<Struct32, object>();
            await Task_GetAndConsumeValuesAsync<object, Struct32>();
        }

        private static async Task Task_GetAndConsumeValuesAsync<T1, T2>()
        {
            _ = await Task_GetValueAsync<T1>();
            _ = await Task_GetValueAsync<T2>();
        }

        private static async Task<T> Task_GetValueAsync<T>()
        {
            await _taskHelper.GetBaseTask();
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .ContinueWith callbacks don't need to capture `this`, which we are not interested in measuring.
        private static TaskAsyncHelper _taskHelper;

        [GlobalSetup(Target = nameof(Task))]
        public void TaskSetup()
        {
            _taskHelper = new TaskAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            _taskHelper.hasInvoked = true;
            Task();
            _taskHelper.hasInvoked = false;
        }

        [Benchmark]
        public void Task()
        {
            _ = Task_ExecuteAsync();
            if (!_taskHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _taskHelper.hasInvoked = true;
                _ = Task_ExecuteAsync();
            }
            _taskHelper.ResolvePending();
        }

        private static Task Task_ExecuteAsync()
        {
            return Task_GetAndConsumeValuesAsync<Struct32, object>()
                .ContinueWith(_ => Task_GetAndConsumeValuesAsync<object, Struct32>(), TaskContinuationOptions.ExecuteSynchronously)
                .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static Task Task_GetAndConsumeValuesAsync<T1, T2>()
        {
            return Task_GetValueAsync<T1>()
                .ContinueWith(_ => Task_GetValueAsync<T2>(), TaskContinuationOptions.ExecuteSynchronously)
                .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static Task<T> Task_GetValueAsync<T>()
        {
            return _taskHelper.GetBaseTask()
                .ContinueWith(_ => default(T), TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}