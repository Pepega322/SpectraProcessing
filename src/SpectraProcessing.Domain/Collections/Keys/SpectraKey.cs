using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.Collections.Keys;

public sealed class SpectraKey : INamedKey
{
    private static long _counter;

    private readonly long id;

    public string Name { get; }

    public readonly SpectraData SpectraData;

    public SpectraKey(SpectraData spectraData)
    {
        id = Interlocked.Increment(ref _counter);
        Name = spectraData.Name;
        SpectraData = spectraData;
    }

    public override string ToString()
    {
        return $"Name: {Name}; Id: {id}";
    }

    public override bool Equals(object? obj) => obj is SpectraKey key && id == key.id;

    public override int GetHashCode() => id.GetHashCode();
}
