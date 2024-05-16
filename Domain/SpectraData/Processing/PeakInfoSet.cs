using System.Collections.Concurrent;
using Domain.InputOutput;

namespace Domain.SpectraData.Processing;

public class PeakInfoSet : IWriteableData
{
	private readonly ConcurrentBag<PeakInfo> peaks = [];

	public string Name => string.Empty;

	public void Add(PeakInfo info)
	{
		peaks.Add(info);
	}

	public IEnumerable<string> ToContents()
	{
		yield return "Name Square Height Start End";
		foreach (var info in peaks
			         .OrderBy(p => p.XStart)
			         .ThenBy(p => p.XEnd)
			         .ThenBy(p => p.Spectra.Name))
		{
			yield return info.ToString();
		}
		//TODO Add metrology calculations
	}
}