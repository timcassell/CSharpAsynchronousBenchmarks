//using BenchmarkDotNet.Attributes;
//using Proto.Promises;
//using System.Threading;
//using System.Threading.Tasks;

//namespace AsynchronousBenchmarks
//{
//    static class ProtoPromiseHelper
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
//            protoVectorSource.Resolve(Program.vector);
//            protoObjectSource.Resolve(Program.obj);
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
//    }

//    partial class ContinuationBenchmarks
//    {
//        [GlobalSetup(Targets = new[] { nameof(Proto_N), nameof(Proto_A) })]
//        public void SetupProtoPromises()
//        {
//            ProtoPromiseHelper.SetProtoPromises();
//        }

//        [GlobalCleanup(Targets = new[] { nameof(Proto_N), nameof(Proto_A) })]
//        public void CleanupProtoPromises()
//        {
//            ProtoPromiseHelper.ClearProtoPromises();
//        }

//        [Benchmark]
//        public void Proto_N()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            Proto();
//        }

//        [Benchmark]
//        public void Proto_A()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            Proto();
//        }

//        private void Proto()
//        {
//            Promise<object>.Deferred deferred = Promise.NewDeferred<object>();
//            var promise = deferred.Promise;

//            for (int i = 0; i < N; ++i)
//            {
//                promise = promise
//                    .ContinueWith(_ => { })
//                    .ContinueWith(_ => Program.vector)
//                    .ContinueWith(_ => Program.obj)
//                    .ContinueWith(_ => ProtoPromiseHelper.protoVoid)
//                    .ContinueWith(_ => ProtoPromiseHelper.protoVector)
//                    .ContinueWith(_ => ProtoPromiseHelper.protoObject);
//            }

//            deferred.Resolve(Program.obj);
//            Promise.Manager.HandleCompletesAndProgress();
//        }
//    }

//    partial class AwaitBenchmarks
//    {
//        [GlobalSetup(Targets = new[] { nameof(Proto_N), nameof(Proto_A) })]
//        public void SetupProtoPromises()
//        {
//            ProtoPromiseHelper.SetProtoPromises();
//        }

//        [GlobalCleanup(Targets = new[] { nameof(Proto_N), nameof(Proto_A) })]
//        public void CleanupProtoPromises()
//        {
//            ProtoPromiseHelper.ClearProtoPromises();
//        }

//        [Benchmark]
//        public void Proto_N()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            _ = Proto();
//            Promise.Manager.HandleCompletesAndProgress();
//        }

//        [Benchmark]
//        public void Proto_A()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            _ = Proto();
//            Promise.Manager.HandleCompletesAndProgress();
//        }

//        private async Task<object> Proto()
//        {
//            for (int i = 0; i < N; ++i)
//            {
//                await ProtoPromiseHelper.protoVoid;
//                _ = await ProtoPromiseHelper.protoVector;
//                _ = await ProtoPromiseHelper.protoObject;
//            }
//            return Program.obj;
//        }
//    }

//    partial class AsyncBenchmarks
//    {
//        [Benchmark]
//        public void Proto_N()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.None;
//            Proto();
//        }

//        [Benchmark]
//        public void Proto_A()
//        {
//            Promise.Config.ObjectPooling = Promise.PoolType.All;
//            Proto();
//        }

//        private void Proto()
//        {
//            // Create a promise to await so that the async functions won't complete synchronously.
//            Promise.Deferred deferred = Promise.NewDeferred();
//            Promise promise = deferred.Promise;
//            long counter = 0L;

//            for (int i = 0; i < N; ++i)
//            {
//                _ = TaskVoid();
//                _ = TaskVector();
//                _ = TaskObject();
//            }

//            async Promise TaskVoid()
//            {
//                Interlocked.Increment(ref counter);
//                await promise;
//                Interlocked.Decrement(ref counter);
//            }

//            async Promise<Vector4> TaskVector()
//            {
//                Interlocked.Increment(ref counter);
//                await promise;
//                Interlocked.Decrement(ref counter);
//                return Program.vector;
//            }

//            async Promise<object> TaskObject()
//            {
//                Interlocked.Increment(ref counter);
//                await promise;
//                Interlocked.Decrement(ref counter);
//                return Program.obj;
//            }

//            deferred.Resolve();
//            Promise.Manager.HandleCompletesAndProgress();
//            while (Interlocked.Read(ref counter) > 0) { }
//        }
//    }
//}