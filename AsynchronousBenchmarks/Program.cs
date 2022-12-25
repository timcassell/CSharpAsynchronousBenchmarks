using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AsynchronousBenchmarks
{
    public class Program
    {
        /// <summary>
        /// Console args are provided from BenchmarkRunner.bat.
        /// See https://benchmarkdotnet.org/articles/guides/console-args.html
        /// </summary>
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    public abstract class AsyncBenchmark
    {
        [Params(true, false)]
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