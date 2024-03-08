using Domain.DataSource;
using Domain.SpectraData;
using Domain.SpectraData.Support;

namespace Scott.Data;
public class ScottSpectraParser : ISpectraParser {
    public Spectra Parse(SpectraFormat format, string name, string[] contents) {
        return format switch {
            SpectraFormat.ASP => ScottASP.Parse(name, contents),
            SpectraFormat.ESP => ScottESP.Parse(name, contents),
            _ => throw new NotImplementedException(),
        };
    }
}
