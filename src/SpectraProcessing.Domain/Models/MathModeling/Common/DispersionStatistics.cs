using System.Numerics;

namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public sealed record DispersionStatistics<T>(
    string ParameterName,
    int ValuesCount,
    T AverageValue,
    T StandardDeviation,
    T RelativeDeviation,
    T ConfidenceInterval
) where T : struct, INumber<T>;
