using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace SpectraProcessing.MathStatistics.SpectraProcessing;

internal class SpectraSetPeaks : IEnumerable<(PeakBorders Borders, IImmutableList<SpectraPeak> Peaks)>
{
    private readonly ConcurrentDictionary<PeakBorders, ConcurrentBag<SpectraPeak>> storage = [];

    public void Add(SpectraPeak info)
    {
        if (!storage.ContainsKey(info.Borders))
            storage.TryAdd(info.Borders, []);
        storage[info.Borders].Add(info);
    }

    public IEnumerator<(PeakBorders Borders, IImmutableList<SpectraPeak> Peaks)> GetEnumerator()
    {
        foreach (var pair in storage)
            yield return (pair.Key, pair.Value.ToImmutableList());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
