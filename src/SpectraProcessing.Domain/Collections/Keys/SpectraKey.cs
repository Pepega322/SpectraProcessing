using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.Collections.Keys;

public sealed class SpectraKey : INamedKey
{
    public string Name { get; }

    public readonly SpectraData SpectraData;

    public SpectraKey(SpectraData spectraData)
    {
        Name = spectraData.Name;
        SpectraData = spectraData;
    }

    public override string ToString()
    {
        return $"Name: {Name}; Id: {GetHashCode()}";
    }

    public override bool Equals(object? obj)
    {
        return obj is SpectraKey key && string.Equals(Name, key.Name);
    }

    public override int GetHashCode() => SpectraData.GetHashCode();
}
