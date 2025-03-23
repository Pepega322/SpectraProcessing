using System.Numerics;

namespace SpectraProcessing.Bll.Models.Math;

public record DispersionStatistics<T>(
    string ParameterName,
    int ValuesCount,
    T AverageValue,
    T StandardDeviation,
    T RelativeDeviation,
    T ConfidenceInterval
) where T : struct, INumber<T>;
