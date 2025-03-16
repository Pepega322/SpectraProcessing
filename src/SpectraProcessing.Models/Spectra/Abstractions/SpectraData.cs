using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Enums;

namespace SpectraProcessing.Models.Spectra.Abstractions;

public abstract class SpectraData(string name, SpectraPoints points) : IWriteableData, IPlottableData
{
    public string Name { get; set; } = name;

    public abstract string Extension { get; }

    public abstract SpectraFormat Format { get; }

    public SpectraPoints Points { get; init; } = points;

    public abstract SpectraData ChangePoints(SpectraPoints points);

    public virtual IEnumerable<string> ToContents() => Points.ToContents();

    public override bool Equals(object? obj)
    {
        return obj is SpectraData spectra && Name == spectra.Name
            && Format == spectra.Format &&
            Points.Count == spectra.Points.Count;
    }

    public override int GetHashCode() => Points.GetHashCode();

    public override string ToString() => $"{Name} {Format} {Points.Count}";
}
