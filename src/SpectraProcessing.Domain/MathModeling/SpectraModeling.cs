using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling;

public static class SpectraModeling
{
    private const double areaPercentage = 0.95;

    public static Task<IReadOnlyList<IReadOnlyPeakData>> FitPeaks(
        this SpectraData spectra,
        IReadOnlyList<IReadOnlyPeakData> peaks,
        OptimizationSettings settings)
    {
        return Task.Run(() => spectra.FitPeaksInternal(peaks, settings));
    }

    private static async Task<IReadOnlyList<IReadOnlyPeakData>> FitPeaksInternal(
        this SpectraData spectra,
        IReadOnlyList<IReadOnlyPeakData> peaks,
        OptimizationSettings settings)
    {
        if (peaks.IsEmpty())
        {
            throw new ArgumentException("Peaks cannot be empty");
        }

        var groups = GetPeakGroups(spectra, peaks);

        var fittedGroups = await Task.WhenAll(groups.Select(g => g.FitGroup(spectra, settings)));

        return fittedGroups
            .OrderBy(g => g.Order)
            .SelectMany(g => g.Peaks)
            .ToArray();
    }

    private static async Task<PeaksGroup> FitGroup(
        this PeaksGroup group,
        SpectraData spectra,
        OptimizationSettings settings)
    {
        var startVector = new VectorN(
            group.Peaks
                .SelectMany(
                    p => new double[]
                    {
                        p.Center,
                        p.Amplitude,
                        p.HalfWidth,
                        p.GaussianContribution,
                    })
                .ToArray());

        var fittedVector = await NelderMead.Optimization(
            startVector,
            OptimizationFunc(spectra, group),
            settings);

        UpdatePeaks(group, fittedVector);

        return group;

        static Func<IReadOnlyVectorN, double> OptimizationFunc(SpectraData spectra, PeaksGroup group)
        {
            return OptimizationFunc;

            double OptimizationFunc(IReadOnlyVectorN vector)
            {
                UpdatePeaks(group, vector);

                Span<double> span = [];

                for (var i = group.StartIndex; i < group.EndIndex; i++)
                {
                    var delta = spectra.Points.Y[i] - group.Peaks.GetPeaksValueAt(spectra.Points.X[i]);
                    span.Fill(delta);
                }

                return span.GetStandardDeviation();
            }
        }

        static void UpdatePeaks(PeaksGroup group, IReadOnlyVectorN vector)
        {
            const int peakParametersCont = 4;

            if (vector.Dimension % peakParametersCont != group.Peaks.Count)
            {
                throw new ArgumentException("Vector length must be divisible by peak parameters.");
            }

            for (var i = 0; i < group.Peaks.Count; i++)
            {
                var peak = group.Peaks[i];
                peak.Center = (float) vector.Values[4 * i];
                peak.Amplitude = (float) vector.Values[4 * i + 1];
                peak.HalfWidth = (float) vector.Values[4 * i + 2];
                peak.GaussianContribution = (float) vector.Values[4 * i + 3];
            }
        }
    }

    private static IReadOnlyCollection<PeaksGroup> GetPeakGroups(
        SpectraData spectra,
        IReadOnlyCollection<IReadOnlyPeakData> peaks)
    {
        var effectiveRadius = peaks.ToDictionary(
            p => p,
            p => p.GetPeakEffectiveRadius(areaPercentage));

        var peaksByLeftBorder = peaks
            .OrderBy(x => x.Center - effectiveRadius[x])
            .ToArray();

        var groups = new List<List<PeakData>>()
        {
            new() { peaksByLeftBorder.First().Copy() },
        };

        foreach (var currentPeak in peaksByLeftBorder.Skip(1))
        {
            var lastSet = groups.Last();
            var lastPeak = lastSet.Last();

            var lastPeakRightBorder = lastPeak.Center + effectiveRadius[lastPeak];
            var currentPeakLeftBorder = currentPeak.Center - effectiveRadius[currentPeak];

            if (lastPeakRightBorder <= currentPeakLeftBorder)
            {
                groups.Add([currentPeak.Copy()]);
            }
            else
            {
                lastSet.Add(currentPeak.Copy());
            }
        }

        return groups.Select(
            (g, order) =>
            {
                var leftPeak = g.First();
                var rightPeak = g.Last();

                var startX = (float) (leftPeak.Center - effectiveRadius[leftPeak]);
                var endX = (float) (rightPeak.Center + effectiveRadius[rightPeak]);

                return new PeaksGroup(
                    Order: order,
                    Peaks: g,
                    StartIndex: spectra.Points.X.ClosestIndexBinarySearch(startX),
                    EndIndex: spectra.Points.X.ClosestIndexBinarySearch(endX));
            }).ToArray();
    }

    private sealed record PeaksGroup(
        int Order,
        IReadOnlyList<PeakData> Peaks,
        int StartIndex,
        int EndIndex
    );
}
