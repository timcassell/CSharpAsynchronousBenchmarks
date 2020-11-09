using BenchmarkDotNet.Attributes;

namespace AsynchronousBenchmarks
{
    partial class ContinueWithPending
    {
        // No progress

        [IterationSetup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void SetupProtoPromisesIteration_N()
        {
            ProtoPromise_NoProgress.ContinueWithPending.Setup(N);
        }

        [IterationCleanup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void CleanupProtoPromisesIteration_N()
        {
            ProtoPromise_NoProgress.ContinueWithPending.Cleanup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NN()
        {
            ProtoPromise_NoProgress.ContinueWithPending.ExecuteWithoutPool();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NP()
        {
            ProtoPromise_NoProgress.ContinueWithPending.ExecuteWithPool();
        }

        // With progress
        // Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638

        //[IterationSetup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void SetupProtoPromisesIteration_P()
        //{
        //    ProtoPromise_Progress.ContinueWithPending.Setup(N);
        //}

        //[IterationCleanup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void CleanupProtoPromisesIteration_P()
        //{
        //    ProtoPromise_Progress.ContinueWithPending.Cleanup();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_NoPool")]
        //public void ProtoPromise_PN()
        //{
        //    ProtoPromise_Progress.ContinueWithPending.ExecuteWithoutPool();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_Pool")]
        //public void ProtoPromise_PP()
        //{
        //    ProtoPromise_Progress.ContinueWithPending.ExecuteWithPool();
        //}
    }

    partial class ContinueWithResolved
    {
        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void SetupProtoPromises_N()
        {
            ProtoPromise_NoProgress.ContinueWithResolved.Setup(N);
        }

        [GlobalCleanup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void CleanupProtoPromises_N()
        {
            ProtoPromise_NoProgress.ContinueWithResolved.Cleanup();
        }

        // No progress

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NN()
        {
            ProtoPromise_NoProgress.ContinueWithResolved.ExecuteWithoutPool();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NP()
        {
            ProtoPromise_NoProgress.ContinueWithResolved.ExecuteWithPool();
        }

        // With progress
        // Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638

        //[GlobalSetup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void SetupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.ContinueWithResolved.Setup(N);
        //}

        //[GlobalCleanup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void CleanupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.ContinueWithResolved.Cleanup();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_NoPool")]
        //public void ProtoPromise_PN()
        //{
        //    ProtoPromise_Progress.ContinueWithResolved.ExecuteWithoutPool();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_Pool")]
        //public void ProtoPromise_PP()
        //{
        //    ProtoPromise_Progress.ContinueWithResolved.ExecuteWithPool();
        //}
    }

    partial class ContinueWithFromValue
    {
        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void SetupProtoPromises_N()
        {
            ProtoPromise_NoProgress.ContinueWithFromValue.Setup(N);
        }

        [GlobalCleanup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void CleanupProtoPromises_N()
        {
            ProtoPromise_NoProgress.ContinueWithFromValue.Cleanup();
        }

        // No progress

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NN()
        {
            ProtoPromise_NoProgress.ContinueWithFromValue.ExecuteWithoutPool();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NP()
        {
            ProtoPromise_NoProgress.ContinueWithFromValue.ExecuteWithPool();
        }

        // With progress
        // Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638

        //[GlobalSetup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void SetupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.ContinueWithFromValue.Setup(N);
        //}

        //[GlobalCleanup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void CleanupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.ContinueWithFromValue.Cleanup();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_NoPool")]
        //public void ProtoPromise_PN()
        //{
        //    ProtoPromise_Progress.ContinueWithFromValue.ExecuteWithoutPool();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_Pool")]
        //public void ProtoPromise_PP()
        //{
        //    ProtoPromise_Progress.ContinueWithFromValue.ExecuteWithPool();
        //}
    }

    partial class AwaitPending
    {
        // No progress

        [IterationSetup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void SetupProtoPromisesIteration_N()
        {
            ProtoPromise_NoProgress.AwaitPending.Setup(N);
        }

        [IterationCleanup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void CleanupProtoPromisesIteration_N()
        {
            ProtoPromise_NoProgress.AwaitPending.Cleanup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NN()
        {
            ProtoPromise_NoProgress.AwaitPending.ExecuteWithoutPool();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NP()
        {
            ProtoPromise_NoProgress.AwaitPending.ExecuteWithPool();
        }

        // With progress
        // Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638

        //[IterationSetup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void SetupProtoPromisesIteration_P()
        //{
        //    ProtoPromise_Progress.AwaitPending.Setup(N);
        //}

        //[IterationCleanup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void CleanupProtoPromisesIteration_P()
        //{
        //    ProtoPromise_Progress.AwaitPending.Cleanup();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_NoPool")]
        //public void ProtoPromise_PN()
        //{
        //    ProtoPromise_Progress.AwaitPending.ExecuteWithoutPool();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_Pool")]
        //public void ProtoPromise_PP()
        //{
        //    ProtoPromise_Progress.AwaitPending.ExecuteWithPool();
        //}
    }

    partial class AwaitResolved
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void SetupProtoPromisesIteration_N()
        {
            ProtoPromise_NoProgress.AwaitResolved.Setup(N);
        }

        [GlobalCleanup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void CleanupProtoPromisesIteration_N()
        {
            ProtoPromise_NoProgress.AwaitResolved.Cleanup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NN()
        {
            ProtoPromise_NoProgress.AwaitResolved.ExecuteWithoutPool();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NP()
        {
            ProtoPromise_NoProgress.AwaitResolved.ExecuteWithPool();
        }

        // With progress
        // Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638

        //[GlobalSetup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void SetupProtoPromisesIteration_P()
        //{
        //    ProtoPromise_Progress.AwaitResolved.Setup(N);
        //}

        //[GlobalCleanup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void CleanupProtoPromisesIteration_P()
        //{
        //    ProtoPromise_Progress.AwaitResolved.Cleanup();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_NoPool")]
        //public void ProtoPromise_PN()
        //{
        //    ProtoPromise_Progress.AwaitResolved.ExecuteWithoutPool();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_Pool")]
        //public void ProtoPromise_PP()
        //{
        //    ProtoPromise_Progress.AwaitResolved.ExecuteWithPool();
        //}
    }

    partial class AsyncPending
    {
        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void SetupProtoPromises_N()
        {
            ProtoPromise_NoProgress.AsyncPending.Setup(N);
        }

        [GlobalCleanup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void CleanupProtoPromises_N()
        {
            ProtoPromise_NoProgress.AsyncPending.Cleanup();
        }

        // No progress

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NN()
        {
            ProtoPromise_NoProgress.AsyncPending.ExecuteWithoutPool();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NP()
        {
            ProtoPromise_NoProgress.AsyncPending.ExecuteWithPool();
        }

        // With progress
        // Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638

        //[GlobalSetup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void SetupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.AsyncPending.Setup(N);
        //}

        //[GlobalCleanup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void CleanupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.AsyncPending.Cleanup();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_NoPool")]
        //public void ProtoPromise_PN()
        //{
        //    ProtoPromise_Progress.AsyncPending.ExecuteWithoutPool();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_Pool")]
        //public void ProtoPromise_PP()
        //{
        //    ProtoPromise_Progress.AsyncPending.ExecuteWithPool();
        //}
    }

    partial class AsyncResolved
    {
        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void SetupProtoPromises_N()
        {
            ProtoPromise_NoProgress.AsyncResolved.Setup(N);
        }

        [GlobalCleanup(Targets = new[] { nameof(ProtoPromise_NN), nameof(ProtoPromise_NP) })]
        public void CleanupProtoPromises_N()
        {
            ProtoPromise_NoProgress.AsyncResolved.Cleanup();
        }

        // No progress

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NN()
        {
            ProtoPromise_NoProgress.AsyncResolved.ExecuteWithoutPool();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NP()
        {
            ProtoPromise_NoProgress.AsyncResolved.ExecuteWithPool();
        }

        // With progress
        // Removed until compile errors are fixed: https://github.com/dotnet/msbuild/issues/4943, https://developercommunity2.visualstudio.com/t/vs-2019-can-only-use-one-reference-with-the-same-a/1247638

        //[GlobalSetup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void SetupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.AsyncResolved.Setup(N);
        //}

        //[GlobalCleanup(Targets = new[] { nameof(ProtoPromise_PN), nameof(ProtoPromise_PP) })]
        //public void CleanupProtoPromises_P()
        //{
        //    ProtoPromise_Progress.AsyncResolved.Cleanup();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_NoPool")]
        //public void ProtoPromise_PN()
        //{
        //    ProtoPromise_Progress.AsyncResolved.ExecuteWithoutPool();
        //}

        //[Benchmark(Description = "ProtoPromise")]
        //[BenchmarkCategory("Progress_Pool")]
        //public void ProtoPromise_PP()
        //{
        //    ProtoPromise_Progress.AsyncResolved.ExecuteWithPool();
        //}
    }
}