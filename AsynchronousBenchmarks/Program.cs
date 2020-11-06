using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace AsynchronousBenchmarks
{
    public class Program
    {
        public static readonly object obj = new object();
        public static readonly Vector4 vector = new Vector4(1f, -2.1f, -4f, 5.5f);

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
            // you can change it to Job.Default but Short just takes less time to run and is good for testing
            var job = Job.ShortRun;

#if PROTO_PROMISE_PROGRESS_DISABLE
            // define custom MsBuildArgument that BenchmarkDotNet MUST pass to MsBuild when building the auto-generated projects
            // and the referenced proejcts
            job = job.WithArguments(new Argument[] { new MsBuildArgument("/p:Progress=false") });
#endif
            // tell BDN that this job should be extended by arguments passed from command line
            job = job.AsDefault();

            var config = DefaultConfig.Instance.AddJob(job);

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }

    [MemoryDiagnoser]
    public partial class ContinuationBenchmarks
    {
        [Params(100, 10_000)]
        public int N;
    }

    [MemoryDiagnoser]
    public partial class AwaitBenchmarks
    {
        [Params(100, 10_000)]
        public int N;
    }

    [MemoryDiagnoser]
    public partial class AsyncBenchmarks
    {
        [Params(100, 10_000)]
        public int N;
    }

    public struct Vector4
    {
        public float x, y, z, w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}
