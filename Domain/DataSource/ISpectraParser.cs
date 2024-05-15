using Domain.SpectraData;
using Domain.SpectraData.Support;

namespace Domain.DataSource;

public interface ISpectraParser
{
	Spectra Parse(SpectraFormat format, string name, string[] contents);
}