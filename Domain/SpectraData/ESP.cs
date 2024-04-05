using Domain.SpectraData.Support;

namespace Domain.SpectraData;
public abstract class Esp : Spectra {
    protected const int FirstPointIndex = 2;

    protected readonly ESPInfo Info;

    protected Esp(string name, SpectraPoints points, ESPInfo info)
        : base(name, points) {
        Info = info;
        Format = SpectraFormat.ESP;
    }
}