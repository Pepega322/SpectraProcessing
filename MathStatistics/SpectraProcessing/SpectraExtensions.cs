using Domain.SpectraData;

namespace MathStatistics.SpectraProcessing;

public static class SpectraExtensions
{
	public static Spectra SubstractBaseLine(this Spectra s)
	{
		var baseline = MathRegressionAnalysis.GetLinearRegression(s.Points);
		var newPoints = s.Points.Transform(TransformationRule);
		return s.ChangePoints(newPoints);
		float TransformationRule(float x, float y) => y - baseline(x);
	}

	public static Spectra GetAverageSpectra(this IEnumerable<Spectra> spectras)
	{
		var spectraCountPerX = new Dictionary<float, int>();
		var spectraYSumForX = new Dictionary<float, float>();
		var spectraEnumerable = spectras as Spectra[] ?? spectras.ToArray();
		foreach (var spectra in spectraEnumerable)
		{
			var points = spectra.Points;
			for (var i = 0; i < points.Count; i++)
			{
				var x = points.X[i];
				var y = points.Y[i];
				if (!spectraCountPerX.TryAdd(x, 1))
					spectraCountPerX[x]++;
				if (!spectraYSumForX.TryAdd(x, y))
					spectraYSumForX[x] += y;
			}
		}

		var resultX = spectraCountPerX.Keys.OrderBy(x => x).ToList();
		var resultY = resultX
			.Select(x => spectraYSumForX[x] / spectraCountPerX[x])
			.ToList();
		var newPoints = new SpectraPoints(resultX, resultY);

		var result = spectraEnumerable.First().Copy();
		result.Name = "average";
		result.ChangePoints(newPoints);
		return result;
	}

	public static SpectraPeak ProcessPeak(this Spectra s, PeakBorders borders)
	{
		var leftIndex = MathRegressionAnalysis.ClosestIndexBinarySearch(s.Points.X, borders.XStart);
		var rightIndex = MathRegressionAnalysis.ClosestIndexBinarySearch(s.Points.X, borders.XEnd);
		var realBorders = new PeakBorders(s.Points[leftIndex].X, s.Points[rightIndex].X);
		var baseline = MathRegressionAnalysis.GetLinearRegression(s.Points[leftIndex], s.Points[rightIndex]);

		var square = 0f;
		var maxHeight = 0f;
		var height = GetHeight(leftIndex);
		for (var index = leftIndex; index < rightIndex; index++)
		{
			if (height > maxHeight) maxHeight = height;
			var nextHeight = GetHeight(index + 1);
			var dS = MathRegressionAnalysis.GetQuadrangleSquare(height, nextHeight, GetDeltaX(index));
			if (dS > 0) square += dS;
			height = nextHeight;
		}

		return new SpectraPeak(s, realBorders, square, maxHeight);

		float GetDeltaX(int index) => s.Points.X[index + 1] - s.Points.X[index];
		float GetHeight(int index) => s.Points.Y[index] - baseline(s.Points.X[index]);
	}
}