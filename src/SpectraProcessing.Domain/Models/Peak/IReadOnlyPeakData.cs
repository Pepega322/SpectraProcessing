using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Domain.Models.Peak;

public interface IReadOnlyPeakData : IPlottableData
{
    float Center { get; }

    float HalfWidth { get; }

    float Amplitude { get; }

    float GaussianContribution { get; }

    PeakData Copy();
}
