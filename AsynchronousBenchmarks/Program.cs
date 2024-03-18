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
}