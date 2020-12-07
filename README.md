# C# Asynchronous Libraries Benchmarks

This project is built to benchmark C# asynchronous libraries against each other.

These benchmarks are ran using [BenchmarkDotNet v0.12.1](https://github.com/dotnet/BenchmarkDotNet).

This is a non-exhaustive benchmark of each library's features. It is only meant to compare the most common use scenarios. Some libraries provide more/different features than others.

Asynchronous libraries benchmarked:
- [System.Threading.Tasks (TPL)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) labeled as `DotNetTask` - this was measured as the baseline.
- [ProtoPromise v1.0.0](https://github.com/timcassell/ProtoPromise)
- [UniTask v2.0.37](https://github.com/Cysharp/UniTask)
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
BenchmarkDotNet=v0.12.1.20201206-develop, OS=Windows 7 SP1 (6.1.7601.0)
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

Note: I removed multiple columns from the tables to condense them and make them easier to read. Those columns will show in your local benchmarks results if you run them on your machine.

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


|       Method |       Runtime | Categories |     N |        Mean | Ratio | Allocated |  Survived |
|------------- |-------------- |----------- |------ |------------:|------:|----------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    229.9 μs |  1.07 |   65536 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |    114.4 μs |  0.53 |         - |   52888 B |
|   RSGPromise |      .NET 4.8 |            |   100 |          NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |    214.4 μs |  1.00 |   73728 B |    9504 B |
|      UniTask |      .NET 4.8 |            |   100 |    151.1 μs |  0.71 |         - |   80424 B |
|              |               |            |       |             |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    301.0 μs |  1.43 |   61600 B |     320 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |    232.6 μs |  1.13 |         - |   53344 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |          NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            |   100 |    210.8 μs |  1.00 |   65600 B |    9736 B |
|      UniTask | .NET Core 3.1 |            |   100 |    308.7 μs |  1.49 |         - |   88624 B |
|              |               |            |       |             |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    111.9 μs |  1.05 |   56800 B |         - |
|   RSGPromise |    CoreRt 3.1 |            |   100 |          NA |     ? |         - |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |    109.9 μs |  1.03 |         - |   48088 B |
|   DotNetTask |    CoreRt 3.1 |            |   100 |    106.3 μs |  1.00 |   48800 B |    9504 B |
|      UniTask |    CoreRt 3.1 |            |   100 |          NA |     ? |         - |         - |
|              |               |            |       |             |       |           |           |
| ProtoPromise |          Mono |    No Pool |   100 |    443.4 μs |  2.39 |         - |         - |
| ProtoPromise |          Mono |       Pool |   100 |    126.6 μs |  0.68 |         - |         - |
|   RSGPromise |          Mono |            |   100 |          NA |     ? |         - |         - |
|   DotNetTask |          Mono |            |   100 |    185.2 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            |   100 |    194.9 μs |  1.02 |         - |         - |
|              |               |            |       |             |       |           |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 26,730.3 μs |  1.06 | 6185200 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 13,547.3 μs |  0.54 |         - | 5280088 B |
|   RSGPromise |      .NET 4.8 |            | 10000 |          NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 25,102.5 μs |  1.00 | 7060824 B |  959904 B |
|      UniTask |      .NET 4.8 |            | 10000 | 19,924.7 μs |  0.80 |  380496 B | 8133624 B |
|              |               |            |       |             |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 25,407.7 μs |  1.56 | 6160000 B |     344 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 12,699.3 μs |  0.78 |         - | 5280544 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 |          NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 16,284.4 μs |  1.00 | 5360000 B |  960136 B |
|      UniTask | .NET Core 3.1 |            | 10000 | 20,031.2 μs |  1.22 |         - | 8549776 B |
|              |               |            |       |             |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 16,851.9 μs |  1.16 | 5680000 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 11,769.1 μs |  0.82 |         - | 4800088 B |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |          NA |     ? |         - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 14,496.0 μs |  1.00 | 4880000 B |  959904 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |          NA |     ? |         - |         - |
|              |               |            |       |             |       |           |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 61,727.9 μs |  1.60 |         - |         - |
| ProtoPromise |          Mono |       Pool | 10000 | 13,816.0 μs |  0.36 |         - |         - |
|   RSGPromise |          Mono |            | 10000 |          NA |     ? |         - |         - |
|   DotNetTask |          Mono |            | 10000 | 38,681.3 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            | 10000 | 22,636.3 μs |  0.59 |         - |         - |

```
Benchmarks with issues:
  AsyncPending.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=100]
  AsyncPending.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=100]
  AsyncPending.UniTask: Job-LHARXI(Runtime=CoreRt 3.1, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=100]
  AsyncPending.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=100]
  AsyncPending.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  AsyncPending.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  AsyncPending.RSGPromise: Job-LKAUXA(Runtime=.NET Core 3.1, RunStrategy=Throughput) [N=10000]
  AsyncPending.UniTask: Job-LHARXI(Runtime=CoreRt 3.1, InvocationCount=1, RunStrategy=Throughput, UnrollFactor=1) [N=10000]
  AsyncPending.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
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


|       Method |       Runtime | Categories |     N |         Mean | Ratio | Allocated |  Survived |
|------------- |-------------- |----------- |------ |-------------:|------:|----------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    271.80 μs |  8.21 |   20062 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |     43.49 μs |  1.31 |         - |   20000 B |
|   RSGPromise |      .NET 4.8 |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |     33.16 μs |  1.00 |   16852 B |         - |
|      UniTask |      .NET 4.8 |            |   100 |     22.14 μs |  0.67 |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    216.27 μs | 13.91 |   20000 B |     208 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |     37.42 μs |  2.41 |         - |   20264 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            |   100 |     15.55 μs |  1.00 |   15200 B |         - |
|      UniTask | .NET Core 3.1 |            |   100 |     21.97 μs |  1.41 |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    116.43 μs |  9.38 |   20000 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |     31.49 μs |  2.54 |         - |   20000 B |
|   RSGPromise |    CoreRt 3.1 |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |     12.41 μs |  1.00 |   15200 B |         - |
|      UniTask |    CoreRt 3.1 |            |   100 |     15.09 μs |  1.22 |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool |   100 |    185.01 μs |  4.58 |         - |         - |
| ProtoPromise |          Mono |       Pool |   100 |     51.74 μs |  1.28 |         - |         - |
|   RSGPromise |          Mono |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask |          Mono |            |   100 |     40.41 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            |   100 |     17.27 μs |  0.43 |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 26,176.70 μs |  7.89 | 2006042 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 |  4,860.33 μs |  1.47 |         - | 2000000 B |
|   RSGPromise |      .NET 4.8 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 |  3,310.42 μs |  1.00 | 1685159 B |         - |
|      UniTask |      .NET 4.8 |            | 10000 |  2,211.36 μs |  0.67 |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 23,608.72 μs | 15.34 | 2000040 B |     256 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 |  4,042.72 μs |  2.62 |         - | 2000288 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 |  1,542.57 μs |  1.00 | 1520001 B |         - |
|      UniTask | .NET Core 3.1 |            | 10000 |  2,158.23 μs |  1.40 |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 12,153.33 μs |  9.73 | 2000000 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 |  3,408.11 μs |  2.73 |         - | 2000000 B |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 |  1,248.77 μs |  1.00 | 1520000 B |         - |
|      UniTask |    CoreRt 3.1 |            | 10000 |  1,511.84 μs |  1.21 |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 19,777.21 μs |  4.91 |         - |         - |
| ProtoPromise |          Mono |       Pool | 10000 |  5,342.93 μs |  1.33 |         - |         - |
|   RSGPromise |          Mono |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |          Mono |            | 10000 |  4,031.19 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            | 10000 |  1,706.14 μs |  0.42 |         - |         - |

```
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


|       Method |       Runtime | Categories |     N |         Mean | Ratio | Allocated |  Survived |
|------------- |-------------- |----------- |------ |-------------:|------:|----------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    296.12 μs |  1.95 |   24576 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |    256.89 μs |  1.69 |         - |     120 B |
|   RSGPromise |      .NET 4.8 |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |    152.09 μs |  1.00 |         - |         - |
|      UniTask |      .NET 4.8 |            |   100 |     29.71 μs |  0.20 |         - |    7200 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    300.91 μs |  3.09 |   18632 B |     400 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |    312.22 μs |  3.22 |         - |     656 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            |   100 |     97.14 μs |  1.00 |         - |         - |
|      UniTask | .NET Core 3.1 |            |   100 |     27.73 μs |  0.29 |         - |    7200 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    125.66 μs |  1.54 |   18568 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |    139.44 μs |  1.71 |         - |     120 B |
|   RSGPromise |    CoreRt 3.1 |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |     81.50 μs |  1.00 |         - |         - |
|      UniTask |    CoreRt 3.1 |            |   100 |     72.09 μs |  0.88 |   19272 B |   28416 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool |   100 |    160.13 μs |  1.58 |         - |         - |
| ProtoPromise |          Mono |       Pool |   100 |    190.34 μs |  1.87 |         - |         - |
|   RSGPromise |          Mono |            |   100 |           NA |     ? |         - |         - |
|   DotNetTask |          Mono |            |   100 |    101.69 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            |   100 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 31,033.57 μs |  2.14 | 1853568 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 24,930.18 μs |  1.71 |         - |     120 B |
|   RSGPromise |      .NET 4.8 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 14,511.81 μs |  1.00 |         - |         - |
|      UniTask |      .NET 4.8 |            | 10000 |  3,128.07 μs |  0.22 |  726192 B |  720000 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 18,263.09 μs |  2.76 | 1840232 B |     376 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 17,875.02 μs |  2.70 |         - |     656 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 |  6,625.51 μs |  1.00 |         - |         - |
|      UniTask | .NET Core 3.1 |            | 10000 |  2,717.51 μs |  0.41 |  720184 B |  720000 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 12,098.17 μs |  1.53 | 1840168 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 13,864.35 μs |  1.75 |         - |     120 B |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 |  7,918.12 μs |  1.00 |         - |         - |
|      UniTask |    CoreRt 3.1 |            | 10000 |  6,484.53 μs |  0.82 | 1920072 B | 2703360 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 15,471.62 μs |  1.55 |         - |         - |
| ProtoPromise |          Mono |       Pool | 10000 | 17,819.19 μs |  1.78 |         - |         - |
|   RSGPromise |          Mono |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |          Mono |            | 10000 | 10,005.38 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            | 10000 |           NA |     ? |         - |         - |

```
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


|       Method |       Runtime | Categories |     N |       Mean | Ratio | Allocated | Survived |
|------------- |-------------- |----------- |------ |-----------:|------:|----------:|---------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |   4.415 μs |  0.78 |      80 B |        - |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |   4.414 μs |  0.78 |      80 B |        - |
|   RSGPromise |      .NET 4.8 |            |   100 |         NA |     ? |         - |        - |
|   DotNetTask |      .NET 4.8 |            |   100 |   5.642 μs |  1.00 |      80 B |        - |
|      UniTask |      .NET 4.8 |            |   100 |   6.711 μs |  1.19 |      80 B |        - |
|              |               |            |       |            |       |           |          |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |   4.177 μs |  0.73 |      72 B |     48 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |   4.119 μs |  0.72 |      72 B |     48 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |         NA |     ? |         - |        - |
|   DotNetTask | .NET Core 3.1 |            |   100 |   5.712 μs |  1.00 |      72 B |        - |
|      UniTask | .NET Core 3.1 |            |   100 |   7.116 μs |  1.25 |      72 B |    240 B |
|              |               |            |       |            |       |           |          |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |   5.725 μs |  1.02 |      72 B |        - |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |   5.707 μs |  1.01 |      72 B |        - |
|   RSGPromise |    CoreRt 3.1 |            |   100 |         NA |     ? |         - |        - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |   5.622 μs |  1.00 |      72 B |        - |
|      UniTask |    CoreRt 3.1 |            |   100 |   7.649 μs |  1.36 |      72 B |        - |
|              |               |            |       |            |       |           |          |
| ProtoPromise |          Mono |    No Pool |   100 |   6.286 μs |  0.75 |         - |        - |
| ProtoPromise |          Mono |       Pool |   100 |   6.286 μs |  0.75 |         - |        - |
|   RSGPromise |          Mono |            |   100 |         NA |     ? |         - |        - |
|   DotNetTask |          Mono |            |   100 |   8.437 μs |  1.00 |         - |        - |
|      UniTask |          Mono |            |   100 |         NA |     ? |         - |        - |
|              |               |            |       |            |       |           |          |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 401.169 μs |  0.73 |      84 B |        - |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 426.140 μs |  0.77 |      84 B |        - |
|   RSGPromise |      .NET 4.8 |            | 10000 |         NA |     ? |         - |        - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 551.113 μs |  1.00 |      88 B |        - |
|      UniTask |      .NET 4.8 |            | 10000 | 655.881 μs |  1.19 |      88 B |        - |
|              |               |            |       |            |       |           |          |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 404.570 μs |  0.72 |      72 B |     48 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 404.623 μs |  0.72 |      73 B |     48 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 |         NA |     ? |         - |        - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 558.252 μs |  1.00 |      73 B |        - |
|      UniTask | .NET Core 3.1 |            | 10000 | 683.573 μs |  1.22 |      72 B |    240 B |
|              |               |            |       |            |       |           |          |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 552.486 μs |  1.00 |      72 B |        - |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 535.879 μs |  0.97 |      72 B |        - |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |         NA |     ? |         - |        - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 554.619 μs |  1.00 |      72 B |        - |
|      UniTask |    CoreRt 3.1 |            | 10000 | 759.122 μs |  1.36 |      72 B |        - |
|              |               |            |       |            |       |           |          |
| ProtoPromise |          Mono |    No Pool | 10000 | 605.220 μs |  0.72 |         - |        - |
| ProtoPromise |          Mono |       Pool | 10000 | 612.850 μs |  0.73 |         - |        - |
|   RSGPromise |          Mono |            | 10000 |         NA |     ? |         - |        - |
|   DotNetTask |          Mono |            | 10000 | 838.295 μs |  1.00 |         - |        - |
|      UniTask |          Mono |            | 10000 |         NA |     ? |         - |        - |

```
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


|       Method |       Runtime | Categories |     N |         Mean | Ratio | Allocated |  Survived |
|------------- |-------------- |----------- |------ |-------------:|------:|----------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    389.47 μs |  1.62 |   25775 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |     86.29 μs |  0.36 |         - |   16944 B |
|   RSGPromise |      .NET 4.8 |            |   100 |    671.08 μs |  2.79 |  625122 B |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |    240.32 μs |  1.00 |   38620 B |         - |
|      UniTask |      .NET 4.8 |            |   100 |    104.84 μs |  0.44 |     117 B |   72000 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    292.25 μs |  2.93 |   25696 B |     320 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |     90.62 μs |  0.90 |         - |   17424 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |    631.17 μs |  6.30 |  609649 B |     320 B |
|   DotNetTask | .NET Core 3.1 |            |   100 |    100.08 μs |  1.00 |   36096 B |         - |
|      UniTask | .NET Core 3.1 |            |   100 |     99.19 μs |  0.99 |     106 B |   73000 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    154.23 μs |  1.46 |   25696 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |     84.73 μs |  0.81 |         - |   16944 B |
|   RSGPromise |    CoreRt 3.1 |            |   100 |    587.60 μs |  5.63 |  547184 B |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |    105.54 μs |  1.00 |   36096 B |         - |
|      UniTask |    CoreRt 3.1 |            |   100 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool |   100 |    272.48 μs |  1.34 |         - |         - |
| ProtoPromise |          Mono |       Pool |   100 |     78.23 μs |  0.39 |         - |         - |
|   RSGPromise |          Mono |            |   100 |  1,581.83 μs |  7.77 |         - |         - |
|   DotNetTask |          Mono |            |   100 |    202.70 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            |   100 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 42,032.47 μs |  1.60 | 2567934 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 |  8,987.08 μs |  0.32 |         - | 1680144 B |
|   RSGPromise |      .NET 4.8 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 28,016.32 μs |  1.00 | 3851634 B |  270979 B |
|      UniTask |      .NET 4.8 |            | 10000 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 30,710.10 μs |  2.56 | 2560096 B |     344 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 |  8,316.68 μs |  0.69 |         - | 1680600 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 12,038.59 μs |  1.00 | 3600170 B |   43280 B |
|      UniTask | .NET Core 3.1 |            | 10000 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 14,440.66 μs |  1.14 | 2560195 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 |  8,455.55 μs |  0.67 |         - | 1680144 B |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 12,661.15 μs |  1.00 | 3600277 B |   33608 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 30,705.97 μs |     ? |         - |         - |
| ProtoPromise |          Mono |       Pool | 10000 |  7,898.23 μs |     ? |         - |         - |
|   RSGPromise |          Mono |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |          Mono |            | 10000 |           NA |     ? |         - |         - |
|      UniTask |          Mono |            | 10000 |           NA |     ? |         - |         - |

```
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


|       Method |       Runtime | Categories |     N |         Mean | Ratio |  Allocated |  Survived |
|------------- |-------------- |----------- |------ |-------------:|------:|-----------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |     379.4 μs |  1.02 |    32768 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |     211.8 μs |  0.57 |          - |   19344 B |
|   RSGPromise |      .NET 4.8 |            |   100 |     633.0 μs |  1.69 |   794712 B |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |     373.8 μs |  1.00 |    90112 B |         - |
|      UniTask |      .NET 4.8 |            |   100 |     201.8 μs |  0.54 |    32768 B |   86472 B |
|              |               |            |       |              |       |            |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |     463.1 μs |  1.74 |    28096 B |     296 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |     360.8 μs |  1.36 |          - |   19776 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |     870.0 μs |  3.27 |   764848 B |     320 B |
|   DotNetTask | .NET Core 3.1 |            |   100 |     266.0 μs |  1.00 |    81696 B |      32 B |
|      UniTask | .NET Core 3.1 |            |   100 |     296.0 μs |  1.11 |    28904 B |   87496 B |
|              |               |            |       |              |       |            |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |     186.1 μs |  0.88 |    28096 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |     183.4 μs |  0.87 |          - |   19344 B |
|   RSGPromise |    CoreRt 3.1 |            |   100 |     467.1 μs |  2.21 |   692784 B |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |     211.1 μs |  1.00 |    76896 B |         - |
|      UniTask |    CoreRt 3.1 |            |   100 |           NA |     ? |          - |         - |
|              |               |            |       |              |       |            |           |
| ProtoPromise |          Mono |    No Pool |   100 |     215.0 μs |  0.66 |          - |         - |
| ProtoPromise |          Mono |       Pool |   100 |     201.2 μs |  0.63 |          - |         - |
|   RSGPromise |          Mono |            |   100 |   1,093.0 μs |  3.35 |          - |         - |
|   DotNetTask |          Mono |            |   100 |     327.3 μs |  1.00 |          - |         - |
|      UniTask |          Mono |            |   100 |           NA |     ? |          - |         - |
|              |               |            |       |              |       |            |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 |  39,850.8 μs |  0.76 |  2812648 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 |  21,038.0 μs |  0.40 |          - | 1920144 B |
|   RSGPromise |      .NET 4.8 |            | 10000 | 480,111.5 μs |  9.18 | 78958072 B |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 |  52,345.3 μs |  1.00 |  8671760 B |         - |
|      UniTask |      .NET 4.8 |            | 10000 |  27,409.5 μs |  0.52 |  2891280 B | 8640072 B |
|              |               |            |       |              |       |            |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 |  29,663.4 μs |  1.13 |  2800096 B |     320 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 |  17,959.5 μs |  0.69 |          - | 1920576 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 | 460,456.7 μs | 17.49 | 76400872 B |    3648 B |
|   DotNetTask | .NET Core 3.1 |            | 10000 |  26,334.3 μs |  1.00 |  8160096 B |      32 B |
|      UniTask | .NET Core 3.1 |            | 10000 |  21,278.9 μs |  0.81 |  2880104 B | 8641112 B |
|              |               |            |       |              |       |            |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 |  18,910.0 μs |  0.70 |  2800096 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 |  18,807.2 μs |  0.69 |          - | 1920144 B |
|   RSGPromise |    CoreRt 3.1 |            | 10000 | 426,938.1 μs | 15.77 | 69200784 B |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 |  27,070.5 μs |  1.00 |  7680096 B |         - |
|      UniTask |    CoreRt 3.1 |            | 10000 |           NA |     ? |          - |         - |
|              |               |            |       |              |       |            |           |
| ProtoPromise |          Mono |    No Pool | 10000 |  22,933.2 μs |  0.34 |          - |         - |
| ProtoPromise |          Mono |       Pool | 10000 |  19,458.2 μs |  0.29 |          - |         - |
|   RSGPromise |          Mono |            | 10000 | 349,842.1 μs |  5.24 |          - |         - |
|   DotNetTask |          Mono |            | 10000 |  66,890.8 μs |  1.00 |          - |         - |
|      UniTask |          Mono |            | 10000 |           NA |     ? |          - |         - |

```
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


|       Method |       Runtime | Categories |     N |         Mean | Ratio | Allocated |  Survived |
|------------- |-------------- |----------- |------ |-------------:|------:|----------:|----------:|
| ProtoPromise |      .NET 4.8 |    No Pool |   100 |    372.49 μs |  1.00 |   16950 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool |   100 |    132.77 μs |  0.36 |         - |   16896 B |
|   RSGPromise |      .NET 4.8 |            |   100 |    580.81 μs |  1.56 |  581797 B |         - |
|   DotNetTask |      .NET 4.8 |            |   100 |    371.13 μs |  1.00 |   65102 B |         - |
|      UniTask |      .NET 4.8 |            |   100 |    131.66 μs |  0.35 |     118 B |   79200 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool |   100 |    292.73 μs |  1.73 |   16896 B |     248 B |
| ProtoPromise | .NET Core 3.1 |       Pool |   100 |    114.16 μs |  0.68 |         - |   17144 B |
|   RSGPromise | .NET Core 3.1 |            |   100 |    590.79 μs |  3.49 |  566449 B |     320 B |
|   DotNetTask | .NET Core 3.1 |            |   100 |    169.08 μs |  1.00 |   60096 B |     152 B |
|      UniTask | .NET Core 3.1 |            |   100 |    124.86 μs |  0.74 |     104 B |   80280 B |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool |   100 |    183.01 μs |  0.98 |   16896 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool |   100 |    122.32 μs |  0.66 |         - |   16896 B |
|   RSGPromise |    CoreRt 3.1 |            |   100 |    505.50 μs |  2.71 |  508784 B |         - |
|   DotNetTask |    CoreRt 3.1 |            |   100 |    186.33 μs |  1.00 |   60096 B |         - |
|      UniTask |    CoreRt 3.1 |            |   100 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool |   100 |    278.96 μs |  0.74 |         - |         - |
| ProtoPromise |          Mono |       Pool |   100 |     94.59 μs |  0.25 |         - |         - |
|   RSGPromise |          Mono |            |   100 |  1,215.94 μs |  3.21 |         - |         - |
|   DotNetTask |          Mono |            |   100 |    378.35 μs |  1.00 |         - |         - |
|      UniTask |          Mono |            |   100 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |      .NET 4.8 |    No Pool | 10000 | 33,859.22 μs |  0.76 | 1685554 B |         - |
| ProtoPromise |      .NET 4.8 |       Pool | 10000 | 13,693.31 μs |  0.31 |         - | 1680096 B |
|   RSGPromise |      .NET 4.8 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |      .NET 4.8 |            | 10000 | 44,841.43 μs |  1.00 | 6515837 B |  230035 B |
|      UniTask |      .NET 4.8 |            | 10000 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise | .NET Core 3.1 |    No Pool | 10000 | 30,235.46 μs |  1.28 | 1680096 B |     272 B |
| ProtoPromise | .NET Core 3.1 |       Pool | 10000 | 11,057.63 μs |  0.48 |         - | 1680344 B |
|   RSGPromise | .NET Core 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask | .NET Core 3.1 |            | 10000 | 23,613.79 μs |  1.00 | 6000642 B |   11248 B |
|      UniTask | .NET Core 3.1 |            | 10000 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |    CoreRt 3.1 |    No Pool | 10000 | 16,144.91 μs |  0.66 | 1680096 B |         - |
| ProtoPromise |    CoreRt 3.1 |       Pool | 10000 | 12,634.53 μs |  0.52 |         - | 1680096 B |
|   RSGPromise |    CoreRt 3.1 |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |    CoreRt 3.1 |            | 10000 | 24,400.17 μs |  1.00 | 6000378 B |    7264 B |
|      UniTask |    CoreRt 3.1 |            | 10000 |           NA |     ? |         - |         - |
|              |               |            |       |              |       |           |           |
| ProtoPromise |          Mono |    No Pool | 10000 | 29,006.18 μs |     ? |         - |         - |
| ProtoPromise |          Mono |       Pool | 10000 |  9,566.34 μs |     ? |         - |         - |
|   RSGPromise |          Mono |            | 10000 |           NA |     ? |         - |         - |
|   DotNetTask |          Mono |            | 10000 |           NA |     ? |         - |         - |
|      UniTask |          Mono |            | 10000 |           NA |     ? |         - |         - |

```
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