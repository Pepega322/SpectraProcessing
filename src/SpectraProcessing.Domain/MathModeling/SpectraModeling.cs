using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling;

public static class SpectraModeling
{
    private const double areaPercentage = 0.95;

    public static Task FitPeaks(
        this SpectraData spectra,
        IReadOnlyList<PeakData> peaks,
        OptimizationSettings settings)
    {
        return FitPeaksInternal(spectra, peaks, settings);

        static Task FitPeaksInternal(
            SpectraData spectra,
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
        const int peakParametersCount = 3;

        var startValues = new List<double>();

        foreach (var peak in group)
        {
            startValues.Add(peak.Center);
            startValues.Add(peak.Amplitude);
            startValues.Add(peak.HalfWidth);
            // startValues.Add(peak.GaussianContribution);
        }

        var optimizedVector = await NelderMead.GetOptimized(
            new VectorN(startValues),
            OptimizationFunc(spectra, group, settings),
            settings);

        UpdatePeaks(group, optimizedVector);

        return;

        static Func<IReadOnlyVectorN, double> OptimizationFunc(
            SpectraData spectra,
            IReadOnlyList<PeakData> group,
            OptimizationSettings settings)
        {
            return OptimizationFunc;

            double OptimizationFunc(IReadOnlyVectorN vector)
            {
                UpdatePeaks(group, vector);

                var (startIndex, endIndex) = group.GetBorders(spectra);

                Span<float> relativeDeviations = stackalloc float[endIndex - startIndex + 1];

                for (var i = startIndex; i < endIndex; i++)
                {
                    var realValue = spectra.Points.Y[i];
                    var currentValue = group.GetPeaksValueAt(spectra.Points.X[i]);

                    relativeDeviations[startIndex - i] =
                        (float) DispersionAnalysis.GetRelativeDeviation(currentValue, realValue);
                }

                var goodPointsCount =
                    relativeDeviations.Count(deviation => deviation < settings.MaxAcceptableRelativeDeviation);

                throw new NotImplementedException();
            }
        }

        static void UpdatePeaks(IReadOnlyList<PeakData> group, IReadOnlyVectorN vector)
        {
            if (vector.Dimension / peakParametersCount != group.Count)
            {
                throw new ArgumentException("Vector length must be divisible by peak parameters.");
            }

            for (var i = 0; i < group.Count; i++)
            {
                var peak = group[i];
                peak.Center = (float) vector.Values[peakParametersCount * i];
                peak.Amplitude = (float) vector.Values[peakParametersCount * i + 1];
                peak.HalfWidth = (float) vector.Values[peakParametersCount * i + 2];
                // peak.GaussianContribution = (float) vector.Values[peakParametersCount * i + 3];
            }
        }
    }

    private static (int StartIndex, int EndIndex) GetBorders(this IReadOnlyList<PeakData> peaks, SpectraData spectra)
    {
        var startValue = float.MaxValue;
        var endValue = float.MinValue;

        foreach (var peak in peaks)
        {
            var effectiveRadius = peak.GetPeakEffectiveRadius(areaPercentage);
            startValue = (float) Math.Min(startValue, peak.Center - effectiveRadius);
            endValue = (float) Math.Max(endValue, peak.Center + effectiveRadius);
        }

        var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
        var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);

        return (startIndex, endIndex);
    }
}
