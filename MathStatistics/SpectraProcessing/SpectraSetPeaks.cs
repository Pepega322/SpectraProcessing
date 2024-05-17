using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace MathStatistics.SpectraProcessing;

public class SpectraSetPeaks
{
	private readonly ConcurrentDictionary<PeakBorders, ConcurrentBag<SpectraPeak>> storage = [];

	public IReadOnlyList<PeakBorders> Borders
		=> storage.Keys
			.ToImmutableArray();

	public IReadOnlyList<IReadOnlyList<SpectraPeak>> PeaksSets
		=> storage.Values
			.Select(l => l.ToImmutableList())
			.ToImmutableArray();

	public void Add(SpectraPeak info)
	{
		if (!storage.ContainsKey(info.Borders))
			storage.TryAdd(info.Borders, []);
		storage[info.Borders].Add(info);
	}
}