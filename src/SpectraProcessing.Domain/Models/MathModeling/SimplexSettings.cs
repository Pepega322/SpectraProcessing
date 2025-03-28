namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed record SimplexSettings
{
    public required double InitialShift { get; init; }
}
