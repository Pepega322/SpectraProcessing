namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed record OptimizationSettings
{
    public int MaxIterationsCount { get; init; } = 100000;

    public SimplexSettings SimplexSettings { get; init; } = new();

    public IterationCoefficients Coefficients { get; init; } = new();

    public CompletionСriteria Сriteria { get; init; } = new();

    public sealed record CompletionСriteria
    {
        public double? AbsoluteValue { get; init; }
    }

    public sealed record IterationCoefficients
    {
        public double Reflection { get; init; } = 1;

        public double Expansion { get; init; } = 2;

        public double Contraction { get; init; } = 0.5;

        public double Shrink { get; init; } = 0.5;
    }
}

public sealed record SimplexSettings
{
    public double InitialShift { get; init; } = 0.01;
}
