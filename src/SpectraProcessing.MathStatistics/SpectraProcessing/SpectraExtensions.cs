using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.PeakEstimate;
using SpectraProcessing.Models.Spectra;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.MathStatistics.SpectraProcessing;

public static class SpectraExtensions
{
    public static SpectraData SubstractBaseLine(this SpectraData spectra)
    {
        var baseline = MathRegressionAnalysis.GetLinearRegression(spectra.Points);

        var points = spectra.Points.Transform(TransformationRule);

        return new EstimatedSpectraData($"{spectra.Name} -b", points);

        float TransformationRule(float x, float y) => y - baseline(x);
    }

    public static Task<SpectraData> GetAverageSpectra(this IReadOnlyCollection<SpectraData> spectras)
    {
        return Task.Run(() => GetAverageSpectraInternal(spectras));

        SpectraData GetAverageSpectraInternal(IReadOnlyCollection<SpectraData> s)
        {
            var spectraCountPerX = new Dictionary<float, int>();
            var spectraYSumForX = new Dictionary<float, float>();

            foreach (var spectra in s)
            {
                for (var i = 0; i < spectra.Points.Count; i++)
                {
                    var x = spectra.Points.X[i];
                    var y = spectra.Points.Y[i];

                    if (!spectraCountPerX.TryAdd(x, 1))
                    {
                        spectraCountPerX[x]++;
                    }

                    if (!spectraYSumForX.TryAdd(x, y))
                    {
                        spectraYSumForX[x] += y;
                    }
                }
            }

            var resultX = spectraCountPerX.Keys.OrderBy(x => x).ToArray();

            var resultY = resultX
                .Select(x => spectraYSumForX[x] / spectraCountPerX[x])
                .ToArray();

            var points = new SpectraPoints(resultX, resultY);

            return new EstimatedSpectraData("average", points);
        }
    }

    public static Task<SpectraData> GetSumSpectra(this IReadOnlyCollection<SpectraData> spectras)
    {
        return Task.Run(() => GetSumSpectraInternal(spectras));

        SpectraData GetSumSpectraInternal(IReadOnlyCollection<SpectraData> s)
        {
            var spectraYSumForX = new Dictionary<float, float>();

            foreach (var spectra in s)
            {
                for (var i = 0; i < spectra.Points.Count; i++)
                {
                    var x = spectra.Points.X[i];
                    var y = spectra.Points.Y[i];

                    if (!spectraYSumForX.TryAdd(x, y))
                    {
                        spectraYSumForX[x] += y;
                    }
                }
            }

            var resultX = spectraYSumForX.Keys.OrderBy(x => x).ToArray();

            var resultY = resultX
                .Select(x => spectraYSumForX[x])
                .ToArray();

            var points = new SpectraPoints(resultX, resultY);

            return new EstimatedSpectraData("sum", points);
        }
    }

    public static async Task<IDictionary<PeakEstimateData, SpectraData>> GetPeaksSpectras(
        this SpectraData spectra,
        IReadOnlyCollection<PeakEstimateData> peakEstimates)
    {
        var tasks = peakEstimates
            .Select(e => Task.Run(() => GetPeakSpectra(spectra.Points.X, e)));

        var results = await Task.WhenAll(tasks);

        return results.ToDictionary(x => x.Item1, x => x.Item2);

        (PeakEstimateData, SpectraData) GetPeakSpectra(IReadOnlyList<float> xs, PeakEstimateData estimate)
        {
            var ys = xs.Select(x => (float) MathFunctions.GaussianAndLorentzianMix(x, estimate)).ToArray();

            var points = new SpectraPoints(xs, ys);

            return (estimate, new EstimatedSpectraData("", points));
        }
    }

    // public static SpectraPeak ProcessPeak(this Spectra s, PeakInfo info)
    // {
    //     var leftIndex = MathRegressionAnalysis.ClosestIndexBinarySearch(s.Points.X, info.XStart);
    //     var rightIndex = MathRegressionAnalysis.ClosestIndexBinarySearch(s.Points.X, info.XEnd);
    //     // var realBorders = new PeakInfo(s.Points[leftIndex].X, s.Points[rightIndex].X);
    //     var baseline = MathRegressionAnalysis.GetLinearRegression(s.Points[leftIndex], s.Points[rightIndex]);
    //
    //     var square = 0f;
    //     var maxHeight = 0f;
    //     var height = GetHeight(leftIndex);
    //     for (var index = leftIndex; index < rightIndex; index++)
    //     {
    //         if (height > maxHeight)
    //         {
    //             maxHeight = height;
    //         }
    //
    //         var nextHeight = GetHeight(index + 1);
    //
    //         var dS = MathRegressionAnalysis.GetQuadrangleSquare(height, nextHeight, GetDeltaX(index));
    //
    //         if (dS > 0)
    //         {
    //             square += dS;
    //         }
    //
    //         height = nextHeight;
    //     }
    //
    //     return new SpectraPeak
    //     {
    //         SpectraName = s.Name,
    //         Info = realBorders,
    //         Square = square,
    //         Height = maxHeight,
    //     };
    //
    //     float GetDeltaX(int index) => s.Points.X[index + 1] - s.Points.X[index];
    //     float GetHeight(int index) => s.Points.Y[index] - baseline(s.Points.X[index]);
    // }
}
