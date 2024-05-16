namespace Domain.SpectraData.Formats;

public sealed class AspSpectra : Spectra
{
	public readonly AspInfo Info;

	internal AspSpectra(string name, SpectraPoints points, AspInfo info)
		: base(name, points)
	{
		Info = info;
		Format = SpectraFormat.Asp;
	}

	public override Spectra Copy() => new AspSpectra(Name, Points, Info);
}