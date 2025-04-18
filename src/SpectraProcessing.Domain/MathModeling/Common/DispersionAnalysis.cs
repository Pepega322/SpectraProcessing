using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.MathModeling.Common;

namespace SpectraProcessing.Domain.MathModeling.Common;

public static class DispersionAnalysis
{
    public static DispersionStatistics<float> GetDispersionStatistics(
        this IReadOnlyCollection<float> values,
        string parameterName)
    {
        if (values.Count <= 1)
        {
            throw new IndexOutOfRangeException("Collection must contains at least two elements");
        }

        var standardDeviation = values.GetStandardDeviation(out var averageValue);

        var relativeDeviation = standardDeviation / averageValue * 100;

        var confidenceInterval = GetConfidenceInterval(values.Count, standardDeviation);

        return new DispersionStatistics<float>(
            parameterName,
            values.Count,
            averageValue,
            standardDeviation,
            relativeDeviation,
            confidenceInterval);
    }

    public static float GetRelativeDeviation(float value, float realValue)
    {
        return Math.Abs((value - realValue) / realValue);
    }

    public static float GetStandardDeviation(this IReadOnlyCollection<float> values, out float averageValue)
    {
        var average = values.Sum() / values.Count;
        var standardDeviationSquare = values.Sum(v => (v - average) * (v - average)) / (values.Count - 1);
        averageValue = average;
        return (float) Math.Sqrt(standardDeviationSquare);
    }

    private static float GetConfidenceInterval(int valuesCount, float standardDeviation)
        => GetStudentCoefficients(valuesCount) * standardDeviation / (float) Math.Sqrt(valuesCount);

    private static float GetStudentCoefficients(int valuesCount)
    {
        if (valuesCount <= 1)
        {
            throw new IndexOutOfRangeException("Values count must be greater than 1");
        }

        return valuesCount switch
        {
            2  => 12.7f,
            3  => 4.30f,
            4  => 3.18f,
            5  => 2.77f,
            6  => 2.57f,
            7  => 2.45f,
            8  => 2.36f,
            9  => 2.31f,
            10 => 2.26f,
            _  => 1.96f,
        };
    }

    public static float GetR2Coefficient(
        in Span<float> realValues,
        in Span<float> predictedValues)
    {
        if (realValues.Length != predictedValues.Length)
        {
            throw new IndexOutOfRangeException("Real-time values and predicted values do not match");
        }

        var residualSumOfSquares = 0f;

        for (var i = 0; i < realValues.Length; i++)
        {
            var delta = realValues[i] - predictedValues[i];
            residualSumOfSquares += delta * delta;
        }

        var average = realValues.Sum() / realValues.Length;

        var totalSumOfSquares = 0f;

        for (var i = 0; i < realValues.Length; i++)
        {
            var delta = average - predictedValues[i];
            totalSumOfSquares += delta * delta;
        }

        return 1 - residualSumOfSquares / totalSumOfSquares;
    }
}
