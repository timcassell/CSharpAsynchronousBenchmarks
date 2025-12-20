dotnet run -c Release -f net10.0 --project ..\AsynchronousBenchmarks\AsynchronousBenchmarks.csproj ^
--filter * ^
--hide Error StdDev Median RatioSD "Alloc Ratio" ^
--join