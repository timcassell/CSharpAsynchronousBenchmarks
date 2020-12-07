// Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638
//extern alias ProtoPromise_Without_Progress;

//using ProtoPromise_Without_Progress::Proto.Promises;
using Proto.Promises;
using Helper;
using System.Threading;
using System.Threading.Tasks;

namespace ProtoPromise_NoProgress
{
    internal static class ProtoPromiseHelper
    {
        public static Promise protoVoid;
        public static Promise<Vector4> protoVector;
        public static Promise<object> protoObject;

        public static void SetProtoPromises()
        {
            var protoVoidSource = Promise.NewDeferred();
            var protoVectorSource = Promise.NewDeferred<Vector4>();
            var protoObjectSource = Promise.NewDeferred<object>();
            protoVoid = protoVoidSource.Promise;
            protoVector = protoVectorSource.Promise;
            protoObject = protoObjectSource.Promise;
            protoVoid.Retain();
            protoVector.Retain();
            protoObject.Retain();
            protoVoidSource.Resolve();
            protoVectorSource.Resolve(Instances.vector);
            protoObjectSource.Resolve(Instances.obj);
        }

        public static Promise.Deferred[] protoVoids;
        public static Promise<Vector4>.Deferred[] protoVectors;
        public static Promise<object>.Deferred[] protoObjects;

        public static void SetDeferreds(int N)
        {
            if (protoVoids != null)
            {
                // Don't recreate deferreds.
                return;
            }

            protoVoids = new Promise.Deferred[N];
            protoVectors = new Promise<Vector4>.Deferred[N];
            protoObjects = new Promise<object>.Deferred[N];
            for (int i = 0; i < N; ++i)
            {
                protoVoids[i] = Promise.NewDeferred();
                protoVectors[i] = Promise.NewDeferred<Vector4>();
                protoObjects[i] = Promise.NewDeferred<object>();
            }
        }

        public static void ClearDeferreds()
        {
            protoVoids = default;
            protoVectors = default;
            protoObjects = default;
        }

        public static void ResolveDeferreds()
        {
            for (int i = 0, max = protoVoids.Length; i < max; ++i)
            {
                protoVoids[i].Resolve();
                Promise.Manager.HandleCompletesAndProgress(); // Move async func to next await/Resolve next ContinueWith before resolving the next deferred.
                protoVectors[i].Resolve(Instances.vector);
                Promise.Manager.HandleCompletesAndProgress(); // Move async func to next await/Resolve next ContinueWith before resolving the next deferred.
                protoObjects[i].Resolve(Instances.obj);
                Promise.Manager.HandleCompletesAndProgress(); // Move async func to next await/Resolve next ContinueWith before resolving the next deferred.
            }
        }
    }

    public static class ContinueWithPending
    {
        public static void GlobalSetup()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            IterationSetup(1);
            ExecuteWithoutPool(1);
            IterationCleanup();
        }

        public static void IterationSetup(int N)
        {
            ProtoPromiseHelper.SetDeferreds(N);
        }

        public static void IterationCleanup()
        {
            ProtoPromiseHelper.ClearDeferreds();
        }

        public static void ExecuteWithoutPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.None;
            Execute(N);
        }

        public static void ExecuteWithPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            Execute(N);
        }

        private static void Execute(int N)
        {
            Promise<object>.Deferred deferred = Promise.NewDeferred<object>();
            var promise = deferred.Promise;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(i, (index, _) => ProtoPromiseHelper.protoVoids[index].Promise)
                    .ContinueWith(i, (index, _) => ProtoPromiseHelper.protoVectors[index].Promise)
                    .ContinueWith(i, (index, _) => ProtoPromiseHelper.protoObjects[index].Promise);
            }

            deferred.Resolve(Instances.obj);
            Promise.Manager.HandleCompletesAndProgress();
            ProtoPromiseHelper.ResolveDeferreds();
        }
    }

    public static class ContinueWithResolved
    {
        public static void GlobalSetup()
        {
            ProtoPromiseHelper.SetProtoPromises();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteWithoutPool(1);
        }

        public static void ExecuteWithoutPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.None;
            Execute(N);
        }

        public static void ExecuteWithPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            Execute(N);
        }

        private static void Execute(int N)
        {
            Promise<object>.Deferred deferred = Promise.NewDeferred<object>();
            var promise = deferred.Promise;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(_ => ProtoPromiseHelper.protoVoid)
                    .ContinueWith(_ => ProtoPromiseHelper.protoVector)
                    .ContinueWith(_ => ProtoPromiseHelper.protoObject);
            }

            deferred.Resolve(Instances.obj);
            Promise.Manager.HandleCompletesAndProgress();
        }
    }

    public static class ContinueWithFromValue
    {
        public static void GlobalSetup()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteWithoutPool(1);
        }

        public static void ExecuteWithoutPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.None;
            Execute(N);
        }

        public static void ExecuteWithPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            Execute(N);
        }

        private static void Execute(int N)
        {
            Promise<object>.Deferred deferred = Promise.NewDeferred<object>();
            var promise = deferred.Promise;

            for (int i = 0; i < N; ++i)
            {
                promise = promise
                    .ContinueWith(_ => { })
                    .ContinueWith(_ => Instances.vector)
                    .ContinueWith(_ => Instances.obj);
            }

            deferred.Resolve(Instances.obj);
            Promise.Manager.HandleCompletesAndProgress();
        }
    }

    public static class AwaitPending
    {
        public static void GlobalSetup()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ProtoPromiseHelper.SetDeferreds(1);
            ExecuteWithoutPool(1);
            ProtoPromiseHelper.ClearDeferreds();
        }

        public static void IterationSetup(int N)
        {
            ProtoPromiseHelper.SetDeferreds(N);
        }

        public static void IterationCleanup()
        {
            ProtoPromiseHelper.ClearDeferreds();
        }

        public static void ExecuteWithoutPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.None;
            var task = Execute(N);
            ProtoPromiseHelper.ResolveDeferreds();
            task.Wait();
        }

        public static void ExecuteWithPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            var task = Execute(N);
            ProtoPromiseHelper.ResolveDeferreds();
            task.Wait();
        }

        private static async Task<object> Execute(int N)
        {
            for (int i = 0; i < N; ++i)
            {
                await ProtoPromiseHelper.protoVoids[i].Promise;
                _ = await ProtoPromiseHelper.protoVectors[i].Promise;
                _ = await ProtoPromiseHelper.protoObjects[i].Promise;
            }
            return Instances.obj;
        }
    }

    public static class AwaitResolved
    {
        public static void GlobalSetup()
        {
            ProtoPromiseHelper.SetProtoPromises();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteWithoutPool(1);
        }

        public static void ExecuteWithoutPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.None;
            _ = Execute(N);
            Promise.Manager.HandleCompletesAndProgress();
        }

        public static void ExecuteWithPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            _ = Execute(N);
            Promise.Manager.HandleCompletesAndProgress();
        }

        private static async Task<object> Execute(int N)
        {
            for (int i = 0; i < N; ++i)
            {
                await ProtoPromiseHelper.protoVoid;
                _ = await ProtoPromiseHelper.protoVector;
                _ = await ProtoPromiseHelper.protoObject;
            }
            return Instances.obj;
        }
    }

    public static class AsyncPending
    {
        private static Promise.Deferred deferred;
        private static Promise promise;
        private static long counter;

        public static void GlobalSetup()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            IterationSetup();
            ExecuteWithoutPool(1);
        }

        public static void IterationSetup()
        {
            // Create a promise to await so that the async functions won't complete synchronously.
            deferred = Promise.NewDeferred();
            promise = deferred.Promise;
            counter = 0L;
        }

        public static void ExecuteWithoutPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.None;
            Execute(N);
        }

        public static void ExecuteWithPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            Execute(N);
        }

        private static void Execute(int N)
        {
            for (int i = 0; i < N; ++i)
            {
                _ = PromiseVoid();
                _ = PromiseVector();
                _ = PromiseObject();
            }

            async Promise PromiseVoid()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
            }

            async Promise<Vector4> PromiseVector()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
                return Instances.vector;
            }

            async Promise<object> PromiseObject()
            {
                Interlocked.Increment(ref counter);
                await promise;
                Interlocked.Decrement(ref counter);
                return Instances.obj;
            }

            deferred.Resolve();
            Promise.Manager.HandleCompletesAndProgress();
            while (Interlocked.Read(ref counter) > 0) { }
        }
    }

    public static class AsyncResolved
    {
        public static void GlobalSetup()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteWithoutPool(1);
        }

        public static void ExecuteWithoutPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.None;
            Execute(N);
        }

        public static void ExecuteWithPool(int N)
        {
            Promise.Config.ObjectPooling = Promise.PoolType.All;
            Execute(N);
        }

        private static void Execute(int N)
        {
            for (int i = 0; i < N; ++i)
            {
                _ = TaskVoid();
                _ = TaskVector();
                _ = TaskObject();
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            async Promise TaskVoid()
            {
            }

            async Promise<Vector4> TaskVector()
            {
                return Instances.vector;
            }

            async Promise<object> TaskObject()
            {
                return Instances.obj;
            }

            Promise.Manager.HandleCompletesAndProgress();
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }
    }
}