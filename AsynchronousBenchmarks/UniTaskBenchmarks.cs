using BenchmarkDotNet.Attributes;
using Cysharp.Threading.Tasks;
using Helper;
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
            uniVectorSource.TrySetResult(Instances.vector);
            uniObjectSource.TrySetResult(Instances.obj);
        }

        public static void ClearUniTasks()
        {
            uniVoid = default;
            uniVector = default;
            uniObject = default;
        }

        public static UniTaskCompletionSource[] uniVoids;
        public static UniTaskCompletionSource<Vector4>[] uniVectors;
        public static UniTaskCompletionSource<object>[] uniObjects;

        public static void SetContinuationSources(int N)
        {
            if (uniVoids != null)
            {
                // Don't recreate completion sources. This is necessary because this is ran separately for the JIT optimizer.
                return;
            }

            uniVoids = new UniTaskCompletionSource[N];
            uniVectors = new UniTaskCompletionSource<Vector4>[N];
            uniObjects = new UniTaskCompletionSource<object>[N];
            for (int i = 0; i < N; ++i)
            {
                uniVoids[i] = new UniTaskCompletionSource();
                uniVectors[i] = new UniTaskCompletionSource<Vector4>();
                uniObjects[i] = new UniTaskCompletionSource<object>();
            }
        }

        public static void ClearContinuationSources()
        {
            uniVoids = default;
            uniVectors = default;
            uniObjects = default;
        }

        public static void ResolveCompletionSources()
        {
            for (int i = 0, max = uniVoids.Length; i < max; ++i)
            {
                uniVoids[i].TrySetResult();
                uniVectors[i].TrySetResult(Instances.vector);
                uniObjects[i].TrySetResult(Instances.obj);
            }
        }
    }

    partial class ContinueWithPending
    {
        [IterationSetup(Target = nameof(UniTask))]
        public void SetupUniTasks()
        {
            UniTaskHelper.SetContinuationSources(N);
        }

        [IterationCleanup(Target = nameof(UniTask))]
        public void CleanupUniTasks()
        {
            UniTaskHelper.ClearContinuationSources();
        }

        [Benchmark]
        public void UniTask()
        {
            UniTaskCompletionSource<object> deferred = new UniTaskCompletionSource<object>();
            var promise = deferred.Task;

            for (int i = 0; i < N; ++i)
            {
                int index = i;
                promise = promise
                    .ContinueWith(_ => UniTaskHelper.uniVoids[index].Task)
                    .ContinueWith(() => UniTaskHelper.uniVectors[index].Task)
                    .ContinueWith(_ => UniTaskHelper.uniObjects[index].Task);
            }

            promise.Forget();
            deferred.TrySetResult(Instances.obj);
            UniTaskHelper.ResolveCompletionSources();
        }
    }

    partial class ContinueWithResolved
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
                    .ContinueWith(_ => UniTaskHelper.uniVoid)
                    .ContinueWith(() => UniTaskHelper.uniVector)
                    .ContinueWith(_ => UniTaskHelper.uniObject);
            }

            promise.Forget();
            deferred.TrySetResult(Instances.obj);
        }
    }

    partial class ContinueWithFromValue
    {
        [Benchmark]
        public void UniTask()
        {
            UniTaskCompletionSource<object> deferred = new UniTaskCompletionSource<object>();
            var promise = deferred.Task;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(_ => { })
                    .ContinueWith(() => Instances.vector)
                    .ContinueWith(_ => Instances.obj);
            }

            promise.Forget();
            deferred.TrySetResult(Instances.obj);
        }
    }

    partial class AwaitPending
    {
        [IterationSetup(Target = nameof(UniTask))]
        public void SetupUniTasks()
        {
            UniTaskHelper.SetContinuationSources(N);
        }

        [IterationCleanup(Target = nameof(UniTask))]
        public void CleanupUniTasks()
        {
            UniTaskHelper.ClearContinuationSources();
        }

        [Benchmark]
        public void UniTask()
        {
            _ = AwaitUniTasks();
            UniTaskHelper.ResolveCompletionSources();
        }

        private async Task<object> AwaitUniTasks()
        {
            for (int i = 0; i < N; ++i)
            {
                await UniTaskHelper.uniVoid;
                _ = await UniTaskHelper.uniVector;
                _ = await UniTaskHelper.uniObject;
            }
            return Instances.obj;
        }
    }

    partial class AwaitResolved
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
            return Instances.obj;
        }
    }

    partial class AsyncPending
    {
        private static Promise uni_promise;
        private static long uni_counter;

        [Benchmark]
        public void UniTask()
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            // Create a promise to await so that the async functions won't complete synchronously.
            Promise.Deferred deferred = Promise.NewDeferred();
            uni_promise = deferred.Promise;
            uni_counter = 0L;

            for (int i = 0; i < N; ++i)
            {
                // Forget so that the objects will repool.
                UniTaskVoid().Forget();
                UniTaskVector().Forget();
                UniTaskObject().Forget();
            }

            async UniTask UniTaskVoid()
            {
                Interlocked.Increment(ref uni_counter);
                await uni_promise;
                Interlocked.Decrement(ref uni_counter);
            }

            async UniTask<Vector4> UniTaskVector()
            {
                Interlocked.Increment(ref uni_counter);
                await uni_promise;
                Interlocked.Decrement(ref uni_counter);
                return Instances.vector;
            }

            async UniTask<object> UniTaskObject()
            {
                Interlocked.Increment(ref uni_counter);
                await uni_promise;
                Interlocked.Decrement(ref uni_counter);
                return Instances.obj;
            }

            deferred.Resolve();
            Promise.Manager.HandleCompletesAndProgress();
            while (Interlocked.Read(ref uni_counter) > 0) { }
        }
    }

    partial class AsyncResolved
    {
        [Benchmark]
        public void UniTask()
        {
            for (int i = 0; i < N; ++i)
            {
                // Forget so that the objects will repool.
                TaskVoid().Forget();
                TaskVector().Forget();
                TaskObject().Forget();
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            async UniTask TaskVoid()
            {
            }

            async UniTask<Vector4> TaskVector()
            {
                return Instances.vector;
            }

            async UniTask<object> TaskObject()
            {
                return Instances.obj;
            }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }
    }
}