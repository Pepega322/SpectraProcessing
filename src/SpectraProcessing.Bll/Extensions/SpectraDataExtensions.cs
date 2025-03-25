using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.MathModels;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Extensions;

public static class SpectraDataExtensions
{
    public static SpectraData SubstractBaseLine(this SpectraData spectra)
    {
        var baseline = RegressionAnalysis.GetLinearRegression(spectra.Points);

        var points = spectra.Points.Transform(TransformationRule);

        return new SimpleSpectraData($"{spectra.Name} -b", points);

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

            return new SimpleSpectraData("average", points);
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

            return new SimpleSpectraData("sum", points);
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
