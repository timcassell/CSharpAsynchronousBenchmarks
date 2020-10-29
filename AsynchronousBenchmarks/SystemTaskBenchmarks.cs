using BenchmarkDotNet.Attributes;
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
            // Set task objects from completion source to more accurately replicate real-world scenarios.
            var taskVoidSource = new TaskCompletionSource<bool>();
            var taskVectorSource = new TaskCompletionSource<Vector4>();
            var taskObjectSource = new TaskCompletionSource<object>();
            taskVoid = taskVoidSource.Task;
            taskVector = taskVectorSource.Task;
            taskObject = taskObjectSource.Task;
            taskVoidSource.SetResult(true);
            taskVectorSource.SetResult(Program.vector);
            taskObjectSource.SetResult(Program.obj);
        }

        public static void ClearTasks()
        {
            taskVoid = default;
            taskVector = default;
            taskObject = default;
        }
    }

    partial class ContinuationBenchmarks
    {
        [GlobalSetup(Target = nameof(SystemTask))]
        public void SetupTasks()
        {
            TaskHelper.SetTasks();
        }

        [GlobalCleanup(Target = nameof(SystemTask))]
        public void CleanupTasks()
        {
            TaskHelper.ClearTasks();
        }

        [Benchmark(Baseline = true)]
        public void SystemTask()
        {
            TaskCompletionSource<object> deferred = new TaskCompletionSource<object>();
            var promise = deferred.Task;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ => Program.vector, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ => Program.obj, TaskContinuationOptions.ExecuteSynchronously)
                    .ContinueWith(_ => TaskHelper.taskVoid, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskVector, TaskContinuationOptions.ExecuteSynchronously).Unwrap()
                    .ContinueWith(_ => TaskHelper.taskObject, TaskContinuationOptions.ExecuteSynchronously).Unwrap();
            }

            deferred.SetResult(Program.obj);
            promise.Wait();
        }
    }

    partial class AwaitBenchmarks
    {

        [GlobalSetup(Target = nameof(SystemTask))]
        public void SetupTasks()
        {
            TaskHelper.SetTasks();
        }

        [GlobalCleanup(Target = nameof(SystemTask))]
        public void CleanupTasks()
        {
            TaskHelper.ClearTasks();
        }

        [Benchmark(Baseline = true)]
        public void SystemTask()
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
            return Program.obj;
        }
    }

    partial class AsyncBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void SystemTask()
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            // Create a promise to await so that the async functions won't complete synchronously.
            Promise.Deferred deferred = Promise.NewDeferred();
            Promise promise = deferred.Promise;
            long counter = 0L;

            for (int i = 0; i < N; ++i)
            {
                _ = TaskVoid();
                _ = TaskVector();
                _ = TaskObject();
            }

            async Task TaskVoid()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
            }

            async Task<Vector4> TaskVector()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
                return Program.vector;
            }

            async Task<object> TaskObject()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
                return Program.obj;
            }

            deferred.Resolve();
            Promise.Manager.HandleCompletesAndProgress();
            while (Interlocked.Read(ref counter) > 0) { }
        }
    }
}