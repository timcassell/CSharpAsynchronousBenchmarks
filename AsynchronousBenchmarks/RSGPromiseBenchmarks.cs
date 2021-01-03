using BenchmarkDotNet.Attributes;
using Helper;
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
            rsgVectorSource.Resolve(Instances.vector);
            rsgObjectSource.Resolve(Instances.obj);
        }

        public static Promise[] rsgVoids;
        public static Promise<Vector4>[] rsgVectors;
        public static Promise<object>[] rsgObjects;

        public static void SetDeferreds(int N)
        {
            if (rsgVoids != null)
            {
                // Don't recreate deferreds.
                return;
            }

            rsgVoids = new Promise[N];
            rsgVectors = new Promise<Vector4>[N];
            rsgObjects = new Promise<object>[N];
            for (int i = 0; i < N; ++i)
            {
                rsgVoids[i] = new Promise();
                rsgVectors[i] = new Promise<Vector4>();
                rsgObjects[i] = new Promise<object>();
            }
        }

        public static void ClearDeferreds()
        {
            rsgVoids = default;
            rsgVectors = default;
            rsgObjects = default;
        }

        public static void ResolveDeferreds()
        {
            for (int i = 0, max = rsgVoids.Length; i < max; ++i)
            {
                rsgVoids[i].Resolve();
                rsgVectors[i].Resolve(Instances.vector);
                rsgObjects[i].Resolve(Instances.obj);
            }
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

    partial class ContinueWithPending
    {
        [GlobalSetup(Target = nameof(RSGPromise))]
        public void GlobalSetupRSGPromises()
        {
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            RSGPromiseHelper.SetDeferreds(1);
            ExecuteRSGPromise(1);
            RSGPromiseHelper.ClearDeferreds();
        }

        [IterationSetup(Target = nameof(RSGPromise))]
        public void IterationSetupRSGPromises()
        {
            RSGPromiseHelper.SetDeferreds(N);
        }

        [IterationCleanup(Target = nameof(RSGPromise))]
        public void IterationCleanupRSGPromises()
        {
            RSGPromiseHelper.ClearDeferreds();
        }

        [Benchmark]
        public void RSGPromise()
        {
            ExecuteRSGPromise(N);
        }

        private static int rsg_index;
        private void ExecuteRSGPromise(int n)
        {
            rsg_index = -1;
            var deferred = new Promise<object>();
            IPromise<object> promise = deferred;

            for (int i = 0; i < n; ++i)
            {
                promise = promise
                    // Native methods.
                    .ContinueWith(() => (IPromise) RSGPromiseHelper.rsgVoids[++rsg_index])
                    .ContinueWith(() => (IPromise<Vector4>) RSGPromiseHelper.rsgVectors[rsg_index])
                    .ContinueWith(() => (IPromise<object>) RSGPromiseHelper.rsgObjects[rsg_index]);
            }

            promise.Done();
            deferred.Resolve(Instances.obj);
            RSGPromiseHelper.ResolveDeferreds();
        }
    }

    partial class ContinueWithResolved
    {
        [GlobalSetup(Target = nameof(RSGPromise))]
        public void GlobalSetupRSGPromises()
        {
            RSGPromiseHelper.SetRSGPromises();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteRSGPromise(1);
        }

        [Benchmark]
        public void RSGPromise()
        {
            ExecuteRSGPromise(N);
        }

        private void ExecuteRSGPromise(int n)
        {
            var deferred = new Promise<object>();
            IPromise<object> promise = deferred;

            for (int i = 0; i < n; ++i)
            {
                promise = promise
                    // Native methods.
                    .ContinueWith(() => RSGPromiseHelper.rsgVoid)
                    .ContinueWith(() => RSGPromiseHelper.rsgVector)
                    .ContinueWith(() => RSGPromiseHelper.rsgObject);
            }

            promise.Done();
            deferred.Resolve(Instances.obj);
        }
    }

    partial class ContinueWithFromValue
    {
        [GlobalSetup(Target = nameof(RSGPromise))]
        public void GlobalSetupRSGPromises()
        {
            RSGPromiseHelper.SetRSGPromises();
            // Run once to allow JIT to allocate (necessary for CORE runtimes) so survived memory is only measuring the actual objects, not the code.
            ExecuteRSGPromise(1);
        }

        [Benchmark]
        public void RSGPromise()
        {
            ExecuteRSGPromise(N);
        }

        private void ExecuteRSGPromise(int n)
        {
            var deferred = new Promise<object>();
            IPromise<object> promise = deferred;

            for (int i = 0; i < n; ++i)
            {
                promise = promise
                    // Extension methods created here.
                    .ContinueWith(() => { })
                    .ContinueWith(() => Instances.vector)
                    .ContinueWith(() => Instances.obj);
            }

            promise.Done();
            deferred.Resolve(Instances.obj);
        }
    }

    // RSG does not support async/await, just throw so it will still be included in the summary table.

    partial class AwaitPending
    {
        [Benchmark]
        public void RSGPromise()
        {
            throw new NotImplementedException();
        }
    }

    partial class AwaitResolved
    {
        [Benchmark]
        public void RSGPromise()
        {
            throw new NotImplementedException();
        }
    }

    partial class AsyncPending
    {
        [Benchmark]
        public void RSGPromise()
        {
            throw new NotImplementedException();
        }
    }

    partial class AsyncResolved
    {
        [Benchmark]
        public void RSGPromise()
        {
            throw new NotImplementedException();
        }
    }
}