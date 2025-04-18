using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Enums;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.Models.Spectra;

public class SimpleSpectraData(string name, SpectraPoints points) : SpectraData(name, points)
{
    public override string Extension => "estimated";
    protected override SpectraFormat Format => SpectraFormat.Estimated;
}
