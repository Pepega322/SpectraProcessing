using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Enums;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.Models.Spectra;

public sealed class EspSpectraData(
    string name,
    SpectraPoints points,
    EspSpectraData.EspInfo info
) : SpectraData(name, points)
{
    public EspInfo Info { get; } = info;

    public override string Extension => "esp";

    protected override SpectraFormat Format => SpectraFormat.Esp;

    public override SpectraData ChangePoints(SpectraPoints newPoints) => new EspSpectraData(Name, newPoints, Info);

    public sealed record EspInfo
    {
        public string ExpCfg { get; init; }

        public string ProcCfg { get; init; }

        public EspInfo(string[] contents)
        {
            ExpCfg = contents[0];
            ProcCfg = contents[1];
        }
    }
}
