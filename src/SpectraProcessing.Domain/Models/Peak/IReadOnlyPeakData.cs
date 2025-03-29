using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Domain.Models.Peak;

public interface IReadOnlyPeakData : IPlottableData
{
    float Amplitude { get; }

    float Center { get; }

    float HalfWidth { get; }

    float GaussianContribution { get; }

    PeakData Copy();
}
