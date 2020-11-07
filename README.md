This project is built to benchmark C# asynchronous libraries against each other.

To run the benchmarks on your machine:
  1. Init git submodules by running `git submodule update --init --recursive`.
  2. Build `ProtoPromise\ProtoPromise.sln` in `Release` mode.
  3. Build `RSGPromise\C-Sharp-Promise.sln` in `Release` mode.
  4. Build `UniTask\UniTask.NetCore.sln` in `Release` mode.
  5. Build `AsynchronousBenchmarks.sln"` in `Release` mode.
  6. Run `BenchmarkRunner.bat`.
    - Some benchmarks crash, causing windows to show a popup. You must close that popup for the rest of the benchmarks to continue.
  7. See your results in the generated `BenchmarkDotNet.Artifacts` directory.

Here are the results after running on my 2010 desktop:

### System Specs

```
BenchmarkDotNet=v0.12.1, OS=Windows 7 SP1 (6.1.7601.0)
AMD Phenom(tm) II X6 1055T Processor, 1 CPU, 6 logical and 6 physical cores
Frequency=2746679 Hz, Resolution=364.0760 ns, Timer=TSC
  [Host]     : .NET Framework 4.8 (4.8.4110.0), X64 RyuJIT
  Job-KZKWXR : .NET Framework 4.8 (4.8.4110.0), X64 RyuJIT
  Job-OUGRBP : .NET 5.0.29408.02 @BuiltBy: dlab14-DDVSOWINAGE075 @Branch: master @Commit: 4ce1c21ac0d4d1a3b7f7a548214966f69ac9f199, X64 AOT
  Job-IAWAXN : Mono 6.10.0 (Visual Studio), X64 
```

- `Proto_N` = ProtoPromise with no object pooling.
- `Proto_A` = ProtoPromise with all object pooling.
- ProtoPromise progress is disabled for these benchmarks.

Note: RSG Promises don't support all ContinueWith methods natively, so I added extensions for this benchmark. May not be representative of its performance if they get added in the future.
RSG Promises also do not support async/await, so those were omitted.

### ContinueWith Benchmarks

```
|     Method |        Job |    Runtime |     N |        Mean |       Error |      StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
|----------- |----------- |----------- |------ |------------:|------------:|------------:|------:|--------:|----------:|---------:|---------:|-----------:|
|    Proto_N | Job-KZKWXR |   .NET 4.8 |   100 |    820.0 μs |    16.35 μs |    33.39 μs |  1.29 |    0.03 |   72.2656 |  36.1328 |   4.8828 |    57090 B |
|    Proto_A | Job-KZKWXR |   .NET 4.8 |   100 |    210.0 μs |     2.15 μs |     1.90 μs |  0.34 |    0.00 |         - |        - |        - |          - |
| RSGPromise | Job-KZKWXR |   .NET 4.8 |   100 |  1,546.8 μs |     9.34 μs |     8.28 μs |  2.52 |    0.02 |  248.0469 |  99.6094 |        - |  1206041 B |
| SystemTask | Job-KZKWXR |   .NET 4.8 |   100 |    612.3 μs |     4.08 μs |     3.18 μs |  1.00 |    0.00 |   88.8672 |  11.7188 |        - |   103615 B |
|    UniTask | Job-KZKWXR |   .NET 4.8 |   100 |    223.6 μs |     1.10 μs |     1.03 μs |  0.37 |    0.00 |         - |        - |        - |      118 B |
|            |            |            |       |             |             |             |       |         |           |          |          |            |
|    Proto_N | Job-OUGRBP | CoreRt 3.1 |   100 |    387.8 μs |     5.23 μs |     4.89 μs |  1.36 |    0.02 |   18.0664 |   8.7891 |   0.9766 |    56912 B |
|    Proto_A | Job-OUGRBP | CoreRt 3.1 |   100 |    216.9 μs |     1.59 μs |     1.41 μs |  0.76 |    0.01 |         - |        - |        - |          - |
| RSGPromise | Job-OUGRBP | CoreRt 3.1 |   100 |  1,274.8 μs |     3.47 μs |     3.08 μs |  4.48 |    0.03 |  187.5000 |  78.1250 |        - |  1055184 B |
| SystemTask | Job-OUGRBP | CoreRt 3.1 |   100 |    284.0 μs |     1.94 μs |     1.91 μs |  1.00 |    0.00 |   30.2734 |        - |        - |    96096 B |
|    UniTask | Job-OUGRBP | CoreRt 3.1 |   100 |    180.4 μs |     1.18 μs |     0.98 μs |  0.63 |    0.00 |         - |        - |        - |      144 B |
|            |            |            |       |             |             |             |       |         |           |          |          |            |
|    Proto_N | Job-IAWAXN |       Mono |   100 |    561.3 μs |     2.22 μs |     1.85 μs |  0.96 |    0.01 |   10.7422 |   0.9766 |   0.9766 |          - |
|    Proto_A | Job-IAWAXN |       Mono |   100 |    179.5 μs |     3.24 μs |     3.03 μs |  0.31 |    0.01 |         - |        - |        - |          - |
| RSGPromise | Job-IAWAXN |       Mono |   100 |  3,831.0 μs |   100.25 μs |   294.01 μs |  6.40 |    0.57 |  414.0625 |  23.4375 |  23.4375 |          - |
| SystemTask | Job-IAWAXN |       Mono |   100 |    587.8 μs |     3.81 μs |     3.56 μs |  1.00 |    0.00 |   55.6641 |        - |        - |          - |
|    UniTask | Job-IAWAXN |       Mono |   100 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |
|            |            |            |       |             |             |             |       |         |           |          |          |            |
|    Proto_N | Job-KZKWXR |   .NET 4.8 | 10000 | 72,756.1 μs | 1,439.38 μs | 2,109.82 μs |  1.00 |    0.03 | 1000.0000 | 285.7143 | 142.8571 |  5697415 B |
|    Proto_A | Job-KZKWXR |   .NET 4.8 | 10000 | 20,833.1 μs |   278.07 μs |   217.10 μs |  0.29 |    0.00 |         - |        - |        - |          - |
| RSGPromise | Job-KZKWXR |   .NET 4.8 | 10000 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |
| SystemTask | Job-KZKWXR |   .NET 4.8 | 10000 | 72,908.7 μs |   330.46 μs |   309.11 μs |  1.00 |    0.00 | 1714.2857 | 714.2857 | 285.7143 | 10373592 B |
|    UniTask | Job-KZKWXR |   .NET 4.8 | 10000 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |
|            |            |            |       |             |             |             |       |         |           |          |          |            |
|    Proto_N | Job-OUGRBP | CoreRt 3.1 | 10000 | 37,998.7 μs |   669.80 μs |   559.32 μs |  0.92 |    0.01 |  937.5000 | 375.0000 | 125.0000 |  5680606 B |
|    Proto_A | Job-OUGRBP | CoreRt 3.1 | 10000 | 21,375.0 μs |   101.26 μs |    79.06 μs |  0.52 |    0.00 |         - |        - |        - |          - |
| RSGPromise | Job-OUGRBP | CoreRt 3.1 | 10000 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |
| SystemTask | Job-OUGRBP | CoreRt 3.1 | 10000 | 41,297.7 μs |   196.46 μs |   183.77 μs |  1.00 |    0.00 | 1615.3846 | 692.3077 | 230.7692 |  9602418 B |
|    UniTask | Job-OUGRBP | CoreRt 3.1 | 10000 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |
|            |            |            |       |             |             |             |       |         |           |          |          |            |
|    Proto_N | Job-IAWAXN |       Mono | 10000 | 70,001.1 μs |   284.76 μs |   266.37 μs |     ? |       ? | 1000.0000 | 125.0000 | 125.0000 |          - |
|    Proto_A | Job-IAWAXN |       Mono | 10000 | 19,197.5 μs |   382.81 μs |   628.97 μs |     ? |       ? |         - |        - |        - |          - |
| RSGPromise | Job-IAWAXN |       Mono | 10000 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |
| SystemTask | Job-IAWAXN |       Mono | 10000 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |
|    UniTask | Job-IAWAXN |       Mono | 10000 |          NA |          NA |          NA |     ? |       ? |         - |        - |        - |          - |

Benchmarks with issues:
  ContinuationBenchmarks.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=100]
  ContinuationBenchmarks.RSGPromise: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  ContinuationBenchmarks.UniTask: Job-KZKWXR(Runtime=.NET 4.8, RunStrategy=Throughput) [N=10000]
  ContinuationBenchmarks.RSGPromise: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  ContinuationBenchmarks.UniTask: Job-OUGRBP(Runtime=CoreRt 3.1, RunStrategy=Throughput) [N=10000]
  ContinuationBenchmarks.RSGPromise: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  ContinuationBenchmarks.SystemTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
  ContinuationBenchmarks.UniTask: Job-IAWAXN(Runtime=Mono, RunStrategy=Throughput) [N=10000]
```

### Async Benchmarks

```
|     Method |        Job |    Runtime |     N |         Mean |        Error |       StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|----------- |----------- |----------- |------ |-------------:|-------------:|-------------:|------:|--------:|----------:|---------:|---------:|----------:|
|    Proto_N | Job-KZKWXR |   .NET 4.8 |   100 |    540.96 μs |    10.760 μs |    13.215 μs |  2.30 |    0.06 |   85.4492 |  42.4805 |   6.8359 |   69112 B |
|    Proto_A | Job-KZKWXR |   .NET 4.8 |   100 |    107.35 μs |     2.083 μs |     3.117 μs |  0.46 |    0.01 |         - |        - |        - |      33 B |
| SystemTask | Job-KZKWXR |   .NET 4.8 |   100 |    234.47 μs |     4.226 μs |     4.150 μs |  1.00 |    0.00 |   77.1484 |   6.1035 |        - |   73050 B |
|    UniTask | Job-KZKWXR |   .NET 4.8 |   100 |    148.83 μs |     2.838 μs |     3.379 μs |  0.64 |    0.02 |    0.7324 |   0.2441 |        - |    3794 B |
|            |            |            |       |              |              |              |       |         |           |          |          |           |
|    Proto_N | Job-OUGRBP | CoreRt 3.1 |   100 |    296.79 μs |     5.769 μs |     9.150 μs |  3.04 |    0.13 |   20.0195 |   9.7656 |   0.9766 |   64104 B |
|    Proto_A | Job-OUGRBP | CoreRt 3.1 |   100 |    102.01 μs |     1.780 μs |     1.665 μs |  1.04 |    0.03 |         - |        - |        - |      32 B |
| SystemTask | Job-OUGRBP | CoreRt 3.1 |   100 |     98.35 μs |     1.958 μs |     2.176 μs |  1.00 |    0.00 |   16.2354 |        - |        - |   51232 B |
|    UniTask | Job-OUGRBP | CoreRt 3.1 |   100 |    115.19 μs |     2.210 μs |     2.171 μs |  1.17 |    0.03 |         - |        - |        - |      32 B |
|            |            |            |       |              |              |              |       |         |           |          |          |           |
|    Proto_N | Job-IAWAXN |       Mono |   100 |    654.07 μs |    12.485 μs |    11.068 μs |  3.43 |    0.08 |   18.5547 |   2.9297 |   2.9297 |         - |
|    Proto_A | Job-IAWAXN |       Mono |   100 |    112.13 μs |     1.929 μs |     1.611 μs |  0.59 |    0.01 |         - |        - |        - |         - |
| SystemTask | Job-IAWAXN |       Mono |   100 |    191.17 μs |     2.822 μs |     2.204 μs |  1.00 |    0.00 |   22.4609 |        - |        - |         - |
|    UniTask | Job-IAWAXN |       Mono |   100 |    140.52 μs |     2.767 μs |     5.526 μs |  0.75 |    0.03 |         - |        - |        - |         - |
|            |            |            |       |              |              |              |       |         |           |          |          |           |
|    Proto_N | Job-KZKWXR |   .NET 4.8 | 10000 | 59,573.54 μs | 1,189.565 μs | 2,803.944 μs |  1.90 |    0.10 | 1222.2222 | 555.5556 | 222.2222 | 6901440 B |
|    Proto_A | Job-KZKWXR |   .NET 4.8 | 10000 | 14,512.96 μs |   345.481 μs | 1,018.659 μs |  0.46 |    0.02 |         - |        - |        - |         - |
| SystemTask | Job-KZKWXR |   .NET 4.8 | 10000 | 31,613.12 μs |   617.977 μs |   845.894 μs |  1.00 |    0.00 | 1333.3333 | 600.0000 |  66.6667 | 7301962 B |
|    UniTask | Job-KZKWXR |   .NET 4.8 | 10000 | 21,531.07 μs |   429.996 μs |   858.749 μs |  0.69 |    0.04 |   62.5000 |  31.2500 |        - |  376369 B |
|            |            |            |       |              |              |              |       |         |           |          |          |           |
|    Proto_N | Job-OUGRBP | CoreRt 3.1 | 10000 | 32,600.60 μs |   424.652 μs |   354.603 μs |  2.24 |    0.03 | 1062.5000 | 375.0000 | 125.0000 | 6400106 B |
|    Proto_A | Job-OUGRBP | CoreRt 3.1 | 10000 | 11,484.49 μs |   213.696 μs |   199.892 μs |  0.79 |    0.01 |         - |        - |        - |         - |
| SystemTask | Job-OUGRBP | CoreRt 3.1 | 10000 | 14,568.78 μs |    83.196 μs |    73.751 μs |  1.00 |    0.00 |  843.7500 | 406.2500 |        - | 5120032 B |
|    UniTask | Job-OUGRBP | CoreRt 3.1 | 10000 | 15,957.28 μs |   151.241 μs |   141.471 μs |  1.09 |    0.01 |         - |        - |        - |         - |
|            |            |            |       |              |              |              |       |         |           |          |          |           |
|    Proto_N | Job-IAWAXN |       Mono | 10000 | 83,611.81 μs |   564.801 μs |   471.635 μs |  2.22 |    0.02 | 1833.3333 | 333.3333 | 333.3333 |         - |
|    Proto_A | Job-IAWAXN |       Mono | 10000 | 13,200.50 μs |    96.656 μs |    80.712 μs |  0.35 |    0.00 |         - |        - |        - |         - |
| SystemTask | Job-IAWAXN |       Mono | 10000 | 37,484.35 μs |   132.685 μs |   274.018 μs |  1.00 |    0.00 | 1928.5714 | 285.7143 | 285.7143 |         - |
|    UniTask | Job-IAWAXN |       Mono | 10000 | 21,911.18 μs |   256.970 μs |   214.581 μs |  0.58 |    0.01 |         - |        - |        - |         - |
```

### Await Benchmarks

```
|     Method |        Job |    Runtime |     N |       Mean |      Error |     StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------- |----------- |----------- |------ |-----------:|-----------:|-----------:|------:|--------:|-------:|------:|------:|----------:|
|    Proto_N | Job-KZKWXR |   .NET 4.8 |   100 |   4.736 μs |  0.0694 μs |  0.0649 μs |  0.96 |    0.02 | 0.0992 |     - |     - |      80 B |
|    Proto_A | Job-KZKWXR |   .NET 4.8 |   100 |   4.803 μs |  0.0822 μs |  0.0729 μs |  0.98 |    0.01 | 0.0992 |     - |     - |      80 B |
| SystemTask | Job-KZKWXR |   .NET 4.8 |   100 |   4.915 μs |  0.0258 μs |  0.0241 μs |  1.00 |    0.00 | 0.0992 |     - |     - |      80 B |
|    UniTask | Job-KZKWXR |   .NET 4.8 |   100 |   6.821 μs |  0.0469 μs |  0.0439 μs |  1.39 |    0.01 | 0.0992 |     - |     - |      80 B |
|            |            |            |       |            |            |            |       |         |        |       |       |           |
|    Proto_N | Job-OUGRBP | CoreRt 3.1 |   100 |   4.777 μs |  0.0257 μs |  0.0240 μs |  1.02 |    0.01 | 0.0229 |     - |     - |      72 B |
|    Proto_A | Job-OUGRBP | CoreRt 3.1 |   100 |   4.989 μs |  0.0584 μs |  0.0517 μs |  1.07 |    0.01 | 0.0229 |     - |     - |      72 B |
| SystemTask | Job-OUGRBP | CoreRt 3.1 |   100 |   4.672 μs |  0.0359 μs |  0.0336 μs |  1.00 |    0.00 | 0.0229 |     - |     - |      72 B |
|    UniTask | Job-OUGRBP | CoreRt 3.1 |   100 |   6.920 μs |  0.0587 μs |  0.0549 μs |  1.48 |    0.02 | 0.0229 |     - |     - |      72 B |
|            |            |            |       |            |            |            |       |         |        |       |       |           |
|    Proto_N | Job-IAWAXN |       Mono |   100 |   5.473 μs |  0.1046 μs |  0.1245 μs |  0.74 |    0.02 | 0.0153 |     - |     - |         - |
|    Proto_A | Job-IAWAXN |       Mono |   100 |   5.720 μs |  0.1107 μs |  0.1230 μs |  0.77 |    0.02 | 0.0153 |     - |     - |         - |
| SystemTask | Job-IAWAXN |       Mono |   100 |   7.429 μs |  0.0793 μs |  0.0703 μs |  1.00 |    0.00 | 0.0153 |     - |     - |         - |
|    UniTask | Job-IAWAXN |       Mono |   100 |         NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
|            |            |            |       |            |            |            |       |         |        |       |       |           |
|    Proto_N | Job-KZKWXR |   .NET 4.8 | 10000 | 363.010 μs |  5.1970 μs |  4.8612 μs |  0.77 |    0.01 |      - |     - |     - |      84 B |
|    Proto_A | Job-KZKWXR |   .NET 4.8 | 10000 | 383.515 μs |  4.8082 μs |  4.4976 μs |  0.81 |    0.01 |      - |     - |     - |      84 B |
| SystemTask | Job-KZKWXR |   .NET 4.8 | 10000 | 473.597 μs |  3.9061 μs |  3.6538 μs |  1.00 |    0.00 |      - |     - |     - |      84 B |
|    UniTask | Job-KZKWXR |   .NET 4.8 | 10000 | 619.340 μs | 12.3793 μs | 17.7541 μs |  1.31 |    0.04 |      - |     - |     - |      88 B |
|            |            |            |       |            |            |            |       |         |        |       |       |           |
|    Proto_N | Job-OUGRBP | CoreRt 3.1 | 10000 | 485.752 μs |  4.3931 μs |  3.8944 μs |  0.97 |    0.02 |      - |     - |     - |      72 B |
|    Proto_A | Job-OUGRBP | CoreRt 3.1 | 10000 | 488.623 μs |  6.8128 μs |  6.3727 μs |  0.97 |    0.02 |      - |     - |     - |      72 B |
| SystemTask | Job-OUGRBP | CoreRt 3.1 | 10000 | 488.360 μs |  9.7044 μs | 18.6970 μs |  1.00 |    0.00 |      - |     - |     - |      72 B |
|    UniTask | Job-OUGRBP | CoreRt 3.1 | 10000 | 694.972 μs | 13.0825 μs | 12.8487 μs |  1.38 |    0.05 |      - |     - |     - |      72 B |
|            |            |            |       |            |            |            |       |         |        |       |       |           |
|    Proto_N | Job-IAWAXN |       Mono | 10000 | 566.605 μs |  9.9361 μs | 14.2500 μs |  0.77 |    0.02 |      - |     - |     - |         - |
|    Proto_A | Job-IAWAXN |       Mono | 10000 | 556.216 μs | 10.8793 μs | 14.8917 μs |  0.75 |    0.02 |      - |     - |     - |         - |
| SystemTask | Job-IAWAXN |       Mono | 10000 | 739.777 μs |  7.0115 μs |  6.2155 μs |  1.00 |    0.00 |      - |     - |     - |         - |
|    UniTask | Job-IAWAXN |       Mono | 10000 |         NA |         NA |         NA |     ? |       ? |      - |     - |     - |         - |
```
