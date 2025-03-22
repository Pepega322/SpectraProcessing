using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Enums;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Models.Spectra;

public class EstimatedSpectraData(string name, SpectraPoints points) : SpectraData(name, points)
{
    public override string Extension => "estimated";
    protected override SpectraFormat Format => SpectraFormat.Estimated;

    public override SpectraData ChangePoints(SpectraPoints newPoints)
    {
        return new EstimatedSpectraData(Name, newPoints);
    }
}
