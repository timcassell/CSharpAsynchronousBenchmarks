using BenchmarkDotNet.Attributes;
using Cysharp.Threading.Tasks;
using Proto.Promises;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousBenchmarks
{
    static class UniTaskHelper
    {
        public static UniTask uniVoid;
        public static UniTask<Vector4> uniVector;
        public static UniTask<object> uniObject;

        public static void SetUniTasks()
        {
            var uniVoidSource = new UniTaskCompletionSource();
            var uniVectorSource = new UniTaskCompletionSource<Vector4>();
            var uniObjectSource = new UniTaskCompletionSource<object>();
            // UniTask documentation says completion source can be awaited multiple times, so no need to preserve.
            uniVoid = uniVoidSource.Task;//.Preserve();
            uniVector = uniVectorSource.Task;//.Preserve();
            uniObject = uniObjectSource.Task;//.Preserve();
            uniVoidSource.TrySetResult();
            uniVectorSource.TrySetResult(Program.vector);
            uniObjectSource.TrySetResult(Program.obj);
        }

        public static void ClearUniTasks()
        {
            uniVoid = default;
            uniVector = default;
            uniObject = default;
        }
    }

    partial class ContinuationBenchmarks
    {
        [GlobalSetup(Target = nameof(UniTask))]
        public void SetupUniTasks()
        {
            UniTaskHelper.SetUniTasks();
        }

        [GlobalCleanup(Target = nameof(UniTask))]
        public void CleanupUniTasks()
        {
            UniTaskHelper.ClearUniTasks();
        }

        [Benchmark]
        public void UniTask()
        {
            UniTaskCompletionSource<object> deferred = new UniTaskCompletionSource<object>();
            var promise = deferred.Task;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(_ => { })
                    .ContinueWith(() => Program.vector)
                    .ContinueWith(_ => Program.obj)
                    .ContinueWith(_ => UniTaskHelper.uniVoid)
                    .ContinueWith(() => UniTaskHelper.uniVector)
                    .ContinueWith(_ => UniTaskHelper.uniObject);
            }

            promise.Forget();
            deferred.TrySetResult(Program.obj);
        }
    }

    partial class AwaitBenchmarks
    {
        [GlobalSetup(Target = nameof(UniTask))]
        public void SetupUniTasks()
        {
            UniTaskHelper.SetUniTasks();
        }

        [GlobalCleanup(Target = nameof(UniTask))]
        public void CleanupUniTasks()
        {
            UniTaskHelper.ClearUniTasks();
        }

        [Benchmark]
        public void UniTask()
        {
            _ = AwaitUniTasks();
        }

        private async Task<object> AwaitUniTasks()
        {
            for (int i = 0; i < N; ++i)
            {
                await UniTaskHelper.uniVoid;
                _ = await UniTaskHelper.uniVector;
                _ = await UniTaskHelper.uniObject;
            }
            return Program.obj;
        }
    }

    partial class AsyncBenchmarks
    {
        [Benchmark]
        public void UniTask()
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            // Create a promise to await so that the async functions won't complete synchronously.
            Promise.Deferred deferred = Promise.NewDeferred();
            Promise promise = deferred.Promise;
            long counter = 0L;

            for (int i = 0; i < N; ++i)
            {
                // Forget so that the objects will repool.
                TaskVoid().Forget();
                TaskVector().Forget();
                TaskObject().Forget();
            }

            async UniTask TaskVoid()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
            }

            async UniTask<Vector4> TaskVector()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
                return Program.vector;
            }

            async UniTask<object> TaskObject()
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