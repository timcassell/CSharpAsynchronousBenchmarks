using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Perfolizer.Metrology;
using System;
using System.Collections.Generic;

namespace AsynchronousBenchmarks;

public interface IPooledMemoryGetter
{
    long GetPooledMemory();
}

public sealed class PooledMemoryDiagnoser : IInProcessDiagnoser
{
    private readonly Dictionary<BenchmarkCase, long> _results = [];

    public IEnumerable<string> Ids => [nameof(PooledMemoryDiagnoser)];

    public IEnumerable<IExporter> Exporters => [];

    public IEnumerable<IAnalyser> Analysers => [];

    public void DisplayResults(ILogger logger) { }

    public RunMode GetRunMode(BenchmarkCase benchmarkCase) => RunMode.NoOverhead;

    public void Handle(HostSignal signal, DiagnoserActionParameters parameters) { }

    public IEnumerable<Metric> ProcessResults(DiagnoserResults results)
    {
        if (_results.TryGetValue(results.BenchmarkCase, out var result))
        {
            yield return new(PooledMemoryMetricDescriptor.Instance, result);
        }
    }

    public IEnumerable<ValidationError> Validate(ValidationParameters validationParameters) => [];

    public void DeserializeResults(BenchmarkCase benchmarkCase, string serializedResults)
        => _results.Add(benchmarkCase, long.Parse(serializedResults));

    public InProcessDiagnoserHandlerData GetHandlerData(BenchmarkCase benchmarkCase)
        => new(typeof(PooledMemoryDiagnoserInProcessHandler), null);

    internal class PooledMemoryMetricDescriptor : IMetricDescriptor
    {
        public readonly static PooledMemoryMetricDescriptor Instance = new();

        public string Id => "PooledMemory";
        public string DisplayName => "Pooled";
        public string Legend => "The memory that survived in a pool after the operation";
        public string NumberFormat => "0.##";
        public UnitType UnitType => UnitType.Size;
        public string Unit => SizeUnit.B.Abbreviation;
        public bool TheGreaterTheBetter => false;
        public int PriorityInCategory => GC.MaxGeneration + 2;
        public bool GetIsAvailable(Metric metric) => true;
    }
}

public sealed class PooledMemoryDiagnoserInProcessHandler : IInProcessDiagnoserHandler
{
    private long _result;

    public void Initialize(string? serializedConfig) { }

    public void Handle(BenchmarkSignal signal, InProcessDiagnoserActionArgs args)
    {
        if (signal == BenchmarkSignal.AfterEngine
            && args.BenchmarkInstance is IPooledMemoryGetter PooledMemoryGetter)
        {
            _result = PooledMemoryGetter.GetPooledMemory();
        }
    }

    public string SerializeResults()
        => _result.ToString();
}