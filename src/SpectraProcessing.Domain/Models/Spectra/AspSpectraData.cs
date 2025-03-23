using System.Globalization;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Enums;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.Models.Spectra;

public sealed class AspSpectraData(
    string name,
    SpectraPoints points,
    AspSpectraData.AspInfo info
) : SpectraData(name, points)
{
    public AspInfo Info { get; } = info;

    public override string Extension => "asp";

    protected override SpectraFormat Format => SpectraFormat.Asp;

    public override SpectraData ChangePoints(SpectraPoints newPoints) => new AspSpectraData(Name, newPoints, Info);

    public sealed record AspInfo
    {
        public int PointCount { get; init; }

        public float StartWavenumber { get; init; }

        public float EndWavenumber { get; init; }

        public float Delta { get; init; }

        public AspInfo(string[] contents)
        {
            PointCount = int.Parse(contents[0]);
            StartWavenumber = float.Parse(contents[1], CultureInfo.InvariantCulture) / (float) (2 * Math.PI);
            EndWavenumber = float.Parse(contents[2], CultureInfo.InvariantCulture) / (float) (2 * Math.PI);
            Delta = float.Parse(contents[5], CultureInfo.InvariantCulture) / (float) (2 * Math.PI);
        }
    }
}
