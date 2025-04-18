namespace SpectraProcessing.Domain.Extensions;

public static class ComparisonsExtensions
{
    public const double DoubleTolerance = 1e-6;

    public const float FloatTolerance = 1e-6f;

    public static bool ApproximatelyEqual(this double a, double b, double tolerance = DoubleTolerance)
        => Math.Abs(a - b) <= tolerance;

    public static bool ApproximatelyEqual(this float a, float b, float tolerance = FloatTolerance)
        => Math.Abs(a - b) <= tolerance;
}
