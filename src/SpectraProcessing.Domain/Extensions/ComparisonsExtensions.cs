namespace SpectraProcessing.Domain.Extensions;

public static class ComparisonsExtensions
{
    public static bool ApproximatelyEqual(this double a, double b, double tolerance) => Math.Abs(a - b) <= tolerance;
}
