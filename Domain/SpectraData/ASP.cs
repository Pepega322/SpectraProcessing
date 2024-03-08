using Domain.SpectraData.Support;

namespace Domain.SpectraData;
public abstract class ASP : Spectra {
    protected const int FirstPointIndex = 7;

    public ASPInfo Info;

    protected ASP(string name, SpectraPoints points, ASPInfo info)
        : base(name, points) {
        Info = info;
        Format = SpectraFormat.ASP;
    }
}