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

            var groups = GetPeakGroups(spectra, peaks);

            return Task.WhenAll(groups.Select(g => g.FitGroup(spectra, settings)));
        }
    }

    private static IReadOnlyCollection<PeaksGroup> GetPeakGroups(
        SpectraData spectra,
        IReadOnlyCollection<PeakData> peaks)
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

        return groups.Select(
            g =>
            {
                var leftPeak = g.First();
                var rightPeak = g.Last();

                var startX = (float) (leftPeak.Center - effectiveRadius[leftPeak]);
                var endX = (float) (rightPeak.Center + effectiveRadius[rightPeak]);

                return new PeaksGroup(
                    Peaks: g,
                    StartIndex: spectra.Points.X.ClosestIndexBinarySearch(startX),
                    EndIndex: spectra.Points.X.ClosestIndexBinarySearch(endX));
            }).ToArray();
    }

    private static async Task FitGroup(
        this PeaksGroup group,
        SpectraData spectra,
        OptimizationSettings settings)
    {
        const int peakParametersCount = 3;

        var startValues = new List<double>();

        foreach (var peak in group.Peaks)
        {
            startValues.Add(peak.Center);
            startValues.Add(peak.Amplitude);
            startValues.Add(peak.HalfWidth);
        }

        var optimizedVector = await NelderMead.GetOptimized(
            new VectorN(startValues.ToArray()),
            OptimizationFunc(spectra, group),
            settings);

        UpdatePeaks(group, optimizedVector);

        return;

        static Func<IReadOnlyVectorN, double> OptimizationFunc(SpectraData spectra, PeaksGroup group)
        {
            return OptimizationFunc;

            double OptimizationFunc(IReadOnlyVectorN vector)
            {
                UpdatePeaks(group, vector);

                Span<double> delta = stackalloc double[group.EndIndex - group.StartIndex + 1];

                var count = 0;
                for (var i = group.StartIndex; i < group.EndIndex; i++)
                {
                    delta[count] = spectra.Points.Y[i] - group.Peaks.GetPeaksValueAt(spectra.Points.X[i]);
                    count++;
                }

                return delta.GetStandardDeviation();
            }
        }

        static void UpdatePeaks(PeaksGroup group, IReadOnlyVectorN vector)
        {
            if (vector.Dimension / peakParametersCount != group.Peaks.Count)
            {
                throw new ArgumentException("Vector length must be divisible by peak parameters.");
            }

            for (var i = 0; i < group.Peaks.Count; i++)
            {
                var peak = group.Peaks[i];
                peak.Center = (float) vector.Values[peakParametersCount * i];
                peak.Amplitude = (float) vector.Values[peakParametersCount * i + 1];
                peak.HalfWidth = (float) vector.Values[peakParametersCount * i + 2];
            }
        }
    }

    private sealed record PeaksGroup(
        IReadOnlyList<PeakData> Peaks,
        int StartIndex,
        int EndIndex
    );
}
