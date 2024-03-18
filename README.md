# C# Asynchronous Libraries Benchmarks

This project is built to benchmark C# asynchronous libraries against each other.

These benchmarks are ran using a fork of [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) where I added the ability to measure survived memory.

This is a non-exhaustive benchmark of each library's features. It is only meant to compare the most common use scenario (awaiting an asynchronous result). Some libraries provide more/different features than others.

Asynchronous libraries benchmarked:
- Built-in callbacks (`System.Action`), measured as the baseline
- [System.Threading.Tasks (TPL)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl), `Task` and `ValueTask`
- [ProtoPromise v3.0.0](https://github.com/timcassell/ProtoPromise)
- [UniTask v2.5.3](https://github.com/Cysharp/UniTask)
- [UnityFx.Async v1.1.0](https://github.com/Arvtesh/UnityFx.Async)
- [RSG Promises v3.0.1](https://github.com/Real-Serious-Games/C-Sharp-Promise)

To run the benchmarks on your machine:
  1. Run `RunAllBenchmarks.bat` (or run a script individually for each runtime in the `Scripts` directory).
  2. See your results in the generated `Scripts/BenchmarkDotNet.Artifacts` directory.

Note:
Some functions are not actually supported in some libraries (like RSG does not support async/await), so they will show as `NA` or `?`.
Callbacks are measured the same for each benchmark as a baseline to compare against, and does not mean you can actually use async/await or ContinueWith with them.

`ContinueWith` is either using the `.ContinueWith` API or `.Then` API, depending on each library.

Here are the results after running on my 2010 desktop:

## .Net 8.0 Benchmarks

```
BenchmarkDotNet v0.13.13-develop (2024-03-18), Windows 10 (10.0.19045.4170/22H2/2022Update)
AMD Phenom(tm) II X6 1055T Processor, 1 CPU, 6 logical and 6 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT SSE3
  DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT SSE3
```

| Type         | Method       | Pending | Mean       | Ratio | Gen0   | Allocated | Survived |
|------------- |------------- |-------- |-----------:|------:|-------:|----------:|---------:|
| AsyncAwait   | Callback     | False   |   270.3 ns |  1.00 | 0.2651 |     832 B |        - |
| AsyncAwait   | ProtoPromise | False   |   172.6 ns |  0.64 |      - |         - |        - |
| AsyncAwait   | RsgPromise   | False   |         NA |     ? |     NA |        NA |       NA |
| AsyncAwait   | Task         | False   |   260.9 ns |  0.97 | 0.0610 |     192 B |        - |
| AsyncAwait   | UniTask      | False   |   306.8 ns |  1.14 |      - |         - |        - |
| AsyncAwait   | UnityFxAsync | False   |   368.6 ns |  1.36 | 0.1144 |     360 B |        - |
| AsyncAwait   | ValueTask    | False   |   337.4 ns |  1.25 |      - |         - |        - |
|              |              |         |            |       |        |           |          |
| AsyncAwait   | Callback     | True    |   331.7 ns |  1.00 | 0.2651 |     832 B |        - |
| AsyncAwait   | ProtoPromise | True    | 1,402.3 ns |  4.23 |      - |         - |    648 B |
| AsyncAwait   | RsgPromise   | True    |         NA |     ? |     NA |        NA |       NA |
| AsyncAwait   | Task         | True    | 2,110.8 ns |  6.36 | 0.3548 |    1120 B |        - |
| AsyncAwait   | UniTask      | True    | 1,817.7 ns |  5.48 |      - |         - |    744 B |
| AsyncAwait   | UnityFxAsync | True    | 1,998.2 ns |  6.02 | 0.6218 |    1952 B |        - |
| AsyncAwait   | ValueTask    | True    | 2,486.9 ns |  7.50 | 0.3052 |     968 B |     40 B |
|              |              |         |            |       |        |           |          |
| ContinueWith | Callback     | False   |   269.8 ns |  1.00 | 0.2651 |     832 B |        - |
| ContinueWith | ProtoPromise | False   |   386.9 ns |  1.43 |      - |         - |        - |
| ContinueWith | RsgPromise   | False   |   508.4 ns |  1.88 | 0.3290 |    1032 B |        - |
| ContinueWith | Task         | False   | 1,738.1 ns |  6.44 | 0.3891 |    1224 B |        - |
| ContinueWith | UniTask      | False   |   561.1 ns |  2.08 |      - |         - |        - |
| ContinueWith | UnityFxAsync | False   | 1,478.4 ns |  5.48 | 0.4139 |    1304 B |  6,144 B |
| ContinueWith | ValueTask    | False   |         NA |     ? |     NA |        NA |       NA |
|              |              |         |            |       |        |           |          |
| ContinueWith | Callback     | True    |   327.4 ns |  1.00 | 0.2651 |     832 B |        - |
| ContinueWith | ProtoPromise | True    | 1,352.7 ns |  4.13 |      - |         - |    336 B |
| ContinueWith | RsgPromise   | True    | 4,889.1 ns | 14.94 | 3.2196 |   10104 B |        - |
| ContinueWith | Task         | True    | 2,321.2 ns |  7.09 | 0.5112 |    1608 B |     16 B |
| ContinueWith | UniTask      | True    | 2,857.5 ns |  8.73 |      - |         - |  1,296 B |
| ContinueWith | UnityFxAsync | True    | 1,946.5 ns |  5.95 | 0.4959 |    1560 B |     16 B |
| ContinueWith | ValueTask    | True    |         NA |     ? |     NA |        NA |       NA |
