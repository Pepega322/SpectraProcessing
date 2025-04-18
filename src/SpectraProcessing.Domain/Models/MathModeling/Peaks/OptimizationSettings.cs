namespace SpectraProcessing.Domain.Models.MathModeling.Peaks;

public sealed record OptimizationSettings
{
    public static readonly OptimizationSettings Default = new()
    {
        RepeatsCount = 10,
        MaxIterationsCount = 100,
        InitialShift = 0.1f,
        Coefficients = new IterationCoefficients
        {
            Reflection = 1,
            Expansion = 2,
            Contraction = 0.5f,
            Shrink = 0.5f,
        },
        Criteria = new CompletionСriteria
        {
            AbsoluteValue = null,
            MaxConsecutiveShrinks = 30,
            MinDeltaBetweenBetweenIterations = null,
            MaxIterationsWithLessThanDelta = null,
        },
    };

    public required int RepeatsCount { get; init; }

    public required int MaxIterationsCount { get; init; }

    public required float InitialShift { get; init; }

    public required IterationCoefficients Coefficients { get; init; }

    public required CompletionСriteria Criteria { get; init; }

    public sealed record IterationCoefficients
    {
        public required float Reflection { get; init; }
        public required float Expansion { get; init; }
        public required float Contraction { get; init; }
        public required float Shrink { get; init; }
    }

    public sealed record CompletionСriteria
    {
        public float? AbsoluteValue { get; init; }

        public int? MaxConsecutiveShrinks { get; init; }

        public float? MinDeltaBetweenBetweenIterations { get; init; }

        public int? MaxIterationsWithLessThanDelta { get; init; }
    }
}
