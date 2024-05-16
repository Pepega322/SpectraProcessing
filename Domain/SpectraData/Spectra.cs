using Domain.Graphics;
using Domain.InputOutput;
using Domain.SpectraData.Formats;

namespace Domain.SpectraData;

public abstract class Spectra(string name, SpectraPoints points) : IWriteableData, IPlottableData
{
	public string Name { get; set; } = name;
	protected SpectraFormat Format { get; init; }
	public SpectraPoints Points { get; private set; } = points;

	public abstract Spectra Copy();

	public Spectra ChangePoints(SpectraPoints points)
	{
		var changed = Copy();
		changed.Points = points;
		return changed;
	}

	public virtual IEnumerable<string> ToContents() => Points.ToContents();

	public override bool Equals(object? obj)
	{
		return obj is Spectra spectra && Name == spectra.Name && Format == spectra.Format &&
			Points.Count == spectra.Points.Count;
	}

	public override int GetHashCode() => HashCode.Combine(Name, Format, Points.Count);

	public override string ToString() => $"{Name} {Format} {Points.Count}";
}