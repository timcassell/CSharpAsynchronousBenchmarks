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

        public static void ClearTasks()
        {
            taskVoid = default;
            taskVector = default;
            taskObject = default;
        }

        public static TaskCompletionSource<bool>[] taskVoids;
        public static TaskCompletionSource<Vector4>[] taskVectors;
        public static TaskCompletionSource<object>[] taskObjects;

        public static void SetCompletionSources(int N)
        {
            if (taskVoids != null)
            {
                // Don't recreate completion sources. This is necessary because this is ran separately for the JIT optimizer.
                return;
            }

            // There is no TaskCompletionSource, so just use TaskCompletionSource<bool>
            taskVoids = new TaskCompletionSource<bool>[N];
            taskVectors = new TaskCompletionSource<Vector4>[N];
            taskObjects = new TaskCompletionSource<object>[N];
            for (int i = 0; i < N; ++i)
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
        [IterationSetup(Target = nameof(DotNetTask))]
        public void SetupTasks()
        {
            TaskHelper.SetCompletionSources(N);
        }

        [IterationCleanup(Target = nameof(DotNetTask))]
        public void CleanupTasks()
        {
            TaskHelper.ClearCompletionSources();
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            TaskCompletionSource<object> deferred = new TaskCompletionSource<object>();
            var promise = deferred.Task;

            for (int i = 0; i < N; ++i)
            {
                int index = i;
                promise = promise
                    .ContinueWith(_ => (Task) TaskHelper.taskVoids[index].Task, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskVectors[index].Task, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskObjects[index].Task, TaskContinuationOptions.ExecuteSynchronously).Unwrap();
            }

            deferred.SetResult(Instances.obj);
            TaskHelper.ResolveCompletionSources();
            promise.Wait();
        }
    }

    partial class ContinueWithResolved
    {
        [GlobalSetup(Target = nameof(DotNetTask))]
        public void SetupTasks()
        {
            TaskHelper.SetTasks();
        }

        [GlobalCleanup(Target = nameof(DotNetTask))]
        public void CleanupTasks()
        {
            TaskHelper.ClearTasks();
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            TaskCompletionSource<object> deferred = new TaskCompletionSource<object>();
            var promise = deferred.Task;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(_ => TaskHelper.taskVoid, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskVector, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskObject, TaskContinuationOptions.ExecuteSynchronously).Unwrap();
            }

            deferred.SetResult(Instances.obj);
            promise.Wait();
        }
    }

    partial class ContinueWithFromValue
    {
        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            TaskCompletionSource<object> deferred = new TaskCompletionSource<object>();
            var promise = deferred.Task;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ => Instances.vector, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ => Instances.obj, TaskContinuationOptions.ExecuteSynchronously);
            }

            deferred.SetResult(Instances.obj);
            promise.Wait();
        }
    }

    partial class AwaitPending
    {
        [IterationSetup(Target = nameof(DotNetTask))]
        public void SetupTasks()
        {
            TaskHelper.SetCompletionSources(N);
        }

        [IterationCleanup(Target = nameof(DotNetTask))]
        public void CleanupTasks()
        {
            TaskHelper.ClearCompletionSources();
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            _ = AwaitTasks();
            TaskHelper.ResolveCompletionSources();
        }

        private async Task<object> AwaitTasks()
        {
            for (int i = 0; i < N; ++i)
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
        public void SetupTasks()
        {
            TaskHelper.SetTasks();
        }

        [GlobalCleanup(Target = nameof(DotNetTask))]
        public void CleanupTasks()
        {
            TaskHelper.ClearTasks();
        }

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            AwaitTasks().Wait();
        }

        private async Task<object> AwaitTasks()
        {
            for (int i = 0; i < N; ++i)
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
        private static Promise task_promise;
        private static long task_counter;

        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            // Create a promise to await so that the async functions won't complete synchronously.
            Promise.Deferred deferred = Promise.NewDeferred();
            task_promise = deferred.Promise;
            task_counter = 0L;

            for (int i = 0; i < N; ++i)
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

            deferred.Resolve();
            Promise.Manager.HandleCompletesAndProgress();
            while (Interlocked.Read(ref task_counter) > 0) { }
        }
    }

    partial class AsyncResolved
    {
        [Benchmark(Baseline = true)]
        public void DotNetTask()
        {
            for (int i = 0; i < N; ++i)
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