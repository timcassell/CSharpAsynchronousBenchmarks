using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Portability;
using Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AsynchronousBenchmarks
{
    public readonly struct TaskAsyncHelper(bool isPending)
    {
        private readonly Stack<TaskCompletionSource<bool>> _completionSources = new(16);

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().SetResult(true);
            }
        }

        public Task GetBaseTask()
        {
            if (!isPending)
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
        private static TaskAsyncHelper s_taskHelper;

        [GlobalSetup(Target = nameof(Task))]
        public void TaskSetup()
        {
            s_taskHelper = new TaskAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(Task))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void TaskCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            _ = Task_ExecuteAsync();
            _ = Task_ExecuteAsync();
            s_taskHelper.ResolvePending();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;
        }

        [Benchmark]
        public void Task()
        {
            _ = Task_ExecuteAsync();
            s_taskHelper.ResolvePending();
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

        private static async Task<T?> Task_GetValueAsync<T>()
        {
            await s_taskHelper.GetBaseTask();
            return default;
        }
    }

    partial class ContinueWith
    {
        // Static so .ContinueWith callbacks don't need to capture `this`, which we are not interested in measuring.
        private static TaskAsyncHelper s_taskHelper;

        [GlobalSetup(Target = nameof(Task))]
        public void TaskSetup()
        {
            s_taskHelper = new TaskAsyncHelper(Pending);
        }

        [GlobalCleanup(Target = nameof(Task))]
        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        public void TaskCleanup()
        {
            InitTotalMemory();
            var initialTotalMemory = GetTotalMemory();
            _ = Task_ExecuteAsync();
            _ = Task_ExecuteAsync();
            s_taskHelper.ResolvePending();
            _pooledMemory = GetTotalMemory() - initialTotalMemory;
        }

        [Benchmark]
        public void Task()
        {
            _ = Task_ExecuteAsync();
            s_taskHelper.ResolvePending();
        }

        private static Task Task_ExecuteAsync()
        {
            return Task_GetAndConsumeValuesAsync<Struct32, object>()
                .ContinueWith(_ => Task_GetAndConsumeValuesAsync<object, Struct32>(), TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static Task Task_GetAndConsumeValuesAsync<T1, T2>()
        {
            return Task_GetValueAsync<T1>()
                .ContinueWith(_ => Task_GetValueAsync<T2>(), TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static Task<T?> Task_GetValueAsync<T>()
        {
            return s_taskHelper.GetBaseTask()
                .ContinueWith(_ => default(T), TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}