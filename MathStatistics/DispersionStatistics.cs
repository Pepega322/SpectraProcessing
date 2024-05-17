using System.Numerics;

namespace MathStatistics;

public record DispersionStatistics<T>(
	int ValuesCount,
	T AverageValue,
	T StandardDeviation,
	T ConfidenceInterval
) where T : struct, INumber<T>;