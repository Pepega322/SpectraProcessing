using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Models.Collections.Keys;

public sealed class SpectraKey(SpectraData spectraData) : INamedKey
{
    public string Name { get; } = spectraData.Name;

    public readonly SpectraData SpectraData = spectraData;

    public override string ToString()
    {
        return SpectraData.Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is SpectraKey key && string.Equals(Name, key.Name);
    }

    public override int GetHashCode() => Name.GetHashCode();
}
