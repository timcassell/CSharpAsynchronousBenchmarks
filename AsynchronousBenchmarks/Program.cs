using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.Net48)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.Mono)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.NetCoreApp31)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, RuntimeMoniker.CoreRt31)]

namespace AsynchronousBenchmarks
{
    public class Program
    {
        /// <summary>
        /// to run specific benchmarks you must pass `--filter $glob`. Examples:
        ///     * --filter '*' // runs all
        ///     * --filter 'ContinuationBenchmarks' // runs only ContinuationBenchmarks
        /// to run against specific runtimes you need to pass their target framework monikers using --runtimes. Examples:
        ///     * --runtimes net472 mono netcoreapp31 corert31
        /// to disable "Progress" modify the .csproj file and set it to false
        /// Sample full command:
        ///     * dotnet run -c Release -f net472 --filter * --runtimes net472 mono netcoreapp31 corert31
        /// For more info please refer to https://benchmarkdotnet.org/articles/configs/toolchains.html#multiple-frameworks-support
        /// </summary>
        public static void Main(string[] args)
        {
            var job =
                Job.Default
                // tell BDN that this job should be extended by arguments passed from command line
                .AsDefault();

            var config = DefaultConfig.Instance.AddJob(job);

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }

    [CategoriesColumn]
    [MemoryDiagnoser]
    public partial class ContinueWithPending
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    [MemoryDiagnoser]
    public partial class ContinueWithResolved
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    [MemoryDiagnoser]
    public partial class ContinueWithFromValue
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    [MemoryDiagnoser]
    public partial class AwaitPending
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    [MemoryDiagnoser]
    public partial class AwaitResolved
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    [MemoryDiagnoser]
    public partial class AsyncPending
    {
        [Params(100, 10_000)]
        public int N;
    }

    [CategoriesColumn]
    [MemoryDiagnoser]
    public partial class AsyncResolved
    {
        [Params(100, 10_000)]
        public int N;
    }
}
