namespace SpectraProcessing.Domain.Models.MathModeling.Baseline;

public sealed record AirPLSSettings
{
    public required int IterationsCount { get; init; }

    public required double SmoothCoefficient { get; init; }

    public required double SmoothingTolerance { get; init; }
}
