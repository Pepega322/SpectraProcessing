using Domain.SpectraData.Processing;
using Domain.Storage;

namespace Domain.SpectraData;

public static class SpectraExtensions
{
	public static Spectra SubstractBaseLine(this Spectra s)
	{
		var baseline = MathOperations.GetLinearRegression(s.Points);
		var newPoints = s.Points.Transform(TransformationRule);
		return s.ChangePoints(newPoints);
		float TransformationRule(float x, float y) => y - baseline(x);
	}

	public static Spectra GetAverageSpectra(this IEnumerable<Spectra> spectras, out Metrology metrology)
	{
		var spectrasPerX = new Dictionary<float, int>();
		var spectrasYSum = new Dictionary<float, float>();
		foreach (var spectra in spectras)
		{
			var points = spectra.Points;
			for (var i = 0; i < points.Count; i++)
			{
				var x = points.X[i];
				var y = points.Y[i];
				if (!spectrasPerX.TryAdd(x, 1))
					spectrasPerX[x]++;
				if (!spectrasYSum.TryAdd(x, y))
					spectrasYSum[x] += y;
			}
		}

		var resultX = spectrasPerX.Keys.OrderBy(x => x).ToList();
		var resultY = resultX
			.Select(x => spectrasYSum[x] / spectrasPerX[x])
			.ToList();
		var newPoints = new SpectraPoints(resultX, resultY);
		var result = spectras.First().Copy();
		result.Name = "average";
		result.ChangePoints(newPoints);
		//TODO Metrology
		metrology = null;
		return result;
	}

	public static PeakInfo ProcessPeak(this Spectra s, PeakBorder border)
	{
		var left = MathOperations.GetClosestIndex(s.Points.X, border.Left);
		var right = MathOperations.GetClosestIndex(s.Points.X, border.Right);
		var baseline = MathOperations.GetLinearRegression(s.Points[left], s.Points[right]);

		var square = 0f;
		var maxHeight = 0f;
		var height = GetHeight(left);
		for (var index = left; index < right; index++)
		{
			if (height > maxHeight) maxHeight = height;
			var nextHeigth = GetHeight(index + 1);
			var dS = MathOperations.GetQuadrangleSquare(height, nextHeigth, GetDeltaX(index));
			if (dS > 0) square += dS;
			height = nextHeigth;
		}

		return new PeakInfo(s, s.Points.X[left], s.Points.X[right], square, maxHeight);
		float GetDeltaX(int index) => s.Points.X[index + 1] - s.Points.X[index];
		float GetHeight(int index) => s.Points.Y[index] - baseline(s.Points.X[index]);
	}

	public static DataSet<Spectra> SetOnlySubstractBaselineAsync(DataSet<Spectra> set)
	{
		var destination = new DataSet<Spectra>($"{set.Name} -b");
		Parallel.ForEach(set, spectra =>
		{
			var substracted = spectra.SubstractBaseLine();
			destination.AddThreadSafe(substracted);
		});
		return destination;
	}

	public static DataSet<Spectra> SetFullDepthSubstractBaselineAsync(DataSet<Spectra> set)
	{
		var refToCopy = set.CopyBranchStructureThreadSafe($"{set.Name} -b");
		var root = refToCopy[set];
		Parallel.ForEach(refToCopy.Keys, reference =>
		{
			Parallel.ForEach(reference, spectra =>
			{
				var substracted = spectra.SubstractBaseLine();
				refToCopy[reference].AddThreadSafe(substracted);
			});
		});
		return root;
	}
}