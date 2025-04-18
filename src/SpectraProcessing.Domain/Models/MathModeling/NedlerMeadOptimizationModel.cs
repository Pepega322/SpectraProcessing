namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed record NedlerMeadOptimizationModel
{
    public required VectorN Start { get; init; }

    public required Dictionary<int, ValueConstraint> Constraints { get; init; }

    public required int BufferSize { get; init; }

    public required OptimizationSettings Settings { get; init; }
}
