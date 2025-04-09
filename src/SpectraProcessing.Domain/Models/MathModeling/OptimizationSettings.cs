namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed record OptimizationSettings
{
    public static readonly OptimizationSettings Default = new()
    {
        MaxIterationsCount = 10000,
        InitialShift = 0.01f,
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
            MaxConsecutiveShrinks = 50,
        },
    };

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
    }
}
