﻿namespace MathStatistics;

internal static class MathDispersionAnalysis
{
	public static DispersionStatistics<float> GetDispersionStatistics(this ICollection<float> values)
	{
		if (values.Count <= 1)
			throw new IndexOutOfRangeException("Collection must contains at least two elements");

		var standardDeviation = values.GetStandardDeviation(out var averageValue);
		var confidenceInterval = GetConfidenceInterval(values.Count, standardDeviation);
		return new DispersionStatistics<float>(
			values.Count,
			averageValue,
			standardDeviation,
			confidenceInterval);
	}

	private static float GetStandardDeviation(this ICollection<float> values, out float averageValue)
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
			throw new IndexOutOfRangeException("Values count must be greater than 1");
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
			_  => 1.96f
		};
	}
}