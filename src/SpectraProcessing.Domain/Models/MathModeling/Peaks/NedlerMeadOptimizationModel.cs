using SpectraProcessing.Domain.Models.MathModeling.Common;

namespace SpectraProcessing.Domain.Models.MathModeling.Peaks;

public sealed record NedlerMeadOptimizationModel
{
    public required VectorN Start { get; init; }

    public required Dictionary<int, ValueConstraint> Constraints { get; init; }

    public required int BufferSize { get; init; }

    public required OptimizationSettings Settings { get; init; }
}
