using Domain.InputOutput;

namespace MathStatistics.SpectraProcessing;

public class SpectraProcessingDispersionStatistics : IWriteableData
{
	private readonly SpectraSetPeaks peaks;

	public string Name { get; set; } = string.Empty;

	internal SpectraProcessingDispersionStatistics(SpectraSetPeaks peaks)
	{
		this.peaks = peaks;
	}

	public IEnumerable<string> ToContents()
	{
		const string statisticsInfoHeader = "Parameter; xStart; xEnd; ValuesCount; AverageValue; StandardDeviation; ConfidenceInterval;";

		yield return statisticsInfoHeader;
		foreach (var borderSet in peaks.PeaksSets)
		{
			var borders = borderSet.First().Borders;
			var square = borderSet.Select(p => p.Square).ToArray().GetDispersionStatistics();
			yield return StatisticsFormat("Square", borders, square);
			var height = borderSet.Select(p => p.Height).ToArray().GetDispersionStatistics();
			yield return StatisticsFormat("Height", borders, height);
		}

		yield break;

		string StatisticsFormat(string parameter, PeakBorders borders, DispersionStatistics<float> s)
			=> $"{parameter}; {borders.XStart}; {borders.XEnd}; {s.ValuesCount}; {s.AverageValue: 0.000}; {s.StandardDeviation: 0.000}; {s.ConfidenceInterval: 0.000};";
	}
}