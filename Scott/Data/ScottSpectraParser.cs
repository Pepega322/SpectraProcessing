using Domain.DataSource;
using Domain.SpectraData;
using Domain.SpectraData.Support;

namespace Scott.Data;

public class ScottSpectraParser : ISpectraParser
{
	public Spectra Parse(SpectraFormat format, string name, string[] contents)
	{
		return format switch
		{
			SpectraFormat.ASP => ScottAsp.Parse(name, contents),
			SpectraFormat.ESP => ScottEsp.Parse(name, contents),
			_                 => throw new NotImplementedException(),
		};
	}
}