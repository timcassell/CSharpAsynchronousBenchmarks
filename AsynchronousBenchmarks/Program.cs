using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.Net48)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.NetCoreApp31)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.CoreRt31)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.Mono)]

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

    [CategoriesColumn]
    public partial class ContinueWithPending
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    public partial class ContinueWithResolved
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    public partial class ContinueWithFromValue
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    public partial class AwaitPending
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    public partial class AwaitResolved
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    public partial class AsyncPending
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    public partial class AsyncResolved
    {
        [Params(100, 10_000)]
        public int N;
    }
}
