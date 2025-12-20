# C# Asynchronous Libraries Benchmarks

This project is built to benchmark C# asynchronous libraries against each other.

These benchmarks are ran using a fork of [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) where I added the ability to measure survived memory.

This is a non-exhaustive benchmark of each library's features. It is only meant to compare the most common use scenario (awaiting an asynchronous result). Some libraries provide more/different features than others.

Asynchronous libraries benchmarked:
- Built-in callbacks (`System.Action`), measured as the baseline
- [System.Threading.Tasks (TPL)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl), `Task` and `ValueTask`
- [ProtoPromise v3.4.0](https://github.com/timcassell/ProtoPromise)
- [UniTask v2.5.10](https://github.com/Cysharp/UniTask)
- [UnityFx.Async v1.1.0](https://github.com/Arvtesh/UnityFx.Async)
- [RSG Promises v3.0.1](https://github.com/Real-Serious-Games/C-Sharp-Promise)

To run the benchmarks on your machine:
  1. Run `RunAllBenchmarks.bat` (or run a script individually for each runtime in the `Scripts` directory).
  2. See your results in the generated `Scripts/BenchmarkDotNet.Artifacts` directory.

Note:
Some functions are not actually supported in some libraries (like RSG does not support async/await), so they will show as `NA` or `?`.
Callbacks are measured the same for each benchmark as a baseline to compare against, and does not mean you can actually use async/await or ContinueWith with them.

`ContinueWith` is either using the `.ContinueWith` API or `.Then` API, depending on each library.

## .Net 10.0 Benchmarks

```
BenchmarkDotNet v0.16.0-nightly.20251220.380, Windows 10 (10.0.19045.6466/22H2/2022Update)
AMD Ryzen 7 9800X3D 4.70GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
```

| Type         | Method       | Pending | Mean      | Ratio | Allocated | Pooled |
|------------- |------------- |-------- |----------:|------:|----------:|-------:|
| AsyncAwait   | Callback     | False   |  41.84 ns |  1.00 |     832 B |      - |
| AsyncAwait   | ProtoPromise | False   |  16.38 ns |  0.39 |         - |      - |
| AsyncAwait   | RsgPromise   | False   |        NA |     ? |        NA |     NA |
| AsyncAwait   | Task         | False   |  31.88 ns |  0.76 |     192 B |      - |
| AsyncAwait   | UniTask      | False   |  33.59 ns |  0.80 |         - |      - |
| AsyncAwait   | UnityFxAsync | False   |  45.51 ns |  1.09 |     360 B |      - |
| AsyncAwait   | ValueTask    | False   |  33.03 ns |  0.79 |         - |      - |
|              |              |         |           |       |           |        |
| AsyncAwait   | Callback     | True    |  51.67 ns |  1.00 |     832 B |      - |
| AsyncAwait   | ProtoPromise | True    | 329.77 ns |  6.38 |         - |  600 B |
| AsyncAwait   | RsgPromise   | True    |        NA |     ? |        NA |     NA |
| AsyncAwait   | Task         | True    | 403.93 ns |  7.82 |    1120 B |  376 B |
| AsyncAwait   | UniTask      | True    | 377.76 ns |  7.31 |         - |  720 B |
| AsyncAwait   | UnityFxAsync | True    | 367.65 ns |  7.12 |    1952 B |      - |
| AsyncAwait   | ValueTask    | True    | 492.04 ns |  9.52 |    1352 B |  504 B |
|              |              |         |           |       |           |        |
| ContinueWith | Callback     | False   |  42.59 ns |  1.00 |     832 B |      - |
| ContinueWith | ProtoPromise | False   |  78.01 ns |  1.83 |         - |      - |
| ContinueWith | RsgPromise   | False   |  74.85 ns |  1.76 |    1032 B |      - |
| ContinueWith | Task         | False   | 298.10 ns |  7.00 |    1224 B |      - |
| ContinueWith | UniTask      | False   |  82.09 ns |  1.93 |         - |      - |
| ContinueWith | UnityFxAsync | False   | 239.16 ns |  5.62 |    1304 B |      - |
| ContinueWith | ValueTask    | False   |        NA |     ? |        NA |     NA |
|              |              |         |           |       |           |        |
| ContinueWith | Callback     | True    |  51.70 ns |  1.00 |     832 B |      - |
| ContinueWith | ProtoPromise | True    | 316.67 ns |  6.13 |         - |  312 B |
| ContinueWith | RsgPromise   | True    | 770.52 ns | 14.91 |   10104 B |      - |
| ContinueWith | Task         | True    | 525.95 ns | 10.17 |    1864 B |      - |
| ContinueWith | UniTask      | True    | 528.12 ns | 10.22 |         - | 1272 B |
| ContinueWith | UnityFxAsync | True    | 365.29 ns |  7.07 |    1560 B |      - |
| ContinueWith | ValueTask    | True    |        NA |     ? |        NA |     NA |
