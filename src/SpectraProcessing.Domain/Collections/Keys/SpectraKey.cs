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
        return $"Name: {Name}; Id: {SpectraData.Id}";
    }

    public override bool Equals(object? obj) => obj is SpectraKey key && SpectraData.Id == key.SpectraData.Id;

    public override int GetHashCode() => SpectraData.Id.GetHashCode();
}
