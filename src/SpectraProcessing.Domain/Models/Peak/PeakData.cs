using SpectraProcessing.Domain.Extensions;

namespace SpectraProcessing.Domain.Models.Peak;

public sealed class PeakData : IReadOnlyPeakData
{
    private static long _counter;

    private readonly long id;

    public float Center { get; set; }

    public float HalfWidth { get; set; }

    public float Amplitude { get; set; }

    public float GaussianContribution { get; set; }

    public PeakData(
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
        Center = center;
        HalfWidth = halfWidth;
        Amplitude = amplitude;
        GaussianContribution = gaussianContribution;
    }

    public PeakData Copy()
        => new(
            center: Center,
            amplitude: Amplitude,
            halfWidth: HalfWidth,
            gaussianContribution: GaussianContribution);

    public override bool Equals(object? obj)
        => obj is PeakData data
            && Amplitude.ApproximatelyEqual(data.Amplitude)
            && Center.ApproximatelyEqual(data.Center)
            && HalfWidth.ApproximatelyEqual(data.HalfWidth)
            && GaussianContribution.ApproximatelyEqual(data.GaussianContribution);

    public override int GetHashCode() => id.GetHashCode();
}
