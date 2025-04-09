using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling;

public static class SpectraModeling
{
    private const int peakParametersCount = 3;
    private const float areaPercentage = 0.95f;

    public static Task FitPeaks(
        this SpectraData spectra,
        IReadOnlyList<PeakData> peaks,
        OptimizationSettings settings)
    {
        if (peaks.IsEmpty())
        {
            throw new ArgumentException("Peaks cannot be empty");
        }

        var groups = peaks.GroupPeaksByEffectiveRadius();

        return Task.WhenAll(groups.Select(g => g.FitPeaksGroup(spectra, settings)));
    }

    private static IReadOnlyCollection<IReadOnlyList<PeakData>> GroupPeaksByEffectiveRadius(
        this IReadOnlyCollection<PeakData> peaks)
    {
        var effectiveRadius = peaks.ToDictionary(
            p => p,
            p => p.GetPeakEffectiveRadius(areaPercentage));

        var peaksByLeftBorder = peaks
            .OrderBy(x => x.Center - effectiveRadius[x])
            .ToArray();

        var groups = new List<List<PeakData>>()
        {
            new() { peaksByLeftBorder.First() },
        };

        foreach (var currentPeak in peaksByLeftBorder.Skip(1))
        {
            var lastSet = groups.Last();
            var lastPeak = lastSet.Last();

            var lastPeakRightBorder = lastPeak.Center + effectiveRadius[lastPeak];
            var currentPeakLeftBorder = currentPeak.Center - effectiveRadius[currentPeak];

            if (lastPeakRightBorder <= currentPeakLeftBorder)
            {
                groups.Add([currentPeak]);
            }
            else
            {
                lastSet.Add(currentPeak);
            }
        }

        return groups;
    }

    private static async Task FitPeaksGroup(
        this IReadOnlyList<PeakData> group,
        SpectraData spectra,
        OptimizationSettings settings)
    {
        var startValues = new float[group.Count * peakParametersCount];

        for (var i = 0; i < group.Count; i++)
        {
            var peak = group[i];
            startValues[peakParametersCount * i] = peak.Center;
            startValues[peakParametersCount * i + 1] = peak.Amplitude;
            startValues[peakParametersCount * i + 2] = peak.HalfWidth;
            // startValues[peakParametersCount * i + 3] = group[i].GaussianContribution;
        }

        var optimizedVector = await NelderMead.GetOptimized(
            new VectorN(startValues),
            OptimizationFunc(spectra),
            settings);

        for (var i = 0; i < group.Count; i++)
        {
            var peak = group[i];
            peak.Center = optimizedVector.Values[peakParametersCount * i];
            peak.Amplitude = optimizedVector.Values[peakParametersCount * i + 1];
            peak.HalfWidth = optimizedVector.Values[peakParametersCount * i + 2];
            // peak.GaussianContribution = (float) vector.Values[peakParametersCount * i + 3];
        }

        return;

        static Func<VectorNRefStruct, float> OptimizationFunc(SpectraData spectra)
        {
            return OptimizationFunc;

            float OptimizationFunc(VectorNRefStruct vector)
            {
                if (vector.Dimension % peakParametersCount != 0)
                {
                    throw new ArgumentException("Vector length must be divisible by peak parameters.");
                }

                var (startValue, endValue) = vector.GetBorders();
                var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
                var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);

                var average = 0f;
                var residualSumOfSquares = 0f;

                for (var i = startIndex; i < endIndex; i++)
                {
                    average += spectra.Points.Y[i];

                    var delta = spectra.Points.Y[i] - vector.GetPeaksValueAt(spectra.Points.X[i]);
                    residualSumOfSquares += delta * delta;
                }

                average /= endIndex - startIndex;

                var totalSumOfSquares = 0f;

                for (var i = startIndex; i < endIndex; i++)
                {
                    var delta = spectra.Points.Y[i] - average;
                    totalSumOfSquares += delta * delta;
                }

                var r2Coefficient = 1 - residualSumOfSquares / totalSumOfSquares;

                return 1 - r2Coefficient;
            }
        }
    }

    private static float GetPeaksValueAt(this in VectorNRefStruct peaksVector, float x)
    {
        var peaksCount = peaksVector.Dimension / peakParametersCount;

        var value = 0f;

        for (var i = 0; i < peaksCount; i++)
        {
            var center = peaksVector.Values[peakParametersCount * i];
            var halfWidth = peaksVector.Values[peakParametersCount * i + 2];
            var amplitude = peaksVector.Values[peakParametersCount * i + 1];
            const int gaussianContribution = 1;

            value += PeakModeling.GetPeakValueAt(
                x: x,
                center: center,
                halfWidth: halfWidth,
                amplitude: amplitude,
                gaussianContribution: gaussianContribution);
        }

        return value;
    }

    private static (float StartValue, float EndValue) GetBorders(this in VectorNRefStruct peaksVector)
    {
        var peaksCount = peaksVector.Dimension / peakParametersCount;

        var startValue = float.MaxValue;
        var endValue = float.MinValue;

        for (var i = 0; i < peaksCount; i++)
        {
            var center = peaksVector.Values[peakParametersCount * i];
            var halfWidth = peaksVector.Values[peakParametersCount * i + 2];
            const int gaussianContribution = 1;

            var effectiveRadius = PeakModeling.GetPeakEffectiveRadius(
                halfWidth: halfWidth,
                gaussianContribution: gaussianContribution,
                areaPercentage: areaPercentage);

            startValue = Math.Min(startValue, center - effectiveRadius);
            endValue = Math.Max(endValue, center + effectiveRadius);
        }

        return (startValue, endValue);
    }
}
