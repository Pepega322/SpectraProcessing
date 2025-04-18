﻿using System.Numerics;

namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed record DispersionStatistics<T>(
    string ParameterName,
    int ValuesCount,
    T AverageValue,
    T StandardDeviation,
    T RelativeDeviation,
    T ConfidenceInterval
) where T : struct, INumber<T>;
