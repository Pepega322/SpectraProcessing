namespace SpectraProcessing.Domain.Models.MathModeling.Peaks;

public sealed record NedlerMeadSettings
{
    public int RepeatsCount { get; init; } = 3;

    public int MaxIterationsCount { get; init; } = 500;

    public float? MinDeltaPercentageBetweenIterations { get; init; } = 1e-6f;

    public int? MaxIterationsWithDeltaLessThanMin { get; init; } = 50;

    public int? MaxConsecutiveShrinks { get; init; } = 50;

    public float? MinAbsoluteValue { get; init; }
}
