using BenchmarkDotNet.Attributes;

namespace AsynchronousBenchmarks
{
    partial class ContinueWithPending
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void GlobalSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.ContinueWithPending.GlobalSetup();
        }

        [IterationSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void IterationSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.ContinueWithPending.IterationSetup(N);
        }

        [IterationCleanup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void IterationCleanupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.ContinueWithPending.IterationCleanup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NoProgress_NoPool()
        {
            ProtoPromise_NoProgress.ContinueWithPending.ExecuteWithoutPool(N);
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NoProgress_Pool()
        {
            ProtoPromise_NoProgress.ContinueWithPending.ExecuteWithPool(N);
        }
    }

    partial class ContinueWithResolved
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void GlobalSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.ContinueWithResolved.GlobalSetup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NoProgress_NoPool()
        {
            ProtoPromise_NoProgress.ContinueWithResolved.ExecuteWithoutPool(N);
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NoProgress_Pool()
        {
            ProtoPromise_NoProgress.ContinueWithResolved.ExecuteWithPool(N);
        }
    }

    partial class ContinueWithFromValue
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void GlobalSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.ContinueWithFromValue.GlobalSetup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NoProgress_NoPool()
        {
            ProtoPromise_NoProgress.ContinueWithFromValue.ExecuteWithoutPool(N);
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NoProgress_Pool()
        {
            ProtoPromise_NoProgress.ContinueWithFromValue.ExecuteWithPool(N);
        }
    }

    partial class AwaitPending
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void GlobalSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.AwaitPending.GlobalSetup();
        }

        [IterationSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void IterationSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.AwaitPending.IterationSetup(N);
        }

        [IterationCleanup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void IterationCleanupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.AwaitPending.IterationCleanup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NoProgress_NoPool()
        {
            ProtoPromise_NoProgress.AwaitPending.ExecuteWithoutPool(N);
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NoProgress_Pool()
        {
            ProtoPromise_NoProgress.AwaitPending.ExecuteWithPool(N);
        }
    }

    partial class AwaitResolved
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void GlobalSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.AwaitResolved.GlobalSetup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NoProgress_NoPool()
        {
            ProtoPromise_NoProgress.AwaitResolved.ExecuteWithoutPool(N);
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NoProgress_Pool()
        {
            ProtoPromise_NoProgress.AwaitResolved.ExecuteWithPool(N);
        }
    }

    partial class AsyncPending
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void GlobalSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.AsyncPending.GlobalSetup();
        }

        [IterationSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void IterationSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.AsyncPending.IterationSetup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NoProgress_NoPool()
        {
            ProtoPromise_NoProgress.AsyncPending.ExecuteWithoutPool(N);
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NoProgress_Pool()
        {
            ProtoPromise_NoProgress.AsyncPending.ExecuteWithPool(N);
        }
    }

    partial class AsyncResolved
    {
        // No progress

        [GlobalSetup(Targets = new[] { nameof(ProtoPromise_NoProgress_NoPool), nameof(ProtoPromise_NoProgress_Pool) })]
        public void GlobalSetupProtoPromises_NoProgress()
        {
            ProtoPromise_NoProgress.AsyncResolved.GlobalSetup();
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("No Pool")]
        public void ProtoPromise_NoProgress_NoPool()
        {
            ProtoPromise_NoProgress.AsyncResolved.ExecuteWithoutPool(N);
        }

        [Benchmark(Description = "ProtoPromise")]
        [BenchmarkCategory("Pool")]
        public void ProtoPromise_NoProgress_Pool()
        {
            ProtoPromise_NoProgress.AsyncResolved.ExecuteWithPool(N);
        }
    }
}