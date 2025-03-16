using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Models.PeakEstimate;

public sealed class PeakEstimateData : IPlottableData
{
    private static long _counter;

    private readonly long id;

    public float Amplitude { get; set; }

    public float Center { get; set; }

    public float HalfWidth { get; set; }

    public PeakEstimateData(
        float center,
        float amplitude,
        float halfWidth)
    {
        id = Interlocked.Increment(ref _counter);
        Amplitude = amplitude;
        Center = center;
        HalfWidth = halfWidth;
    }

    public override int GetHashCode() => HashCode.Combine(id);
}
