# C# Asynchronous Libraries Benchmarks

This project is built to benchmark C# asynchronous libraries against each other.

These benchmarks are ran using [BenchmarkDotNet v0.12.1](https://github.com/dotnet/BenchmarkDotNet).

This is a non-exhaustive benchmark of each library's features. It is only meant to compare the most common use scenarios. Some libraries provide more/different features than others.

Asynchronous libraries benchmarked:
- [System.Threading.Tasks (TPL)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) labeled as `DotNetTask` - this was measured as the baseline.
- [ProtoPromise v1.0.0](https://github.com/timcassell/ProtoPromise)
- [UniTask v2.0.36](https://github.com/Cysharp/UniTask)
- [RSG Promises v3.0.1](https://github.com/Real-Serious-Games/C-Sharp-Promise)

To run the benchmarks on your machine:
  1. If possible, disable CPU boost to get more accurate results.
  2. Run `BenchmarkRunner.bat`.
  - Some benchmarks crash, causing windows to show a popup. You must close that popup for the rest of the benchmarks to continue.
  3. See your results in the generated `BenchmarkDotNet.Artifacts` directory.

## Contents

- [AsyncPending Benchmarks](#asyncpending-benchmarks)
- [AsyncResolved Benchmarks](#asyncresolved-benchmarks)
- [AwaitPending Benchmarks](#awaitpending-benchmarks)
- [AwaitResolved Benchmarks](#awaitresolved-benchmarks)
- [ContinueWithFromValue Benchmarks](#continuewithfromvalue-benchmarks)
- [ContinueWithPending Benchmarks](#continuewithpending-benchmarks)
- [ContinueWithResolved Benchmarks](#continuewithresolved-benchmarks)

Here are the results after running on my 2010 desktop:

```
BenchmarkDotNet=v0.12.1, OS=Windows 7 SP1 (6.1.7601.0)
AMD Phenom(tm) II X6 1055T Processor, 1 CPU, 6 logical and 6 physical cores
Frequency=2746679 Hz, Resolution=364.0760 ns, Timer=TSC
  [Host]     : .NET Framework 4.8 (4.8.4110.0), X64 RyuJIT
  Job-KZKWXR : .NET Framework 4.8 (4.8.4110.0), X64 RyuJIT
  Job-LKAUXA : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  Job-OUGRBP : .NET 5.0.29408.02 @BuiltBy: dlab14-DDVSOWINAGE075 @Branch: master @Commit: 4ce1c21ac0d4d1a3b7f7a548214966f69ac9f199, X64 AOT
  Job-IAWAXN : Mono 6.10.0 (Visual Studio), X64
```

- Category `No Pool` = ProtoPromise with no object pooling.
- Category `Pool` = ProtoPromise with all object pooling.
- ProtoPromise progress is disabled for these benchmarks.

Any benchmark that shows `NA` or `?` as the value means it was unable to complete the benchmark. Either an exception was thrown, or the runtime crashed.

You can see that some libraries win some benchmarks and lose others, but `ProtoPromise` is the only one that successfully completes all of them in all runtimes and all values of `N`.

Note: I removed `Job`, `Error`, `StdDev`, and `RatioSD` from the table to condense it and make it easier to read. Those columns will show in your local benchmarks results if you run them on your machine.

### AsyncPending Benchmarks

This is testing this setup:

```
for (int i = 0; i < N; ++i)
{
    _ = TaskVoid();
    _ = TaskVector();
    _ = TaskObject();
}

async Task TaskVoid()
{
    await pending_task;
}

async Task<Vector4> TaskVector()
{
    await pending_task;
    return Instances.vector;
}

async Task<object> TaskObject()
{
    await pending_task;
    return Instances.obj;
}
```

Note: RSG Promises don't support async, so the benchmark for this just throws NotImplementedException so that it will still show on the table.


```
|       Method |       Runtime | Categories |     N |        Mean | Ratio |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------- |-------------- |----------- |------ |------------:|------:|----------:|---------:|---------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    430.7 μs |  1.71 |   75.1953 |  37.5977 |   4.8828 |   61841 B |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |    108.5 μs |  0.43 |         - |        - |        - |         - |
|   RSGPromise |      .NET 4.8 |            |   100 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |    251.3 μs |  1.00 |   82.0313 |   3.9063 |        - |   70613 B |
|      UniTask |      .NET 4.8 |            |   100 |    158.1 μs |  0.63 |    0.7324 |   0.2441 |        - |    3761 B |
|              |               |            |       |             |       |           |          |          |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    398.5 μs |  3.69 |   19.5313 |   9.7656 |   0.9766 |   61656 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |    101.6 μs |  0.94 |         - |        - |        - |       2 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask | .NET Core 3.1 |            |   100 |    108.1 μs |  1.00 |   16.9678 |        - |        - |   53600 B |
|      UniTask | .NET Core 3.1 |            |   100 |    136.1 μs |  1.26 |         - |        - |        - |         - |
|              |               |            |       |             |       |           |          |          |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    290.6 μs |  2.48 |   18.0664 |   8.7891 |   0.4883 |   56856 B |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |    110.0 μs |  0.94 |         - |        - |        - |         - |
|   RSGPromise |    CoreRt 3.1 |            |   100 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |    117.1 μs |  1.00 |   15.5029 |        - |        - |   48800 B |
|      UniTask |    CoreRt 3.1 |            |   100 |          NA |     ? |         - |        - |        - |         - |
|              |               |            |       |             |       |           |          |          |           |
| ProtoPromise |          Mono |    No Pool |   100 |    643.3 μs |  3.43 |   16.6016 |   1.9531 |   1.9531 |         - |
| ProtoPromise |          Mono |       Pool |   100 |    119.2 μs |  0.64 |         - |        - |        - |         - |
|   RSGPromise |          Mono |            |   100 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask |          Mono |            |   100 |    187.6 μs |  1.00 |   21.7285 |        - |        - |         - |
|      UniTask |          Mono |            |   100 |    140.1 μs |  0.75 |         - |        - |        - |         - |
|              |               |            |       |             |       |           |          |          |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 45,896.7 μs |  1.56 | 1000.0000 | 363.6364 | 181.8182 | 6178409 B |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 12,413.3 μs |  0.42 |         - |        - |        - |         - |
|   RSGPromise |      .NET 4.8 |            | 10000 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 29,437.8 μs |  1.00 | 1312.5000 | 656.2500 |        - | 7060860 B |
|      UniTask |      .NET 4.8 |            | 10000 | 20,939.7 μs |  0.71 |   62.5000 |  31.2500 |        - |  376318 B |
|              |               |            |       |             |       |           |          |          |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 50,156.6 μs |  2.92 | 1090.9091 | 454.5455 | 181.8182 | 6160183 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 11,476.3 μs |  0.67 |         - |        - |        - |         - |
|   RSGPromise | .NET Core 3.1 |            | 10000 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 17,162.4 μs |  1.00 |  875.0000 | 406.2500 |        - | 5360000 B |
|      UniTask | .NET Core 3.1 |            | 10000 | 19,713.4 μs |  1.15 |         - |        - |        - |         - |
|              |               |            |       |             |       |           |          |          |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 34,336.9 μs |  2.15 |  937.5000 | 375.0000 | 125.0000 | 5680058 B |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 11,649.1 μs |  0.73 |         - |        - |        - |         - |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 15,945.2 μs |  1.00 |  781.2500 | 375.0000 |        - | 4880000 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |          NA |     ? |         - |        - |        - |         - |
|              |               |            |       |             |       |           |          |          |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 84,877.4 μs |  2.16 | 1571.4286 | 285.7143 | 285.7143 |         - |
| ProtoPromise |          Mono |       Pool | 10000 | 13,223.3 μs |  0.34 |         - |        - |        - |         - |
|   RSGPromise |          Mono |            | 10000 |          NA |     ? |         - |        - |        - |         - |
|   DotNetTask |          Mono |            | 10000 | 39,304.7 μs |  1.00 | 1923.0769 | 230.7692 | 230.7692 |         - |
|      UniTask |          Mono |            | 10000 | 22,169.0 μs |  0.56 |         - |        - |        - |         - |

Benchmarks with issues:
  AsyncPending.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=100]
  AsyncPending.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=100]
  AsyncPending.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  AsyncPending.UniTask: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  AsyncPending.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  AsyncPending.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  AsyncPending.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  AsyncPending.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  AsyncPending.UniTask: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  AsyncPending.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
```

### AsyncResolved Benchmarks

This is testing this setup:

```
for (int i = 0; i < N; ++i)
{
    _ = TaskVoid();
    _ = TaskVector();
    _ = TaskObject();
}

async Task TaskVoid()
{
}

async Task<Vector4> TaskVector()
{
    return Instances.vector;
}

async Task<object> TaskObject()
{
    return Instances.obj;
}
```

Note: RSG Promises don't support async, so the benchmark for this just throws NotImplementedException so that it will still show on the table.


```
|       Method |       Runtime | Categories |     N |         Mean | Ratio |     Gen 0 |    Gen 1 |   Gen 2 | Allocated |
|------------- |-------------- |----------- |------ |-------------:|------:|----------:|---------:|--------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    251.89 μs |  7.48 |   25.3906 |  12.6953 |  0.4883 |   20062 B |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |     40.42 μs |  1.20 |         - |        - |       - |         - |
|   RSGPromise |      .NET 4.8 |            |   100 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |     33.68 μs |  1.00 |   21.4233 |        - |       - |   16852 B |
|      UniTask |      .NET 4.8 |            |   100 |     22.16 μs |  0.66 |         - |        - |       - |         - |
|              |               |            |       |              |       |           |          |         |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    207.89 μs | 13.70 |    6.3477 |   3.1738 |       - |   20000 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |     37.47 μs |  2.47 |         - |        - |       - |         - |
|   RSGPromise | .NET Core 3.1 |            |   100 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask | .NET Core 3.1 |            |   100 |     15.18 μs |  1.00 |    4.8370 |        - |       - |   15200 B |
|      UniTask | .NET Core 3.1 |            |   100 |     21.45 μs |  1.41 |         - |        - |       - |         - |
|              |               |            |       |              |       |           |          |         |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    113.53 μs |  8.84 |    6.3477 |   3.1738 |       - |   20000 B |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |     32.45 μs |  2.53 |         - |        - |       - |         - |
|   RSGPromise |    CoreRt 3.1 |            |   100 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |     12.85 μs |  1.00 |    4.8370 |        - |       - |   15200 B |
|      UniTask |    CoreRt 3.1 |            |   100 |     14.96 μs |  1.16 |         - |        - |       - |         - |
|              |               |            |       |              |       |           |          |         |           |
| ProtoPromise |          Mono |    No Pool |   100 |    184.94 μs |  4.88 |    4.1504 |   0.4883 |  0.4883 |         - |
| ProtoPromise |          Mono |       Pool |   100 |     58.48 μs |  1.54 |         - |        - |       - |         - |
|   RSGPromise |          Mono |            |   100 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask |          Mono |            |   100 |     37.89 μs |  1.00 |    4.0283 |        - |       - |         - |
|      UniTask |          Mono |            |   100 |     17.68 μs |  0.47 |         - |        - |       - |         - |
|              |               |            |       |              |       |           |          |         |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 26,351.52 μs |  7.88 |  343.7500 | 156.2500 | 62.5000 | 2006041 B |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 |  4,404.23 μs |  1.32 |         - |        - |       - |         - |
|   RSGPromise |      .NET 4.8 |            | 10000 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 |  3,333.74 μs |  1.00 | 2140.6250 |        - |       - | 1685159 B |
|      UniTask |      .NET 4.8 |            | 10000 |  2,203.33 μs |  0.66 |         - |        - |       - |         - |
|              |               |            |       |              |       |           |          |         |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 23,279.36 μs | 15.49 |  375.0000 | 125.0000 | 62.5000 | 2000018 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 |  4,311.19 μs |  2.87 |         - |        - |       - |         - |
|   RSGPromise | .NET Core 3.1 |            | 10000 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 |  1,503.03 μs |  1.00 |  484.3750 |        - |       - | 1520030 B |
|      UniTask | .NET Core 3.1 |            | 10000 |  2,152.81 μs |  1.43 |         - |        - |       - |         - |
|              |               |            |       |              |       |           |          |         |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 12,096.50 μs |  9.42 |  343.7500 | 140.6250 | 62.5000 | 2000000 B |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 |  3,442.52 μs |  2.68 |         - |        - |       - |         - |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 |  1,283.46 μs |  1.00 |  484.3750 |        - |       - | 1520000 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |  1,494.94 μs |  1.16 |         - |        - |       - |         - |
|              |               |            |       |              |       |           |          |         |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 19,313.77 μs |  5.06 |  437.5000 |  62.5000 | 62.5000 |         - |
| ProtoPromise |          Mono |       Pool | 10000 |  5,144.43 μs |  1.35 |         - |        - |       - |         - |
|   RSGPromise |          Mono |            | 10000 |           NA |     ? |         - |        - |       - |         - |
|   DotNetTask |          Mono |            | 10000 |  3,818.75 μs |  1.00 |  406.2500 |        - |       - |         - |
|      UniTask |          Mono |            | 10000 |  1,756.69 μs |  0.46 |         - |        - |       - |         - |

Benchmarks with issues:
  AsyncResolved.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=100]
  AsyncResolved.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=100]
  AsyncResolved.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  AsyncResolved.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  AsyncResolved.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  AsyncResolved.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  AsyncResolved.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  AsyncResolved.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
```

### AwaitPending Benchmarks

This is testing this setup:

```
private async Task<object> AwaitTasks()
{
    for (int i = 0; i < N; ++i)
    {
        await pendingTaskVoids[i];
        _ = await pendingTaskVectors[i];
        _ = await pendingTaskObjects[i];
    }
    return obj;
}
```

Note: RSG Promises don't support await, so the benchmark for this just throws NotImplementedException so that it will still show on the table.

```
|       Method |       Runtime | Categories |     N |         Mean | Ratio | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------- |-------------- |----------- |------ |-------------:|------:|------:|------:|------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    312.43 μs |  2.08 |     - |     - |     - |   24576 B |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |    283.39 μs |  1.88 |     - |     - |     - |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |    150.16 μs |  1.00 |     - |     - |     - |         - |
|      UniTask |      .NET 4.8 |            |   100 |     29.29 μs |  0.20 |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise |      .NET 4.8 |            |   100 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    300.73 μs |  3.04 |     - |     - |     - |   18616 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |    313.45 μs |  3.17 |     - |     - |     - |         - |
|   DotNetTask | .NET Core 3.1 |            |   100 |     98.98 μs |  1.00 |     - |     - |     - |         - |
|      UniTask | .NET Core 3.1 |            |   100 |     27.40 μs |  0.28 |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise | .NET Core 3.1 |            |   100 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    127.31 μs |  1.71 |     - |     - |     - |   18560 B |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |    142.11 μs |  1.91 |     - |     - |     - |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |     74.36 μs |  1.00 |     - |     - |     - |         - |
|      UniTask |    CoreRt 3.1 |            |   100 |     74.64 μs |  1.01 |     - |     - |     - |   19272 B |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise |    CoreRt 3.1 |            |   100 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
| ProtoPromise |          Mono |    No Pool |   100 |    172.99 μs |  1.73 |     - |     - |     - |         - |
| ProtoPromise |          Mono |       Pool |   100 |    172.64 μs |  1.73 |     - |     - |     - |         - |
|   DotNetTask |          Mono |            |   100 |    100.02 μs |  1.00 |     - |     - |     - |         - |
|      UniTask |          Mono |            |   100 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise |          Mono |            |   100 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 31,970.89 μs |  2.26 |     - |     - |     - | 1852928 B |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 27,699.85 μs |  1.96 |     - |     - |     - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 14,166.37 μs |  1.00 |     - |     - |     - |         - |
|      UniTask |      .NET 4.8 |            | 10000 |  3,062.43 μs |  0.22 |     - |     - |     - |  725960 B |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise |      .NET 4.8 |            | 10000 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 20,548.79 μs |  3.18 |     - |     - |     - | 1840216 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 18,416.60 μs |  2.85 |     - |     - |     - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 |  6,458.99 μs |  1.00 |     - |     - |     - |         - |
|      UniTask | .NET Core 3.1 |            | 10000 |  2,658.85 μs |  0.41 |     - |     - |     - |  720184 B |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise | .NET Core 3.1 |            | 10000 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 12,565.83 μs |  1.77 |     - |     - |     - | 1840160 B |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 13,588.83 μs |  1.91 |     - |     - |     - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 |  7,126.38 μs |  1.00 |     - |     - |     - |         - |
|      UniTask |    CoreRt 3.1 |            | 10000 |  6,596.60 μs |  0.93 |     - |     - |     - | 1920072 B |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 15,259.30 μs |  1.55 |     - |     - |     - |         - |
| ProtoPromise |          Mono |       Pool | 10000 | 19,368.07 μs |  1.97 |     - |     - |     - |         - |
|   DotNetTask |          Mono |            | 10000 |  9,872.12 μs |  1.00 |     - |     - |     - |         - |
|      UniTask |          Mono |            | 10000 |           NA |     ? |     - |     - |     - |         - |
|              |               |            |       |              |       |       |       |       |           |
|   RSGPromise |          Mono |            | 10000 |           NA |     ? |     - |     - |     - |         - |

Benchmarks with issues:
  AwaitPending.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=100]
  AwaitPending.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=100]
  AwaitPending.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  AwaitPending.UniTask: Job-QFEDFF(Runtime=Mono, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=100]
  AwaitPending.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  AwaitPending.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  AwaitPending.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  AwaitPending.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  AwaitPending.UniTask: Job-QFEDFF(Runtime=Mono, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=10000]
  AwaitPending.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
```

### AwaitResolved Benchmarks

This is testing this setup:

```
private async Task<object> AwaitTasks()
{
    for (int i = 0; i < N; ++i)
    {
        await completedTaskVoid;
        _ = await completedTaskVector;
        _ = await completedTaskObject;
    }
    return obj;
}
```

Note: RSG Promises don't support await, so the benchmark for this just throws NotImplementedException so that it will still show on the table.

```
|       Method |       Runtime | Categories |     N |       Mean | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------- |-------------- |----------- |------ |-----------:|------:|-------:|------:|------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |   4.514 μs |  0.82 | 0.0992 |     - |     - |      80 B |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |   4.296 μs |  0.78 | 0.0992 |     - |     - |      80 B |
|   RSGPromise |      .NET 4.8 |            |   100 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |   5.529 μs |  1.00 | 0.0992 |     - |     - |      80 B |
|      UniTask |      .NET 4.8 |            |   100 |   7.073 μs |  1.28 | 0.0992 |     - |     - |      80 B |
|              |               |            |       |            |       |        |       |       |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |   4.116 μs |  0.73 | 0.0229 |     - |     - |      72 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |   4.138 μs |  0.74 | 0.0229 |     - |     - |      72 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask | .NET Core 3.1 |            |   100 |   5.601 μs |  1.00 | 0.0229 |     - |     - |      72 B |
|      UniTask | .NET Core 3.1 |            |   100 |   7.103 μs |  1.27 | 0.0229 |     - |     - |      72 B |
|              |               |            |       |            |       |        |       |       |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |   4.835 μs |  0.92 | 0.0229 |     - |     - |      72 B |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |   4.914 μs |  0.94 | 0.0229 |     - |     - |      72 B |
|   RSGPromise |    CoreRt 3.1 |            |   100 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |   5.227 μs |  1.00 | 0.0229 |     - |     - |      72 B |
|      UniTask |    CoreRt 3.1 |            |   100 |   7.751 μs |  1.48 | 0.0153 |     - |     - |      72 B |
|              |               |            |       |            |       |        |       |       |           |
| ProtoPromise |          Mono |    No Pool |   100 |   6.284 μs |  0.76 | 0.0153 |     - |     - |         - |
| ProtoPromise |          Mono |       Pool |   100 |   6.184 μs |  0.75 | 0.0153 |     - |     - |         - |
|   RSGPromise |          Mono |            |   100 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask |          Mono |            |   100 |   8.275 μs |  1.00 | 0.0153 |     - |     - |         - |
|      UniTask |          Mono |            |   100 |         NA |     ? |      - |     - |     - |         - |
|              |               |            |       |            |       |        |       |       |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 406.796 μs |  0.75 |      - |     - |     - |      84 B |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 408.342 μs |  0.76 |      - |     - |     - |      84 B |
|   RSGPromise |      .NET 4.8 |            | 10000 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 539.924 μs |  1.00 |      - |     - |     - |      88 B |
|      UniTask |      .NET 4.8 |            | 10000 | 693.778 μs |  1.28 |      - |     - |     - |      88 B |
|              |               |            |       |            |       |        |       |       |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 396.821 μs |  0.77 |      - |     - |     - |      72 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 401.413 μs |  0.78 |      - |     - |     - |      72 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 514.887 μs |  1.00 |      - |     - |     - |      73 B |
|      UniTask | .NET Core 3.1 |            | 10000 | 686.039 μs |  1.33 |      - |     - |     - |      73 B |
|              |               |            |       |            |       |        |       |       |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 488.343 μs |  0.95 |      - |     - |     - |      72 B |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 482.725 μs |  0.94 |      - |     - |     - |      72 B |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 514.860 μs |  1.00 |      - |     - |     - |      72 B |
|      UniTask |    CoreRt 3.1 |            | 10000 | 759.730 μs |  1.48 |      - |     - |     - |      72 B |
|              |               |            |       |            |       |        |       |       |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 615.434 μs |  0.76 |      - |     - |     - |         - |
| ProtoPromise |          Mono |       Pool | 10000 | 601.373 μs |  0.74 |      - |     - |     - |         - |
|   RSGPromise |          Mono |            | 10000 |         NA |     ? |      - |     - |     - |         - |
|   DotNetTask |          Mono |            | 10000 | 807.658 μs |  1.00 |      - |     - |     - |         - |
|      UniTask |          Mono |            | 10000 |         NA |     ? |      - |     - |     - |         - |

Benchmarks with issues:
  AwaitResolved.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=100]
  AwaitResolved.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=100]
  AwaitResolved.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  AwaitResolved.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  AwaitResolved.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  AwaitResolved.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  AwaitResolved.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  AwaitResolved.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  AwaitResolved.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  AwaitResolved.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
```

### ContinueWithFromValue Benchmarks

This is testing this setup:

```
for (int i = 0; i < N; ++i)
{
    task = task
        .ContinueWith(_ => { })
        .ContinueWith(_ => vector)
        .ContinueWith(_ => obj);
}
```

Note: RSG Promises don't support all of the ContinueWith methods returning void/simple values, so extension methods were created for this purpose which may not be representative of the performance if they are added to the library natively.


```
|       Method |       Runtime | Categories |     N |         Mean | Ratio |    Gen 0 |    Gen 1 |   Gen 2 | Allocated |
|------------- |-------------- |----------- |------ |-------------:|------:|---------:|---------:|--------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    404.10 μs |  1.68 |  32.7148 |  16.1133 |  0.4883 |   25776 B |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |     88.81 μs |  0.37 |        - |        - |       - |         - |
|   RSGPromise |      .NET 4.8 |            |   100 |    759.32 μs |  3.14 | 125.0000 |  49.8047 |       - |  625125 B |
|   DotNetTask |      .NET 4.8 |            |   100 |    241.42 μs |  1.00 |  49.0723 |        - |       - |   38620 B |
|      UniTask |      .NET 4.8 |            |   100 |    106.48 μs |  0.44 |   0.1221 |        - |       - |     117 B |
|              |               |            |       |              |       |          |          |         |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    295.56 μs |  3.04 |   7.8125 |   3.9063 |       - |   25696 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |     80.87 μs |  0.83 |        - |        - |       - |         - |
|   RSGPromise | .NET Core 3.1 |            |   100 |    628.48 μs |  6.42 | 135.7422 |  41.9922 |       - |  609649 B |
|   DotNetTask | .NET Core 3.1 |            |   100 |     97.91 μs |  1.00 |  11.4746 |        - |       - |   36096 B |
|      UniTask | .NET Core 3.1 |            |   100 |     97.18 μs |  0.99 |        - |        - |       - |     104 B |
|              |               |            |       |              |       |          |          |         |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    146.73 μs |  1.41 |   8.0566 |   3.9063 |       - |   25696 B |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |     79.72 μs |  0.77 |        - |        - |       - |         - |
|   RSGPromise |    CoreRt 3.1 |            |   100 |    571.26 μs |  5.50 | 124.0234 |  47.8516 |       - |  547184 B |
|   DotNetTask |    CoreRt 3.1 |            |   100 |    104.13 μs |  1.00 |  11.4746 |        - |       - |   36096 B |
|      UniTask |    CoreRt 3.1 |            |   100 |           NA |     ? |        - |        - |       - |         - |
|              |               |            |       |              |       |          |          |         |           |
| ProtoPromise |          Mono |    No Pool |   100 |    260.67 μs |  1.28 |   5.3711 |   0.4883 |  0.4883 |         - |
| ProtoPromise |          Mono |       Pool |   100 |     81.53 μs |  0.40 |        - |        - |       - |         - |
|   RSGPromise |          Mono |            |   100 |  1,201.55 μs |  5.88 | 210.9375 |   3.9063 |  3.9063 |         - |
|   DotNetTask |          Mono |            |   100 |    205.25 μs |  1.00 |  19.5313 |        - |       - |         - |
|      UniTask |          Mono |            |   100 |           NA |     ? |        - |        - |       - |         - |
|              |               |            |       |              |       |          |          |         |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 35,203.57 μs |  1.20 | 400.0000 | 133.3333 | 66.6667 | 2568071 B |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 |  8,993.36 μs |  0.31 |        - |        - |       - |         - |
|   RSGPromise |      .NET 4.8 |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 29,692.75 μs |  1.00 | 625.0000 | 281.2500 |       - | 3851738 B |
|      UniTask |      .NET 4.8 |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|              |               |            |       |              |       |          |          |         |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 29,853.98 μs |  2.43 | 437.5000 | 156.2500 | 62.5000 | 2560098 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 |  7,766.94 μs |  0.64 |        - |        - |       - |         - |
|   RSGPromise | .NET Core 3.1 |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 12,158.59 μs |  1.00 | 593.7500 | 281.2500 |       - | 3600180 B |
|      UniTask | .NET Core 3.1 |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|              |               |            |       |              |       |          |          |         |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 15,669.47 μs |  1.14 | 546.8750 | 218.7500 | 78.1250 | 2560097 B |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 |  8,842.46 μs |  0.63 |        - |        - |       - |         - |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 13,943.99 μs |  1.00 | 609.3750 | 296.8750 |       - | 3600276 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|              |               |            |       |              |       |          |          |         |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 31,702.58 μs |     ? | 531.2500 |  62.5000 | 62.5000 |         - |
| ProtoPromise |          Mono |       Pool | 10000 |  8,047.73 μs |     ? |        - |        - |       - |         - |
|   RSGPromise |          Mono |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|   DotNetTask |          Mono |            | 10000 |           NA |     ? |        - |        - |       - |         - |
|      UniTask |          Mono |            | 10000 |           NA |     ? |        - |        - |       - |         - |

Benchmarks with issues:
  ContinueWithFromValue.UniTask: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  ContinueWithFromValue.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  ContinueWithFromValue.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.UniTask: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.UniTask: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.UniTask: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.DotNetTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  ContinueWithFromValue.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  ```

### ContinueWithPending Benchmarks

This is testing this setup:

```
for (int i = 0; i < N; ++i)
{
    int index = i;
    task = task
        .ContinueWith(_ => pendingTaskVoids[index]).Unwrap()
        .ContinueWith(_ => pendingTaskVectors[index]).Unwrap()
        .ContinueWith(_ => pendingTaskObjects[index]).Unwrap();
}
```


```
|       Method |       Runtime | Categories |     N |         Mean |       Median | Ratio |      Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|------------- |-------------- |----------- |------ |-------------:|-------------:|------:|-----------:|----------:|----------:|-----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |     387.0 μs |     386.6 μs |  1.06 |          - |         - |         - |    32768 B |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |     209.3 μs |     209.2 μs |  0.57 |          - |         - |         - |          - |
|   RSGPromise |      .NET 4.8 |            |   100 |     634.6 μs |     633.7 μs |  1.73 |  1000.0000 |         - |         - |   794712 B |
|   DotNetTask |      .NET 4.8 |            |   100 |     367.1 μs |     367.0 μs |  1.00 |          - |         - |         - |    90112 B |
|      UniTask |      .NET 4.8 |            |   100 |     197.4 μs |     196.2 μs |  0.54 |          - |         - |         - |    32768 B |
|              |               |            |       |              |              |       |            |           |           |            |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |     468.2 μs |     466.4 μs |  1.55 |          - |         - |         - |    28096 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |     368.5 μs |     368.4 μs |  1.22 |          - |         - |         - |          - |
|   RSGPromise | .NET Core 3.1 |            |   100 |     884.0 μs |     876.7 μs |  2.92 |          - |         - |         - |   764848 B |
|   DotNetTask | .NET Core 3.1 |            |   100 |     302.2 μs |     300.7 μs |  1.00 |          - |         - |         - |    81696 B |
|      UniTask | .NET Core 3.1 |            |   100 |     295.2 μs |     296.2 μs |  0.98 |          - |         - |         - |    28904 B |
|              |               |            |       |              |              |       |            |           |           |            |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |     190.3 μs |     190.0 μs |  0.90 |          - |         - |         - |    28096 B |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |     186.5 μs |     186.0 μs |  0.89 |          - |         - |         - |          - |
|   RSGPromise |    CoreRt 3.1 |            |   100 |     472.3 μs |     472.8 μs |  2.24 |          - |         - |         - |   692784 B |
|   DotNetTask |    CoreRt 3.1 |            |   100 |     210.4 μs |     210.4 μs |  1.00 |          - |         - |         - |    76896 B |
|      UniTask |    CoreRt 3.1 |            |   100 |           NA |           NA |     ? |          - |         - |         - |          - |
|              |               |            |       |              |              |       |            |           |           |            |
| ProtoPromise |          Mono |    No Pool |   100 |     197.9 μs |     197.5 μs |  0.59 |          - |         - |         - |          - |
| ProtoPromise |          Mono |       Pool |   100 |     195.7 μs |     189.0 μs |  0.59 |          - |         - |         - |          - |
|   RSGPromise |          Mono |            |   100 |   1,143.1 μs |   1,142.1 μs |  3.47 |          - |         - |         - |          - |
|   DotNetTask |          Mono |            |   100 |     333.3 μs |     330.9 μs |  1.00 |          - |         - |         - |          - |
|      UniTask |          Mono |            |   100 |           NA |           NA |     ? |          - |         - |         - |          - |
|              |               |            |       |              |              |       |            |           |           |            |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 |  40,358.3 μs |  40,445.6 μs |  0.72 |          - |         - |         - |  2812464 B |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 |  20,504.8 μs |  20,229.7 μs |  0.36 |          - |         - |         - |          - |
|   RSGPromise |      .NET 4.8 |            | 10000 | 571,049.8 μs | 574,072.2 μs | 10.12 | 13000.0000 | 5000.0000 | 1000.0000 | 78876112 B |
|   DotNetTask |      .NET 4.8 |            | 10000 |  56,711.0 μs |  56,750.4 μs |  1.00 |  1000.0000 |         - |         - |  8667016 B |
|      UniTask |      .NET 4.8 |            | 10000 |  31,390.7 μs |  31,736.0 μs |  0.55 |          - |         - |         - |  2895744 B |
|              |               |            |       |              |              |       |            |           |           |            |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 |  28,644.9 μs |  28,631.7 μs |  0.96 |          - |         - |         - |  2800096 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 |  19,870.5 μs |  19,661.9 μs |  0.70 |          - |         - |         - |          - |
|   RSGPromise | .NET Core 3.1 |            | 10000 | 466,949.1 μs | 470,638.7 μs | 15.73 | 13000.0000 | 4000.0000 | 1000.0000 | 76402192 B |
|   DotNetTask | .NET Core 3.1 |            | 10000 |  29,660.6 μs |  29,662.7 μs |  1.00 |  1000.0000 |         - |         - |  8160096 B |
|      UniTask | .NET Core 3.1 |            | 10000 |  22,736.7 μs |  23,063.5 μs |  0.77 |          - |         - |         - |  2880104 B |
|              |               |            |       |              |              |       |            |           |           |            |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 |  18,571.4 μs |  18,598.1 μs |  0.67 |          - |         - |         - |  2800096 B |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 |  18,990.1 μs |  19,060.8 μs |  0.68 |          - |         - |         - |          - |
|   RSGPromise |    CoreRt 3.1 |            | 10000 | 447,254.5 μs | 435,605.5 μs | 16.97 | 12000.0000 | 4000.0000 | 1000.0000 | 69200784 B |
|   DotNetTask |    CoreRt 3.1 |            | 10000 |  27,753.0 μs |  27,774.6 μs |  1.00 |  1000.0000 |         - |         - |  7680096 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |           NA |           NA |     ? |          - |         - |         - |          - |
|              |               |            |       |              |              |       |            |           |           |            |
| ProtoPromise |          Mono |    No Pool | 10000 |  23,091.0 μs |  23,085.7 μs |  0.33 |  1000.0000 |         - |         - |          - |
| ProtoPromise |          Mono |       Pool | 10000 |  20,469.3 μs |  20,519.1 μs |  0.29 |          - |         - |         - |          - |
|   RSGPromise |          Mono |            | 10000 | 357,762.7 μs | 357,007.1 μs |  5.12 | 24000.0000 | 3000.0000 | 3000.0000 |          - |
|   DotNetTask |          Mono |            | 10000 |  69,863.9 μs |  69,616.6 μs |  1.00 |  5000.0000 | 1000.0000 | 1000.0000 |          - |
|      UniTask |          Mono |            | 10000 |           NA |           NA |     ? |          - |         - |         - |          - |

Benchmarks with issues:
  ContinueWithPending.UniTask: Job-LHARXI(Runtime=CoreRt 3.1, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=100]
  ContinueWithPending.UniTask: Job-QFEDFF(Runtime=Mono, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=100]
  ContinueWithPending.UniTask: Job-LHARXI(Runtime=CoreRt 3.1, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=10000]
  ContinueWithPending.UniTask: Job-QFEDFF(Runtime=Mono, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=10000]
```

### ContinueWithResolved Benchmarks

This is testing this setup:

```
for (int i = 0; i < N; ++i)
{
    task = task
        .ContinueWith(_ => completedTaskVoid).Unwrap()
        .ContinueWith(_ => completedTaskVector).Unwrap()
        .ContinueWith(_ => completedTaskObject).Unwrap();
}
```


```
|       Method |       Runtime | Categories |     N |        Mean |      Median | Ratio |     Gen 0 |    Gen 1 |   Gen 2 | Allocated |
|------------- |-------------- |----------- |------ |------------:|------------:|------:|----------:|---------:|--------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    395.1 μs |    390.3 μs |  1.08 |   21.4844 |  10.7422 |  0.4883 |   16950 B |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |    119.8 μs |    119.8 μs |  0.31 |         - |        - |       - |         - |
|   RSGPromise |      .NET 4.8 |            |   100 |    616.0 μs |    615.1 μs |  1.62 |  121.0938 |  49.8047 |       - |  581799 B |
|   DotNetTask |      .NET 4.8 |            |   100 |    380.9 μs |    380.9 μs |  1.00 |   78.6133 |   3.4180 |       - |   65100 B |
|      UniTask |      .NET 4.8 |            |   100 |    133.3 μs |    133.1 μs |  0.35 |         - |        - |       - |     118 B |
|              |               |            |       |             |             |       |           |          |         |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    295.5 μs |    293.2 μs |  1.70 |    5.3711 |   2.4414 |       - |   16896 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |    100.7 μs |    100.1 μs |  0.58 |         - |        - |       - |         - |
|   RSGPromise | .NET Core 3.1 |            |   100 |    623.0 μs |    623.3 μs |  3.58 |   99.6094 |  43.9453 |       - |  566451 B |
|   DotNetTask | .NET Core 3.1 |            |   100 |    174.0 μs |    174.2 μs |  1.00 |   19.0430 |        - |       - |   60096 B |
|      UniTask | .NET Core 3.1 |            |   100 |    121.6 μs |    121.6 μs |  0.70 |         - |        - |       - |     104 B |
|              |               |            |       |             |             |       |           |          |         |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    175.9 μs |    175.8 μs |  0.92 |    5.3711 |   2.6855 |       - |   16896 B |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |    113.7 μs |    113.7 μs |  0.59 |         - |        - |       - |         - |
|   RSGPromise |    CoreRt 3.1 |            |   100 |    524.2 μs |    524.2 μs |  2.73 |  121.0938 |  30.2734 |       - |  508784 B |
|   DotNetTask |    CoreRt 3.1 |            |   100 |    191.8 μs |    191.8 μs |  1.00 |   19.0430 |        - |       - |   60096 B |
|      UniTask |    CoreRt 3.1 |            |   100 |          NA |          NA |     ? |         - |        - |       - |         - |
|              |               |            |       |             |             |       |           |          |         |           |
| ProtoPromise |          Mono |    No Pool |   100 |    288.2 μs |    288.1 μs |  0.75 |    3.4180 |   0.4883 |  0.4883 |         - |
| ProtoPromise |          Mono |       Pool |   100 |    100.1 μs |    100.1 μs |  0.26 |         - |        - |       - |         - |
|   RSGPromise |          Mono |            |   100 |  1,259.1 μs |  1,257.5 μs |  3.27 |  193.3594 |   5.8594 |  5.8594 |         - |
|   DotNetTask |          Mono |            |   100 |    385.1 μs |    385.1 μs |  1.00 |   36.6211 |        - |       - |         - |
|      UniTask |          Mono |            |   100 |          NA |          NA |     ? |         - |        - |       - |         - |
|              |               |            |       |             |             |       |           |          |         |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 34,594.6 μs | 34,674.2 μs |  0.73 |  312.5000 | 125.0000 | 62.5000 | 1685240 B |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 12,239.9 μs | 12,204.5 μs |  0.26 |         - |        - |       - |         - |
|   RSGPromise |      .NET 4.8 |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 48,596.4 μs | 49,773.6 μs |  1.00 | 1083.3333 | 500.0000 |       - | 6499986 B |
|      UniTask |      .NET 4.8 |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|              |               |            |       |             |             |       |           |          |         |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 29,253.2 μs | 29,302.2 μs |  1.21 |  312.5000 |  93.7500 | 31.2500 | 1680096 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 11,170.0 μs | 11,156.9 μs |  0.46 |         - |        - |       - |         - |
|   RSGPromise | .NET Core 3.1 |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 24,097.0 μs | 24,169.1 μs |  1.00 | 1000.0000 | 500.0000 |       - | 6000480 B |
|      UniTask | .NET Core 3.1 |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|              |               |            |       |             |             |       |           |          |         |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 15,644.4 μs | 15,636.0 μs |  0.65 |  312.5000 |  93.7500 | 31.2500 | 1680096 B |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 11,607.3 μs | 11,595.0 μs |  0.48 |         - |        - |       - |         - |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 24,213.7 μs | 24,171.4 μs |  1.00 | 1000.0000 | 500.0000 |       - | 6001085 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|              |               |            |       |             |             |       |           |          |         |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 30,135.1 μs | 30,099.3 μs |     ? |  312.5000 |  62.5000 | 62.5000 |         - |
| ProtoPromise |          Mono |       Pool | 10000 | 10,095.3 μs | 10,095.8 μs |     ? |         - |        - |       - |         - |
|   RSGPromise |          Mono |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|   DotNetTask |          Mono |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |
|      UniTask |          Mono |            | 10000 |          NA |          NA |     ? |         - |        - |       - |         - |

Benchmarks with issues:
  ContinueWithResolved.UniTask: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  ContinueWithResolved.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  ContinueWithResolved.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.UniTask: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.UniTask: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.UniTask: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.DotNetTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  ContinueWithResolved.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
```