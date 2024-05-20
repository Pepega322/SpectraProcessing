using Domain.InputOutput;

namespace MathStatistics.SpectraProcessing;

public class SpectrasProcessingResult : IWriteableData
{
	private readonly SpectraSetPeaks setPeaks = new();

	public string Name { get; set; } = string.Empty;
	public string Extension => "peaks";

	public void Add(SpectraPeak peak)
	{
		setPeaks.Add(peak);
	}

	public SpectraProcessingDispersionStatistics GetDispersionStatistics() => new SpectraProcessingDispersionStatistics(setPeaks);

	public IEnumerable<string> ToContents()
	{
		const string peakInfoHeader = "XStart;XEnd;Spectra;Square;Height;";

		yield return peakInfoHeader;
		foreach (var (_, peaks) in setPeaks)
		{
			foreach (var peak in peaks.OrderByDescending(p => p.SpectraName))
				yield return PeakFormat(peak);
		}

		yield break;

		string PeakFormat(SpectraPeak p) => $"{p.Borders.XStart:0.000};{p.Borders.XEnd:0.000};{p.SpectraName};{p.Square:0.000};{p.Height:0.000};";
	}
}