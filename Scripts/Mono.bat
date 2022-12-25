dotnet run -c Release -f net472 --project ..\AsynchronousBenchmarks\AsynchronousBenchmarks.csproj ^
--runtimes mono ^
--filter * ^
--memorySurvived ^
--hide Error StdDev Median RatioSD "Alloc Ratio" ^
--join