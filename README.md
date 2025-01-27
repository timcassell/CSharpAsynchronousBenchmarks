# C# Asynchronous Libraries Benchmarks

This project is built to benchmark C# asynchronous libraries against each other.

These benchmarks are ran using a fork of [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) where I added the ability to measure survived memory.

This is a non-exhaustive benchmark of each library's features. It is only meant to compare the most common use scenario (awaiting an asynchronous result). Some libraries provide more/different features than others.

Asynchronous libraries benchmarked:
- Built-in callbacks (`System.Action`), measured as the baseline
- [System.Threading.Tasks (TPL)](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl), `Task` and `ValueTask`
- [ProtoPromise v3.3.0](https://github.com/timcassell/ProtoPromise)
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

## .Net 8.0 Benchmarks

```
BenchmarkDotNet v0.14.1-develop (2025-01-27), Windows 10 (10.0.19045.5371/22H2/2022Update)
AMD Ryzen 7 9800X3D 4.70GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```

| Type         | Method       | Pending | Mean      | Ratio | Allocated | Survived |
|------------- |------------- |-------- |----------:|------:|----------:|---------:|
| AsyncAwait   | Callback     | False   |  43.99 ns |  1.00 |     832 B |        - |
| AsyncAwait   | ProtoPromise | False   |  37.56 ns |  0.85 |         - |        - |
| AsyncAwait   | RsgPromise   | False   |        NA |     ? |        NA |       NA |
| AsyncAwait   | Task         | False   |  49.97 ns |  1.14 |     192 B |        - |
| AsyncAwait   | UniTask      | False   |  81.06 ns |  1.84 |         - |        - |
| AsyncAwait   | UnityFxAsync | False   |  62.05 ns |  1.41 |     360 B |        - |
| AsyncAwait   | ValueTask    | False   |  70.66 ns |  1.61 |         - |        - |
|              |              |         |           |       |           |          |
| AsyncAwait   | Callback     | True    |  52.06 ns |  1.00 |     832 B |        - |
| AsyncAwait   | ProtoPromise | True    | 401.37 ns |  7.71 |         - |    624 B |
| AsyncAwait   | RsgPromise   | True    |        NA |     ? |        NA |       NA |
| AsyncAwait   | Task         | True    | 455.15 ns |  8.74 |    1120 B |        - |
| AsyncAwait   | UniTask      | True    | 478.92 ns |  9.20 |         - |    744 B |
| AsyncAwait   | UnityFxAsync | True    | 433.54 ns |  8.33 |    1952 B |        - |
| AsyncAwait   | ValueTask    | True    | 498.24 ns |  9.57 |     968 B |     40 B |
|              |              |         |           |       |           |          |
| ContinueWith | Callback     | False   |  44.91 ns |  1.00 |     832 B |        - |
| ContinueWith | ProtoPromise | False   | 107.35 ns |  2.39 |         - |        - |
| ContinueWith | RsgPromise   | False   |  89.26 ns |  1.99 |    1032 B |        - |
| ContinueWith | Task         | False   | 384.44 ns |  8.56 |    1224 B |        - |
| ContinueWith | UniTask      | False   | 158.15 ns |  3.52 |         - |        - |
| ContinueWith | UnityFxAsync | False   | 319.22 ns |  7.11 |    1304 B |        - |
| ContinueWith | ValueTask    | False   |        NA |     ? |        NA |       NA |
|              |              |         |           |       |           |          |
| ContinueWith | Callback     | True    |  53.44 ns |  1.00 |     832 B |        - |
| ContinueWith | ProtoPromise | True    | 375.57 ns |  7.03 |         - |    336 B |
| ContinueWith | RsgPromise   | True    | 889.53 ns | 16.64 |   10104 B |        - |
| ContinueWith | Task         | True    | 646.31 ns | 12.09 |    1864 B |        - |
| ContinueWith | UniTask      | True    | 720.61 ns | 13.48 |         - |  1,296 B |
| ContinueWith | UnityFxAsync | True    | 431.36 ns |  8.07 |    1560 B |     16 B |
| ContinueWith | ValueTask    | True    |        NA |     ? |        NA |       NA |