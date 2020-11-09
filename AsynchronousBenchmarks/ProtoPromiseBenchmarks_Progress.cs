//
//
// Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638
//
//

//extern alias ProtoPromise_With_Progress;

//using ProtoPromise_With_Progress::Proto.Promises;
//using Helper;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ProtoPromise_Progress
//{
//    internal static class ProtoPromiseHelper
//    {
//        public static Promise protoVoid;
//        public static Promise<Vector4> protoVector;
//        public static Promise<object> protoObject;

//        public static void SetProtoPromises()
//        {
//            var protoVoidSource = Promise.NewDeferred();
//            var protoVectorSource = Promise.NewDeferred<Vector4>();
//            var protoObjectSource = Promise.NewDeferred<object>();
//            protoVoid = protoVoidSource.Promise;
//            protoVector = protoVectorSource.Promise;
//            protoObject = protoObjectSource.Promise;
//            protoVoid.Retain();
//            protoVector.Retain();
//            protoObject.Retain();
//            protoVoidSource.Resolve();
//            protoVectorSource.Resolve(Instances.vector);
//            protoObjectSource.Resolve(Instances.obj);
//        }

//        public static void ClearProtoPromises()
//        {
//            protoVoid.Release();
//            protoVoid = default;
//            protoVector.Release();
//            protoVector = default;
//            protoObject.Release();
//            protoObject = default;
//        }

//        public static Promise.Deferred[] protoVoids;
//        public static Promise<Vector4>.Deferred[] protoVectors;
//        public static Promise<object>.Deferred[] protoObjects;

//        public static void SetDeferreds(int N)
//        {
//            if (protoVoids != null)
//            {
//                // Don't recreate deferreds. This is necessary because this is ran separately for the JIT optimizer.
//                return;
//            }

//            protoVoids = new Promise.Deferred[N];
//            protoVectors = new Promise<Vector4>.Deferred[N];
//            protoObjects = new Promise<object>.Deferred[N];
//            for (int i = 0; i < N; ++i)
//            {
//                protoVoids[i] = Promise.NewDeferred();
//                protoVectors[i] = Promise.NewDeferred<Vector4>();
//                protoObjects[i] = Promise.NewDeferred<object>();
//            }
//        }

//        public static void ClearDeferreds()
//        {
//            protoVoids = default;
//            protoVectors = default;
//            protoObjects = default;
//        }

//        public static void ResolveDeferreds()
//        {
//            for (int i = 0, max = protoVoids.Length; i < max; ++i)
//            {
//                protoVoids[i].Resolve();
//                Promise.Manager.HandleCompletesAndProgress(); // Move async func to next await/Resolve next ContinueWith before resolving the next deferred.
//                protoVectors[i].Resolve(Instances.vector);
//                Promise.Manager.HandleCompletesAndProgress(); // Move async func to next await/Resolve next ContinueWith before resolving the next deferred.
//                protoObjects[i].Resolve(Instances.obj);
//                Promise.Manager.HandleCompletesAndProgress(); // Move async func to next await/Resolve next ContinueWith before resolving the next deferred.
//            }
//        }
//    }

//    public static class ContinueWithPending
//    {
//        private static int N;

//        public static void Setup(int N)
//        {
//            ContinueWithPending.N = N;
//            ProtoPromiseHelper.SetDeferreds(N);
//        }

//        public static void Cleanup()
//        {
//            ProtoPromiseHelper.ClearDeferreds();
//        }

//        public static void ExecuteWithoutPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            Execute();
//        }

//        public static void ExecuteWithPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            Execute();
//        }

//        private static void Execute()
//        {
//            Promise<object>.Deferred deferred = Promise.NewDeferred<object>();
//            var promise = deferred.Promise;

//            for (int i = 0; i < N; ++i)
//            {
//                promise = promise
//                    .ContinueWith(i, (index, _) => ProtoPromiseHelper.protoVoids[index].Promise)
//                    .ContinueWith(i, (index, _) => ProtoPromiseHelper.protoVectors[index].Promise)
//                    .ContinueWith(i, (index, _) => ProtoPromiseHelper.protoObjects[index].Promise);
//            }

//            deferred.Resolve(Instances.obj);
//            Promise.Manager.HandleCompletesAndProgress();
//            ProtoPromiseHelper.ResolveDeferreds();
//        }
//    }

//    public static class ContinueWithResolved
//    {
//        private static int N;

//        public static void Setup(int N)
//        {
//            ContinueWithResolved.N = N;
//            ProtoPromiseHelper.SetProtoPromises();
//        }

//        public static void Cleanup()
//        {
//            ProtoPromiseHelper.ClearProtoPromises();
//        }

//        public static void ExecuteWithoutPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            Execute();
//        }

//        public static void ExecuteWithPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            Execute();
//        }

//        private static void Execute()
//        {
//            Promise<object>.Deferred deferred = Promise.NewDeferred<object>();
//            var promise = deferred.Promise;

//            for (int i = 0; i < N; ++i)
//            {
//                promise = promise
//                    .ContinueWith(_ => ProtoPromiseHelper.protoVoid)
//                    .ContinueWith(_ => ProtoPromiseHelper.protoVector)
//                    .ContinueWith(_ => ProtoPromiseHelper.protoObject);
//            }

//            deferred.Resolve(Instances.obj);
//            Promise.Manager.HandleCompletesAndProgress();
//        }
//    }

//    public static class ContinueWithFromValue
//    {
//        private static int N;

//        public static void Setup(int N)
//        {
//            ContinueWithFromValue.N = N;
//        }

//        public static void Cleanup()
//        {
//        }

//        public static void ExecuteWithoutPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            Execute();
//        }

//        public static void ExecuteWithPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            Execute();
//        }

//        private static void Execute()
//        {
//            Promise<object>.Deferred deferred = Promise.NewDeferred<object>();
//            var promise = deferred.Promise;

//            for (int i = 0; i < N; ++i)
//            {
//                promise = promise
//                    .ContinueWith(_ => { })
//                    .ContinueWith(_ => Instances.vector)
//                    .ContinueWith(_ => Instances.obj);
//            }

//            deferred.Resolve(Instances.obj);
//            Promise.Manager.HandleCompletesAndProgress();
//        }
//    }

//    public static class AwaitPending
//    {
//        private static int N;

//        public static void Setup(int N)
//        {
//            AwaitPending.N = N;
//            ProtoPromiseHelper.SetDeferreds(N);
//        }

//        public static void Cleanup()
//        {
//            ProtoPromiseHelper.ClearDeferreds();
//        }

//        public static void ExecuteWithoutPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            var task = Execute();
//            ProtoPromiseHelper.ResolveDeferreds();
//            task.Wait();
//        }

//        public static void ExecuteWithPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            var task = Execute();
//            ProtoPromiseHelper.ResolveDeferreds();
//            task.Wait();
//        }

//        private static async Task<object> Execute()
//        {
//            for (int i = 0; i < N; ++i)
//            {
//                await ProtoPromiseHelper.protoVoids[i].Promise;
//                _ = await ProtoPromiseHelper.protoVectors[i].Promise;
//                _ = await ProtoPromiseHelper.protoObjects[i].Promise;
//            }
//            return Instances.obj;
//        }
//    }

//    public static class AwaitResolved
//    {
//        private static int N;

//        public static void Setup(int N)
//        {
//            AwaitResolved.N = N;
//            ProtoPromiseHelper.SetProtoPromises();
//        }

//        public static void Cleanup()
//        {
//            ProtoPromiseHelper.ClearProtoPromises();
//        }

//        public static void ExecuteWithoutPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            _ = Execute();
//            Promise.Manager.HandleCompletesAndProgress();
//        }

//        public static void ExecuteWithPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            _ = Execute();
//            Promise.Manager.HandleCompletesAndProgress();
//        }

//        private static async Task<object> Execute()
//        {
//            for (int i = 0; i < N; ++i)
//            {
//                await ProtoPromiseHelper.protoVoid;
//                _ = await ProtoPromiseHelper.protoVector;
//                _ = await ProtoPromiseHelper.protoObject;
//            }
//            return Instances.obj;
//        }
//    }

//    public static class AsyncPending
//    {
//        private static int N;

//        public static void Setup(int N)
//        {
//            AsyncPending.N = N;
//        }

//        public static void Cleanup()
//        {

//        }

//        public static void ExecuteWithoutPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            Execute();
//        }

//        public static void ExecuteWithPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            Execute();
//        }

//        private static Promise promise;
//        private static long counter;

//        private static void Execute()
//        {
//            // Create a promise to await so that the async functions won't complete synchronously.
//            Promise.Deferred deferred = Promise.NewDeferred();
//            promise = deferred.Promise;
//            counter = 0L;

//            for (int i = 0; i < N; ++i)
//            {
//                _ = PromiseVoid();
//                _ = PromiseVector();
//                _ = PromiseObject();
//            }

//            async Promise PromiseVoid()
//            {
//                Interlocked.Increment(ref counter);
//                await promise;
//                Interlocked.Decrement(ref counter);
//            }

//            async Promise<Vector4> PromiseVector()
//            {
//                Interlocked.Increment(ref counter);
//                await promise;
//                Interlocked.Decrement(ref counter);
//                return Instances.vector;
//            }

//            async Promise<object> PromiseObject()
//            {
//                Interlocked.Increment(ref counter);
//                await promise;
//                Interlocked.Decrement(ref counter);
//                return Instances.obj;
//            }

//            deferred.Resolve();
//            Promise.Manager.HandleCompletesAndProgress();
//            while (Interlocked.Read(ref counter) > 0) { }
//        }
//    }

//    public static class AsyncResolved
//    {
//        private static int N;

//        public static void Setup(int N)
//        {
//            AsyncResolved.N = N;
//        }

//        public static void Cleanup()
//        {

//        }

//        public static void ExecuteWithoutPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            Execute();
//        }

//        public static void ExecuteWithPool()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            Execute();
//        }

//        private static void Execute()
//        {
//            for (int i = 0; i < N; ++i)
//            {
//                _ = TaskVoid();
//                _ = TaskVector();
//                _ = TaskObject();
//            }

//#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
//            async Promise TaskVoid()
//            {
//            }

//            async Promise<Vector4> TaskVector()
//            {
//                return Instances.vector;
//            }

//            async Promise<object> TaskObject()
//            {
//                return Instances.obj;
//            }

//            Promise.Manager.HandleCompletesAndProgress();
//#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
//        }
//    }
//}