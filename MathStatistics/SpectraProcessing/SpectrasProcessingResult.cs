using Domain.InputOutput;

namespace MathStatistics.SpectraProcessing;

public class SpectrasProcessingResult : IWriteableData
{
	private readonly SpectraSetPeaks peaks = new();

	public string Name { get; set; } = string.Empty;

	public void Add(SpectraPeak peak)
	{
		peaks.Add(peak);
	}

	public SpectraProcessingDispersionStatistics GetMetrology() => new SpectraProcessingDispersionStatistics(peaks);

	public IEnumerable<string> ToContents()
	{
		const string peakInfoHeader = "XStart; XEnd; Spectra; Square; Height;";

		yield return peakInfoHeader;
		foreach (var borderSet in peaks.PeaksSets)
		{
			foreach (var peak in borderSet.OrderByDescending(p => p.SpectraName))
				yield return PeakFormat(peak);
		}

		yield break;

		string PeakFormat(SpectraPeak p) => $"{p.Borders.XStart: 0.000}; {p.Borders.XEnd: 0.000}; {p.SpectraName}; {p.Square: 0.000}; {p.Height: 0.000};";
	}
}