namespace SpectraProcessing.Domain.Models.MathModeling.Baseline;

public sealed record AirPLSBaselineCorrectionModel
{
    public static readonly AirPLSBaselineCorrectionModel Default = new()
    {
        MaxIterationsCount = 50,
        SmoothCoefficient = 1e6f,
        SmoothingTolerance = 1e-6f,
    };

    public required int MaxIterationsCount { get; init; }

    public required float SmoothCoefficient { get; init; }

    public required float SmoothingTolerance { get; init; }
}
