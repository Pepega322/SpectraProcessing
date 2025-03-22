using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Enums;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Models.Spectra;

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
