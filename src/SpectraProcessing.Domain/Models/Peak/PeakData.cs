using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Domain.Models.Peak;

public sealed class PeakData : IPlottableData
{
    private static long _counter;

    private readonly long id;

    public float Amplitude { get; set; }

    public float Center { get; set; }

    public float HalfWidth { get; set; }

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
        Amplitude = amplitude;
        Center = center;
        HalfWidth = halfWidth;
        GaussianContribution = gaussianContribution;
    }

    public PeakData Copy()
        => new(
            center: Center,
            amplitude: Amplitude,
            halfWidth: HalfWidth,
            gaussianContribution: GaussianContribution);

    public override bool Equals(object? obj) => obj is PeakData data && data.id == id;

    public override int GetHashCode() => id.GetHashCode();
}
