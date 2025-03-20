using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Models.PeakEstimate;

public sealed class PeakEstimateData : IPlottableData
{
    private static long _counter;

    private readonly long id;

    public float Amplitude { get; set; }

    public float Center { get; set; }

    public float HalfWidth { get; set; }

    public float GaussianContribution { get; set; }

    public PeakEstimateData(
        float center,
        float amplitude,
        float halfWidth,
        float gaussianContribution = 1)
    {
        if (gaussianContribution is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(gaussianContribution));
        }

        id = Interlocked.Increment(ref _counter);
        Amplitude = amplitude;
        Center = center;
        HalfWidth = halfWidth;
        GaussianContribution = gaussianContribution;
    }

    public override int GetHashCode() => HashCode.Combine(id);
}
