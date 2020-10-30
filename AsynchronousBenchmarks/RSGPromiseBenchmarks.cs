using BenchmarkDotNet.Attributes;
using RSG;

namespace AsynchronousBenchmarks
{
    static class RSGPromiseHelper
    {
        public static IPromise rsgVoid;
        public static IPromise<Vector4> rsgVector;
        public static IPromise<object> rsgObject;

        public static void SetRSGPromises()
        {
            var rsgVoidSource = new Promise();
            var rsgVectorSource = new Promise<Vector4>();
            var rsgObjectSource = new Promise<object>();
            rsgVoid = rsgVoidSource;
            rsgVector = rsgVectorSource;
            rsgObject = rsgObjectSource;
            rsgVoidSource.Resolve();
            rsgVectorSource.Resolve(Program.vector);
            rsgObjectSource.Resolve(Program.obj);
        }

        public static void ClearRSGPromises()
        {
            rsgVoid = default;
            rsgVector = default;
            rsgObject = default;
        }
    }

    partial class ContinuationBenchmarks
    {
        [GlobalSetup(Target = nameof(RSGPromise))]
        public void SetupRSGPromises()
        {
            RSGPromiseHelper.SetRSGPromises();
        }

        [GlobalCleanup(Target = nameof(RSGPromise))]
        public void CleanupRSGPromises()
        {
            RSGPromiseHelper.ClearRSGPromises();
        }

        [Benchmark]
        public void RSGPromise()
        {
            var deferred = new Promise<object>();
            IPromise<object> promise = deferred;

            for (int i = 0; i < N; ++i)
            {
                // Not a fair comparison since RSG doesn't support ContinueWith returning void or a simple value.
                // And apparently it doesn't let you convert from Promise to Promise<T> returning a simple value using then, either...
                promise = promise
                    //.ContinueWith(_ => { })
                    //.ContinueWith(_ => Program.vector)
                    //.ContinueWith(_ => Program.obj)
                    .ContinueWith(() => RSGPromiseHelper.rsgVoid)
                    .ContinueWith(() => RSGPromiseHelper.rsgVector)
                    .ContinueWith(() => RSGPromiseHelper.rsgObject);
            }

            promise.Done();
            deferred.Resolve(Program.obj);
        }
    }

    // RSG does not support async/await, so we won't run benchmarks for those.
    partial class AwaitBenchmarks { }

    partial class AsyncBenchmarks { }
}