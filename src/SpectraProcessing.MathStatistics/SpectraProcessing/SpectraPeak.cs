using SpectraProcessing.Domain.SpectraData;

namespace SpectraProcessing.MathStatistics.SpectraProcessing;

public record SpectraPeak
{
    public required string SpectraName { get; init; }

    public required PeakBorders Borders { get; init; }

    public required float Square { get; init; }

    public required float Height { get; init; }
}
