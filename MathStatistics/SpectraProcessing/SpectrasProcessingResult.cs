using System.Collections.Concurrent;
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

	public IEnumerable<string> ToContents()
	{
		yield return "XStart XEnd Spectra Square Height";
		
		//TODO Add metrology calculations
	}
}