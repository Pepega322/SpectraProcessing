using System.Collections.Immutable;
using Domain.InputOutput;

namespace MathStatistics.SpectraProcessing;

public class SpectraProcessingDispersionStatistics : IWriteableData
{
	private readonly Dictionary<PeakBorders, DispersionStatistics<float>[]> statistics;

	public string? Name { get; set; } = string.Empty;
	public string Extension => "dispersion";

	internal SpectraProcessingDispersionStatistics(SpectraSetPeaks setPeaks)
	{
		statistics = setPeaks.ToDictionary(
			e => e.Borders,
			e => GetStatistics(e.Peaks)
		);

		return;

		DispersionStatistics<float>[] GetStatistics(IImmutableList<SpectraPeak> peaks)
		{
			var square = peaks.Select(p => p.Square).ToArray().GetDispersionStatistics("Square");
			var height = peaks.Select(p => p.Height).ToArray().GetDispersionStatistics("Height");
			return [square, height];
		}
	}

	public IEnumerable<string> ToContents()
	{
		const string statisticsInfoHeader = "xStart;xEnd;Parameter;ValuesCount;AverageValue;StandardDeviation;ConfidenceInterval;";

		yield return statisticsInfoHeader;
		foreach (var (borders, parametersStat) in statistics)
			foreach (var paramStat in parametersStat)
				yield return StatisticsFormat(borders, paramStat);

		yield break;

		string StatisticsFormat(PeakBorders borders, DispersionStatistics<float> s)
			=> $"{borders.XStart};{borders.XEnd};{s.ParameterName};{s.ValuesCount};{s.AverageValue: 0.000};{s.StandardDeviation: 0.000};{s.ConfidenceInterval: 0.000};";
	}
}