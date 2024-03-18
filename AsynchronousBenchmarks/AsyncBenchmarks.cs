using BenchmarkDotNet.Attributes;

namespace AsynchronousBenchmarks
{
    public abstract class AsyncBenchmark
    {
        [ParamsAllValues]
        public bool Pending;
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