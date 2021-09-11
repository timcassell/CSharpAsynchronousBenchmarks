# C# Asynchronous Libraries Benchmarks

This project is built to benchmark C# asynchronous libraries against each other.

These benchmarks are ran using a fork of [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) where I added the ability to measure survived memory.

This is a non-exhaustive benchmark of each library's features. It is only meant to compare the most common use scenario (awaiting an asynchronous result). Some libraries provide more/different features than others.

Asynchronous libraries benchmarked:
- Built-in callbacks (`System.Action`, `System.Func<T>`), measured as the baseline
- [System.Threading.Tasks (TPL)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl), `Task` and `ValueTask`
- [ProtoPromise v2.4.0](https://github.com/timcassell/ProtoPromise)
- [UniTask v2.3.3](https://github.com/Cysharp/UniTask)
- [UnityFx.Async v1.1.0](https://github.com/Arvtesh/UnityFx.Async)
- [RSG Promises v3.0.1](https://github.com/Real-Serious-Games/C-Sharp-Promise)

To run the benchmarks on your machine:
  1. Run `RunAllBenchmarks.bat` (or run a script individually for each runtime in the `Scripts` directory).
  2. See your results in the generated `Scripts/BenchmarkDotNet.Artifacts` directory.

Note:
Some functions are not actually supported in some libraries (like RSG does not support async/await), so they will show as `NA` or `?`.
Callbacks are measured the same for each benchmark as a baseline to compare against, and does not mean you can actually use async/await or ContinueWith with them.

Here are the results after running on my 2010 desktop:

```
BenchmarkDotNet=v0.13.2.20221225-develop, OS=Windows 7 SP1 (6.1.7601.0)
AMD Phenom(tm) II X6 1055T Processor, 1 CPU, 6 logical and 6 physical cores
Frequency=2746669 Hz, Resolution=364.0774 ns, Timer=TSC
```

### DotNet 6.0 Benchmarks

|         Type |       Method | Pending |       Mean | Ratio |   Gen0 | Allocated | Survived |
|------------- |------------- |-------- |-----------:|------:|-------:|----------:|---------:|
|   AsyncAwait |     Callback |   False |   293.1 ns |  1.00 | 0.2651 |     832 B |        - |
|   AsyncAwait | ProtoPromise |   False |   347.0 ns |  1.18 |      - |         - |        - |
|   AsyncAwait |   RsgPromise |   False |         NA |     ? |      ? |         ? |        ? |
|   AsyncAwait |         Task |   False |   311.4 ns |  1.06 | 0.0610 |     192 B |        - |
|   AsyncAwait |      UniTask |   False |   359.4 ns |  1.23 |      - |         - |        - |
|   AsyncAwait | UnityFxAsync |   False |   475.6 ns |  1.64 | 0.1144 |     360 B |     80 B |
|   AsyncAwait |    ValueTask |   False |   470.5 ns |  1.61 |      - |         - |        - |
|              |              |         |            |       |        |           |          |
|   AsyncAwait |     Callback |    True |   338.2 ns |  1.00 | 0.2651 |     832 B |        - |
|   AsyncAwait | ProtoPromise |    True | 2,057.5 ns |  6.08 |      - |         - |    696 B |
|   AsyncAwait |   RsgPromise |    True |         NA |     ? |      ? |         ? |        ? |
|   AsyncAwait |         Task |    True | 2,459.0 ns |  7.27 | 0.3891 |    1232 B |        - |
|   AsyncAwait |      UniTask |    True | 1,873.7 ns |  5.53 |      - |         - |  1,520 B |
|   AsyncAwait | UnityFxAsync |    True | 2,175.5 ns |  6.43 | 0.6218 |    1952 B |     40 B |
|   AsyncAwait |    ValueTask |    True | 2,660.9 ns |  7.86 | 0.3433 |    1080 B |     40 B |
|              |              |         |            |       |        |           |          |
| ContinueWith |     Callback |   False |   294.3 ns |  1.00 | 0.2651 |     832 B |        - |
| ContinueWith | ProtoPromise |   False |   498.1 ns |  1.69 |      - |         - |        - |
| ContinueWith |   RsgPromise |   False |   580.7 ns |  1.97 | 0.3290 |    1032 B |    648 B |
| ContinueWith |         Task |   False | 2,017.9 ns |  6.86 | 0.3891 |    1224 B |        - |
| ContinueWith |      UniTask |   False |   704.6 ns |  2.39 |      - |         - |        - |
| ContinueWith | UnityFxAsync |   False | 1,696.3 ns |  5.76 | 0.4139 |    1304 B |    656 B |
| ContinueWith |    ValueTask |   False |         NA |     ? |      ? |         ? |        ? |
|              |              |         |            |       |        |           |          |
| ContinueWith |     Callback |    True |   341.8 ns |  1.00 | 0.2651 |     832 B |        - |
| ContinueWith | ProtoPromise |    True | 2,177.5 ns |  6.37 |      - |         - |    384 B |
| ContinueWith |   RsgPromise |    True | 5,011.0 ns | 14.66 | 3.2196 |   10104 B |    728 B |
| ContinueWith |         Task |    True | 2,529.4 ns |  7.40 | 0.5112 |    1608 B |     16 B |
| ContinueWith |      UniTask |    True | 2,938.6 ns |  8.60 |      - |         - |  3,960 B |
| ContinueWith | UnityFxAsync |    True | 2,284.8 ns |  6.69 | 0.4959 |    1560 B |    552 B |
| ContinueWith |    ValueTask |    True |         NA |     ? |      ? |         ? |        ? |

### DotNet Framework 4.8 Benchmarks

|         Type |       Method | Pending |       Mean | Ratio |    Gen0 | Allocated | Survived |
|------------- |------------- |-------- |-----------:|------:|--------:|----------:|---------:|
|   AsyncAwait |     Callback |   False |   434.2 ns |  1.00 |  1.0610 |     834 B |        - |
|   AsyncAwait | ProtoPromise |   False | 1,442.4 ns |  3.32 |       - |         - |        - |
|   AsyncAwait |   RsgPromise |   False |         NA |     ? |       ? |         ? |        ? |
|   AsyncAwait |         Task |   False |   775.4 ns |  1.79 |  0.2651 |     209 B |        - |
|   AsyncAwait |      UniTask |   False |   560.5 ns |  1.29 |       - |         - |        - |
|   AsyncAwait | UnityFxAsync |   False |   721.0 ns |  1.66 |  0.4587 |     361 B |        - |
|   AsyncAwait |    ValueTask |   False |   954.4 ns |  2.20 |       - |         - |        - |
|              |              |         |            |       |         |           |          |
|   AsyncAwait |     Callback |    True |   499.3 ns |  1.00 |  1.0605 |     834 B |        - |
|   AsyncAwait | ProtoPromise |    True | 3,680.1 ns |  7.38 |       - |         - |    784 B |
|   AsyncAwait |   RsgPromise |    True |         NA |     ? |       ? |         ? |        ? |
|   AsyncAwait |         Task |    True | 7,127.8 ns | 14.28 |  2.6779 |    2110 B |        - |
|   AsyncAwait |      UniTask |    True | 2,586.7 ns |  5.18 |  0.0038 |      13 B |    760 B |
|   AsyncAwait | UnityFxAsync |    True | 4,177.3 ns |  8.37 |  2.4872 |    1958 B |        - |
|   AsyncAwait |    ValueTask |    True | 7,321.2 ns | 14.66 |  2.5177 |    1982 B |     40 B |
|              |              |         |            |       |         |           |          |
| ContinueWith |     Callback |   False |   436.8 ns |  1.00 |  1.0610 |     834 B |        - |
| ContinueWith | ProtoPromise |   False |   692.2 ns |  1.58 |       - |         - |        - |
| ContinueWith |   RsgPromise |   False |   798.6 ns |  1.78 |  1.3161 |    1035 B |        - |
| ContinueWith |         Task |   False | 5,927.4 ns | 13.58 |  1.6556 |    1308 B |        - |
| ContinueWith |      UniTask |   False |   968.6 ns |  2.22 |       - |         - |        - |
| ContinueWith | UnityFxAsync |   False | 5,732.3 ns | 13.13 |  1.6556 |    1308 B |        - |
| ContinueWith |    ValueTask |   False |         NA |     ? |       ? |         ? |        ? |
|              |              |         |            |       |         |           |          |
| ContinueWith |     Callback |    True |   501.9 ns |  1.00 |  1.0605 |     834 B |        - |
| ContinueWith | ProtoPromise |    True | 3,406.2 ns |  6.79 |       - |         - |    384 B |
| ContinueWith |   RsgPromise |    True | 7,046.1 ns | 14.04 | 13.2675 |   10439 B |        - |
| ContinueWith |         Task |    True | 7,570.7 ns | 15.09 |  2.1820 |    1725 B |     16 B |
| ContinueWith |      UniTask |    True | 4,012.8 ns |  8.00 |  0.0076 |      13 B |  1,320 B |
| ContinueWith | UnityFxAsync |    True | 6,747.0 ns | 13.44 |  1.9836 |    1565 B |     16 B |
| ContinueWith |    ValueTask |    True |         NA |     ? |       ? |         ? |        ? |

### Mono 6.10.0 Benchmarks

|         Type |       Method | Pending |        Mean | Ratio |   Gen0 | Allocated | Survived |
|------------- |------------- |-------- |------------:|------:|-------:|----------:|---------:|
|   AsyncAwait |     Callback |   False |    607.2 ns |  1.00 | 0.3595 |         ? |        ? |
|   AsyncAwait | ProtoPromise |   False |  1,475.9 ns |  2.43 |      - |         ? |        ? |
|   AsyncAwait |   RsgPromise |   False |          NA |     ? |      ? |         ? |        ? |
|   AsyncAwait |         Task |   False |    789.4 ns |  1.30 | 0.0496 |         ? |        ? |
|   AsyncAwait |      UniTask |   False |    476.2 ns |  0.78 |      - |         ? |        ? |
|   AsyncAwait | UnityFxAsync |   False |    912.1 ns |  1.50 | 0.0858 |         ? |        ? |
|   AsyncAwait |    ValueTask |   False |    960.9 ns |  1.58 |      - |         ? |        ? |
|              |              |         |             |       |        |           |          |
|   AsyncAwait |     Callback |    True |    768.4 ns |  1.00 | 0.3595 |         ? |        ? |
|   AsyncAwait | ProtoPromise |    True |  3,567.9 ns |  4.64 |      - |         ? |        ? |
|   AsyncAwait |   RsgPromise |    True |          NA |     ? |      ? |         ? |        ? |
|   AsyncAwait |         Task |    True |  5,081.9 ns |  6.61 | 0.6104 |         ? |        ? |
|   AsyncAwait |      UniTask |    True |  2,366.0 ns |  3.08 |      - |         ? |        ? |
|   AsyncAwait | UnityFxAsync |    True |  3,595.0 ns |  4.68 | 0.5760 |         ? |        ? |
|   AsyncAwait |    ValueTask |    True |  6,216.9 ns |  8.09 | 0.5798 |         ? |        ? |
|              |              |         |             |       |        |           |          |
| ContinueWith |     Callback |   False |    602.6 ns |  1.00 | 0.3595 |         ? |        ? |
| ContinueWith | ProtoPromise |   False |    744.6 ns |  1.24 |      - |         ? |        ? |
| ContinueWith |   RsgPromise |   False |    621.3 ns |  1.03 | 0.2489 |         ? |        ? |
| ContinueWith |         Task |   False |  4,985.6 ns |  8.27 | 0.6561 |         ? |        ? |
| ContinueWith |      UniTask |   False |  1,085.7 ns |  1.80 |      - |         ? |        ? |
| ContinueWith | UnityFxAsync |   False |  3,247.0 ns |  5.39 | 0.3128 |         ? |        ? |
| ContinueWith |    ValueTask |   False |          NA |     ? |      ? |         ? |        ? |
|              |              |         |             |       |        |           |          |
| ContinueWith |     Callback |    True |    753.6 ns |  1.00 | 0.3595 |         ? |        ? |
| ContinueWith | ProtoPromise |    True |  2,939.5 ns |  3.90 |      - |         ? |        ? |
| ContinueWith |   RsgPromise |    True | 10,511.3 ns | 13.95 | 3.3722 |         ? |        ? |
| ContinueWith |         Task |    True |  5,759.3 ns |  7.63 | 0.7553 |         ? |        ? |
| ContinueWith |      UniTask |    True |  5,019.2 ns |  6.66 |      - |         ? |        ? |
| ContinueWith | UnityFxAsync |    True |  4,273.2 ns |  5.67 | 0.3738 |         ? |        ? |
| ContinueWith |    ValueTask |    True |          NA |     ? |      ? |         ? |        ? |