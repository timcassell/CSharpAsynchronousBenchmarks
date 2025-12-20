using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Portability;
using System;
using System.Runtime.CompilerServices;

namespace AsynchronousBenchmarks
{
    [MemoryDiagnoser(false)]
    public abstract class AsyncBenchmark : IPooledMemoryGetter
    {
        [ParamsAllValues]
        public bool Pending;

        internal long _pooledMemory;

        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        protected void InitTotalMemory()
        {
            AppDomain.MonitoringIsEnabled = true;
            // verify the api works
            if (!AppDomain.MonitoringIsEnabled || AppDomain.CurrentDomain.MonitoringSurvivedMemorySize < 0)
            {
                throw new NotSupportedException();
            }
            // Measure bytes to allow GC monitor to make its initial allocations.
            DeadCodeEliminationHelper.KeepAliveWithoutBoxing(GetTotalMemory());
        }

        public long GetPooledMemory() => _pooledMemory;

        [MethodImpl(CodeGenHelper.AggressiveOptimizationOption)]
        protected static long GetTotalMemory()
        {
            // Enforce GC.Collect here to make sure we get accurate results.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return AppDomain.CurrentDomain.MonitoringSurvivedMemorySize;
        }
    }

    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory), BenchmarkCategory(nameof(AsyncAwait))]
    public partial class AsyncAwait : AsyncBenchmark
    {
    }

    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory), BenchmarkCategory(nameof(ContinueWith))]
    public partial class ContinueWith : AsyncBenchmark
    {
    }
}

namespace Helper
{
    public struct Struct32
    {
        public long l1, l2, l3, l4;
    }
}