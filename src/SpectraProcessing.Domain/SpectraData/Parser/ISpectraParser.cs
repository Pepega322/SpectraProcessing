using SpectraProcessing.Domain.SpectraData.Formats;

namespace SpectraProcessing.Domain.SpectraData.Parser;

public interface ISpectraParser
{
    Spectra Parse(SpectraFormat format, string name, string[] contents);
}
