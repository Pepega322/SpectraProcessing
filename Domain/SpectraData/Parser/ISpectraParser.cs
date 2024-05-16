using Domain.SpectraData.Formats;

namespace Domain.SpectraData.Parser;

public interface ISpectraParser
{
	Spectra Parse(SpectraFormat format, string name, string[] contents);
}