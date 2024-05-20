using Domain.Graphics;
using Domain.InputOutput;
using Domain.SpectraData.Formats;

namespace Domain.SpectraData;

public abstract class Spectra(string name, SpectraPoints points) : IWriteableData, IPlottableData
{
	public string Name { get; set; } = name;
	public string Extension => Format.ToString().ToLower();
	protected SpectraFormat Format { get; init; }
	public SpectraPoints Points { get; init; } = points;

	public abstract Spectra ChangePoints(SpectraPoints points);

	public virtual IEnumerable<string> ToContents() => Points.ToContents();

	public override bool Equals(object? obj)
	{
		return obj is Spectra spectra && Name == spectra.Name && Format == spectra.Format &&
			Points.Count == spectra.Points.Count;
	}

	public override int GetHashCode() => Points.GetHashCode();

	public override string ToString() => $"{Name} {Format} {Points.Count}";
}