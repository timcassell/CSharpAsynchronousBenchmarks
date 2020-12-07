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

        public static UniTaskCompletionSource[] uniVoids;
        public static UniTaskCompletionSource<Vector4>[] uniVectors;
        public static UniTaskCompletionSource<object>[] uniObjects;

        public static void SetCompletionSources(int n)
        {
            if (uniVoids != null)
            {
                // Don't recreate completion sources.
                return;
            }

            uniVoids = new UniTaskCompletionSource[n];
            uniVectors = new UniTaskCompletionSource<Vector4>[n];
            uniObjects = new UniTaskCompletionSource<object>[n];
            for (int i = 0; i < n; ++i)
            {
                uniVoids[i] = new UniTaskCompletionSource();
                uniVectors[i] = new UniTaskCompletionSource<Vector4>();
                uniObjects[i] = new UniTaskCompletionSource<object>();
            }
        }

        public static void ClearCompletionSources()
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
        [GlobalSetup(Target = nameof(UniTask))]
        public void GlobalSetupUniTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            UniTaskHelper.SetCompletionSources(1);
            ExecuteUniTask(1);
            UniTaskHelper.ClearCompletionSources();
        }

        [IterationSetup(Target = nameof(UniTask))]
        public void IterationSetupUniTasks()
        {
            UniTaskHelper.SetCompletionSources(N + uniTask_additionalIterations);
        }

        [IterationCleanup(Target = nameof(UniTask))]
        public void IterationCleanupUniTasks()
        {
            UniTaskHelper.ClearCompletionSources();
        }

        // Can't clear UniTask's object pool, so just add an extra iteration to account for GlobalSetup (necessary for a fair survived memory measurement).
        int uniTask_additionalIterations = 1;

        [Benchmark]
        public void UniTask()
        {
            ExecuteUniTask(N + uniTask_additionalIterations);
            uniTask_additionalIterations = 0; // Only include additional iteration on the first run for the survived memory measurement.
        }

        private void ExecuteUniTask(int n)
        {
            UniTaskCompletionSource<object> deferred = new UniTaskCompletionSource<object>();
            var task = deferred.Task;

            for (int i = 0; i < n; ++i)
            {
                int index = i;
                task = task
                    .ContinueWith(_ => UniTaskHelper.uniVoids[index].Task)
                    .ContinueWith(() => UniTaskHelper.uniVectors[index].Task)
                    .ContinueWith(_ => UniTaskHelper.uniObjects[index].Task);
            }

            task.Forget();
            deferred.TrySetResult(Instances.obj);
            UniTaskHelper.ResolveCompletionSources();
        }
    }

    partial class ContinueWithResolved
    {
        [GlobalSetup(Target = nameof(UniTask))]
        public void GlobalSetupUniTasks()
        {
            UniTaskHelper.SetUniTasks();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteUniTask(1);
        }

        // Can't clear UniTask's object pool, so just add an extra iteration to account for GlobalSetup (necessary for a fair survived memory measurement).
        int additionalIterations = 1;

        [Benchmark]
        public void UniTask()
        {
            ExecuteUniTask(N + additionalIterations);
            additionalIterations = 0; // Only include additional iteration on the first run for the survived memory measurement.
        }

        private void ExecuteUniTask(int n)
        {
            UniTaskCompletionSource<object> deferred = new UniTaskCompletionSource<object>();
            var task = deferred.Task;

            for (int i = 0; i < n; ++i)
            {
                task = task
                    .ContinueWith(_ => UniTaskHelper.uniVoid)
                    .ContinueWith(() => UniTaskHelper.uniVector)
                    .ContinueWith(_ => UniTaskHelper.uniObject);
            }

            task.Forget();
            deferred.TrySetResult(Instances.obj);
        }
    }

    partial class ContinueWithFromValue
    {
        [GlobalSetup(Target = nameof(UniTask))]
        public void GlobalSetupUniTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteUniTask(1);
        }

        // Can't clear UniTask's object pool, so just add an extra iteration to account for GlobalSetup (necessary for a fair survived memory measurement).
        int additionalIterations = 1;

        [Benchmark]
        public void UniTask()
        {
            ExecuteUniTask(N + additionalIterations);
            additionalIterations = 0; // Only include additional iteration on the first run for the survived memory measurement.
        }

        private void ExecuteUniTask(int n)
        {
            UniTaskCompletionSource<object> deferred = new UniTaskCompletionSource<object>();
            var task = deferred.Task;

            for (int i = 0; i < n; ++i)
            {
                task = task
                    .ContinueWith(_ => { })
                    .ContinueWith(() => Instances.vector)
                    .ContinueWith(_ => Instances.obj);
            }

            task.Forget();
            deferred.TrySetResult(Instances.obj);
        }
    }

    partial class AwaitPending
    {
        [GlobalSetup(Target = nameof(UniTask))]
        public void GlobalSetupUniTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            UniTaskHelper.SetCompletionSources(1);
            ExecuteUniTask(1);
            UniTaskHelper.ClearCompletionSources();
        }

        [IterationSetup(Target = nameof(UniTask))]
        public void IterationSetupUniTasks()
        {
            UniTaskHelper.SetCompletionSources(N);
        }

        [IterationCleanup(Target = nameof(UniTask))]
        public void IterationCleanupUniTasks()
        {
            UniTaskHelper.ClearCompletionSources();
        }

        // Can't clear UniTask's object pool, so just add an extra iteration to account for GlobalSetup (necessary for a fair survived memory measurement).
        int additionalIterations = 1;

        [Benchmark]
        public void UniTask()
        {
            ExecuteUniTask(N + additionalIterations);
            additionalIterations = 0; // Only include additional iteration on the first run for the survived memory measurement.
        }

        private void ExecuteUniTask(int n)
        {
            _ = AwaitUniTasks(n);
            UniTaskHelper.ResolveCompletionSources();
        }

        private async Task<object> AwaitUniTasks(int n)
        {
            for (int i = 0; i < n; ++i)
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
        public void GlobalSetupUniTasks()
        {
            UniTaskHelper.SetUniTasks();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteUniTask(1);
        }

        // Can't clear UniTask's object pool, so just add an extra iteration to account for GlobalSetup (necessary for a fair survived memory measurement).
        int additionalIterations = 1;

        [Benchmark]
        public void UniTask()
        {
            ExecuteUniTask(N + additionalIterations);
            additionalIterations = 0; // Only include additional iteration on the first run for the survived memory measurement.
        }

        private void ExecuteUniTask(int n)
        {
            _ = AwaitUniTasks(n);
        }

        private async Task<object> AwaitUniTasks(int n)
        {
            for (int i = 0; i < n; ++i)
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
        private static Promise.Deferred uni_deferred;
        private static Promise uni_promise;
        private static long uni_counter;

        [GlobalSetup(Target = nameof(UniTask))]
        public void GlobalSetupUniTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            IterationSetupUniTasks();
            ExecuteUniTask(1);
        }

        [IterationSetup(Target = nameof(UniTask))]
        public void IterationSetupUniTasks()
        {
            // Create a promise to await so that the async functions won't complete synchronously.
            uni_deferred = Promise.NewDeferred();
            uni_promise = uni_deferred.Promise;
            uni_counter = 0L;
        }

        // Can't clear UniTask's object pool, so just add an extra iteration to account for GlobalSetup (necessary for a fair survived memory measurement).
        int additionalIterations = 0;

        [Benchmark]
        public void UniTask()
        {
            ExecuteUniTask(N + additionalIterations);
            additionalIterations = 0; // Only include additional iteration on the first run for the survived memory measurement.
        }

        private void ExecuteUniTask(int n)
        {
            for (int i = 0; i < n; ++i)
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

            uni_deferred.Resolve();
            Promise.Manager.HandleCompletesAndProgress();
            while (Interlocked.Read(ref uni_counter) > 0) { }
        }
    }

    partial class AsyncResolved
    {
        [GlobalSetup(Target = nameof(UniTask))]
        public void GlobalSetupUniTasks()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteUniTask(1);
        }

        // Can't clear UniTask's object pool, so just add an extra iteration to account for GlobalSetup (necessary for a fair survived memory measurement).
        int additionalIterations = 1;

        [Benchmark]
        public void UniTask()
        {
            ExecuteUniTask(N + additionalIterations);
            additionalIterations = 0; // Only include additional iteration on the first run for the survived memory measurement.
        }

        private void ExecuteUniTask(int n)
        {
            for (int i = 0; i < n; ++i)
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