using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.Enums;

namespace SpectraProcessing.Domain.Models.Spectra.Abstractions;

public abstract class SpectraData(string name, SpectraPoints points) : IWriteableData, IPlottableData
{
    private static long _counter;

    private readonly long id = Interlocked.Increment(ref _counter);

    public string Name { get; set; } = name;

    public SpectraPoints Points { get; protected set; } = points;

    public abstract string Extension { get; }

    protected abstract SpectraFormat Format { get; }

    public virtual IEnumerable<string> ToContents() => Points.ToContents();

    public override bool Equals(object? obj) => obj is SpectraData s && s.id == id;

    public override int GetHashCode() => id.GetHashCode();

    public override string ToString() => $"{Name} {Format} {Points.Count}";
}
