namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed record OptimizationSettings
{
    public static readonly OptimizationSettings Default = new()
    {
        MaxIterationsCount = 10000,
        InitialShift = 0.01,
        Coefficients = new IterationCoefficients
        {
            Reflection = 1,
            Expansion = 2,
            Contraction = 0.5,
            Shrink = 0.5,
        },
        Criteria = new CompletionСriteria
        {
            AbsoluteValue = null,
            MaxConsecutiveShrinks = 50,
        },
    };

    public required int MaxIterationsCount { get; init; }

    public required double InitialShift { get; init; }

    public required IterationCoefficients Coefficients { get; init; }

    public required CompletionСriteria Criteria { get; init; }


    public sealed record IterationCoefficients
    {
        public required double Reflection { get; init; }
        public required double Expansion { get; init; }
        public required double Contraction { get; init; }
        public required double Shrink { get; init; }
    }

    public sealed record CompletionСriteria
    {
        public double? AbsoluteValue { get; init; }

        public int? MaxConsecutiveShrinks { get; init; }
    }
}
