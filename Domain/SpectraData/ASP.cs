using Domain.SpectraData.Support;

namespace Domain.SpectraData;

public abstract class Asp : Spectra {
	protected const int FirstPointIndex = 7;

	public readonly ASPInfo Info;

	protected Asp(string name, SpectraPoints points, ASPInfo info)
		: base(name, points) {
		Info = info;
		Format = SpectraFormat.ASP;
	}
}