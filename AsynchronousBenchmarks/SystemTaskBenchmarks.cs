using BenchmarkDotNet.Attributes;
using Helper;
using Proto.Promises;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousBenchmarks
{
    static class TaskHelper
    {
        public static Task taskVoid;
        public static Task<Vector4> taskVector;
        public static Task<object> taskObject;

        public static void SetTasks()
        {
            // There is no TaskCompletionSource, so just use TaskCompletionSource<bool>
            var taskVoidSource = new TaskCompletionSource<bool>();
            var taskVectorSource = new TaskCompletionSource<Vector4>();
            var taskObjectSource = new TaskCompletionSource<object>();
            taskVoid = taskVoidSource.Task;
            taskVector = taskVectorSource.Task;
            taskObject = taskObjectSource.Task;
            taskVoidSource.SetResult(true);
            taskVectorSource.SetResult(Instances.vector);
            taskObjectSource.SetResult(Instances.obj);
        }

        public static TaskCompletionSource<bool>[] taskVoids;
        public static TaskCompletionSource<Vector4>[] taskVectors;
        public static TaskCompletionSource<object>[] taskObjects;

        public static void SetCompletionSources(int n)
        {
            if (taskVoids != null)
            {
                // Don't recreate completion sources. This is necessary because this is ran separately for the JIT optimizer.
                return;
            }

            // There is no TaskCompletionSource, so just use TaskCompletionSource<bool>
            taskVoids = new TaskCompletionSource<bool>[n];
            taskVectors = new TaskCompletionSource<Vector4>[n];
            taskObjects = new TaskCompletionSource<object>[n];
            for (int i = 0; i < n; ++i)
            {
                taskVoids[i] = new TaskCompletionSource<bool>();
                taskVectors[i] = new TaskCompletionSource<Vector4>();
                taskObjects[i] = new TaskCompletionSource<object>();
            }
        }

        public static void ClearCompletionSources()
        {
            taskVoids = default;
            taskVectors = default;
            taskObjects = default;
        }

        public static void ResolveCompletionSources()
        {
            for (int i = 0, max = taskVoids.Length; i < max; ++i)
            {
                taskVoids[i].TrySetResult(true);
                taskVectors[i].TrySetResult(Instances.vector);
                taskObjects[i].TrySetResult(Instances.obj);
            }
        }
    }

    partial class ContinueWithPending
    {
        [GlobalSetup(Target = nameof(DotNetTask))]
        public void GlobalSetupTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            TaskHelper.SetCompletionSources(1);
            ExecuteTask(1);
            TaskHelper.ClearCompletionSources();
        }

        [IterationSetup(Target = nameof(DotNetTask))]
        public void IterationSetupTasks()
        {
            TaskHelper.SetCompletionSources(N);
        }

        [IterationCleanup(Target = nameof(DotNetTask))]
        public void IterationCleanupTasks()
        {
            TaskHelper.ClearCompletionSources();
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            ExecuteTask(N);
        }

        private void ExecuteTask(int n)
        {
            TaskCompletionSource<object> deferred = new TaskCompletionSource<object>();
            var task = deferred.Task;

            for (int i = 0; i < n; ++i)
            {
                int index = i;
                task = task
                    .ContinueWith(_ => (Task) TaskHelper.taskVoids[index].Task, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskVectors[index].Task, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskObjects[index].Task, TaskContinuationOptions.ExecuteSynchronously).Unwrap();
            }

            deferred.SetResult(Instances.obj);
            TaskHelper.ResolveCompletionSources();
            task.Wait();
        }
    }

    partial class ContinueWithResolved
    {
        [GlobalSetup(Target = nameof(DotNetTask))]
        public void GlobalSetupTasks()
        {
            TaskHelper.SetTasks();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteTask(1);
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            ExecuteTask(N);
        }

        private void ExecuteTask(int n)
        {
            TaskCompletionSource<object> deferred = new TaskCompletionSource<object>();
            var task = deferred.Task;

            for (int i = 0; i < n; ++i)
            {
                task = task
                    .ContinueWith(_ => TaskHelper.taskVoid, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskVector, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskObject, TaskContinuationOptions.ExecuteSynchronously).Unwrap();
            }

            deferred.SetResult(Instances.obj);
            task.Wait();
        }
    }

    partial class ContinueWithFromValue
    {
        [GlobalSetup(Target = nameof(DotNetTask))]
        public void GlobalSetupTasks()
        {
            TaskHelper.SetTasks();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteTask(1);
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            ExecuteTask(N);
        }

        private void ExecuteTask(int n)
        {
            TaskCompletionSource<object> deferred = new TaskCompletionSource<object>();
            var task = deferred.Task;

            for (int i = 0; i < n; ++i)
            {
                task = task
                    .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ => Instances.vector, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ => Instances.obj, TaskContinuationOptions.ExecuteSynchronously);
            }

            deferred.SetResult(Instances.obj);
            task.Wait();
        }
    }

    partial class AwaitPending
    {
        [GlobalSetup(Target = nameof(DotNetTask))]
        public void GlobalSetupTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            TaskHelper.SetCompletionSources(1);
            ExecuteTask(1);
            TaskHelper.ClearCompletionSources();
        }

        [IterationSetup(Target = nameof(DotNetTask))]
        public void IterationSetupTasks()
        {
            TaskHelper.SetCompletionSources(N);
        }

        [IterationCleanup(Target = nameof(DotNetTask))]
        public void IterationCleanupTasks()
        {
            TaskHelper.ClearCompletionSources();
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            ExecuteTask(N);
        }

        private void ExecuteTask(int n)
        {
            _ = AwaitTasks(n);
            TaskHelper.ResolveCompletionSources();
        }

        private async Task<object> AwaitTasks(int n)
        {
            for (int i = 0; i < n; ++i)
            {
                await (Task) TaskHelper.taskVoids[i].Task;
                _ = await TaskHelper.taskVectors[i].Task;
                _ = await TaskHelper.taskObjects[i].Task;
            }
            return Instances.obj;
        }
    }

    partial class AwaitResolved
    {
        [GlobalSetup(Target = nameof(DotNetTask))]
        public void GlobalSetupTasks()
        {
            TaskHelper.SetTasks();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteTask(1);
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            ExecuteTask(N);
        }

        private void ExecuteTask(int n)
        {
            AwaitTasks(n).Wait();
        }

        private async Task<object> AwaitTasks(int n)
        {
            for (int i = 0; i < n; ++i)
            {
                await TaskHelper.taskVoid.ConfigureAwait(true);
                _ = await TaskHelper.taskVector.ConfigureAwait(true);
                _ = await TaskHelper.taskObject.ConfigureAwait(true);
            }
            return Instances.obj;
        }
    }

    partial class AsyncPending
    {
        private static Promise.Deferred task_deferred;
        private static Promise task_promise;
        private static long task_counter;

        [GlobalSetup(Target = nameof(DotNetTask))]
        public void GlobalSetupTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            IterationSetupDotNetTasks();
            ExecuteTask(1);
        }

        [IterationSetup(Target = nameof(DotNetTask))]
        public void IterationSetupDotNetTasks()
        {
            // Create a promise to await so that the async functions won't complete synchronously.
            task_deferred = Promise.NewDeferred();
            task_promise = task_deferred.Promise;
            task_counter = 0L;
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            ExecuteTask(N);
        }

        private void ExecuteTask(int n)
        {
            for (int i = 0; i < n; ++i)
            {
                _ = TaskVoid();
                _ = TaskVector();
                _ = TaskObject();
            }

            async Task TaskVoid()
            {
                Interlocked.Increment(ref task_counter);
                await task_promise;
                Interlocked.Decrement(ref task_counter);
            }

            async Task<Vector4> TaskVector()
            {
                Interlocked.Increment(ref task_counter);
                await task_promise;
                Interlocked.Decrement(ref task_counter);
                return Instances.vector;
            }

            async Task<object> TaskObject()
            {
                Interlocked.Increment(ref task_counter);
                await task_promise;
                Interlocked.Decrement(ref task_counter);
                return Instances.obj;
            }

            task_deferred.Resolve();
            Promise.Manager.HandleCompletesAndProgress();
            while (Interlocked.Read(ref task_counter) > 0) { }
        }
    }

    partial class AsyncResolved
    {
        [GlobalSetup(Target = nameof(DotNetTask))]
        public void GlobalSetupTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteTask(1);
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            ExecuteTask(N);
        }

        private void ExecuteTask(int n)
        {
            for (int i = 0; i < n; ++i)
            {
                TaskVoid().Wait();
                TaskVector().Wait();
                TaskObject().Wait();
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            async Task TaskVoid()
            {
            }

            async Task<Vector4> TaskVector()
            {
                return Instances.vector;
            }

            async Task<object> TaskObject()
            {
                return Instances.obj;
            }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }
    }
}