using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.MathModeling;

public static class DispersionAnalysis
{
    public static DispersionStatistics<double> GetDispersionStatistics(
        this IReadOnlyCollection<double> values,
        string parameterName)
    {
        if (values.Count <= 1)
        {
            throw new IndexOutOfRangeException("Collection must contains at least two elements");
        }

        var standardDeviation = values.GetStandardDeviation(out var averageValue);

        var relativeDeviation = standardDeviation / averageValue * 100;

        var confidenceInterval = GetConfidenceInterval(values.Count, standardDeviation);

        return new DispersionStatistics<double>(
            parameterName,
            values.Count,
            averageValue,
            standardDeviation,
            relativeDeviation,
            confidenceInterval);
    }

    public static double GetStandardDeviation(this IReadOnlyCollection<double> values, out double averageValue)
    {
        var average = values.Sum() / values.Count;
        var standardDeviationSquare = values.Sum(v => (v - average) * (v - average)) / (values.Count - 1);
        averageValue = average;
        return Math.Sqrt(standardDeviationSquare);
    }

    public static double GetStandardDeviation(this ref Span<double> values)
    {
        var average = values.Sum() / values.Length;

        var standardDeviationSquare = values.Sum(v => (v - average) * (v - average)) / (values.Length - 1);

        return Math.Sqrt(standardDeviationSquare);
    }

    private static double GetConfidenceInterval(int valuesCount, double standardDeviation)
        => GetStudentCoefficients(valuesCount) * standardDeviation / Math.Sqrt(valuesCount);

    private static double GetStudentCoefficients(int valuesCount)
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
}
