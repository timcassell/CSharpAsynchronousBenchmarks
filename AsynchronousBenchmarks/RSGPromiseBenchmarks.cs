using BenchmarkDotNet.Attributes;
using RSG;
using System;

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

    // RSG doesn't support all ContinueWiths, so create those extension methods here to match other benchmarks.
    static class RSGPromiseExtensions
    {
        public static IPromise<T> ContinueWith<T>(this IPromise promise, Func<T> callback)
        {
            return promise.ContinueWith(() => Promise<T>.Resolved(callback()));
        }

        public static IPromise ContinueWith<T>(this IPromise<T> promise, Action callback)
        {
            return promise.ContinueWith(() =>
            {
                callback();
                return Promise.Resolved();
            });
        }

        public static IPromise<TConvert> ContinueWith<T, TConvert>(this IPromise<T> promise, Func<TConvert> callback)
        {
            return promise.ContinueWith(() => Promise<TConvert>.Resolved(callback()));
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
                promise = promise
                    // Extension methods created here.
                    .ContinueWith(() => { })
                    .ContinueWith(() => Program.vector)
                    .ContinueWith(() => Program.obj)
                    // Native methods.
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