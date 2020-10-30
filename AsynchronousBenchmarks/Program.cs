using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, BenchmarkDotNet.Jobs.RuntimeMoniker.Mono)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, BenchmarkDotNet.Jobs.RuntimeMoniker.CoreRt31)]
[assembly: SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput, BenchmarkDotNet.Jobs.RuntimeMoniker.Net48)]

namespace AsynchronousBenchmarks
{
    public class Program
    {
        public static readonly object obj = new object();
        public static readonly Vector4 vector = new Vector4(1f, -2.1f, -4f, 5.5f);

        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<ContinuationBenchmarks>();
            BenchmarkRunner.Run<AwaitBenchmarks>();
            BenchmarkRunner.Run<AsyncBenchmarks>();
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
