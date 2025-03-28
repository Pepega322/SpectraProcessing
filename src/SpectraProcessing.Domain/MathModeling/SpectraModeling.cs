using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling;

public static class SpectraModeling
{
    private const double areaPercentage = 0.95;

    public static Task<IReadOnlyCollection<PeakData>> FitPeaks(
        this SpectraData spectra,
        IReadOnlyCollection<PeakData> peaks,
        OptimizationSettings settings)
    {
        return Task.Run(() => spectra.FitPeaksInternal(peaks, settings));
    }

    private static async Task<IReadOnlyCollection<PeakData>> FitPeaksInternal(
        this SpectraData spectra,
        IReadOnlyCollection<PeakData> peaks,
        OptimizationSettings settings)
    {
        if (peaks.IsEmpty())
        {
            throw new ArgumentException("Peaks cannot be empty");
        }

        var groups = GetPeakGroups(peaks);

        var fittedGroups = await Task.WhenAll(groups.Select(g => GetFittedGroup(spectra, g, settings)));

        return fittedGroups
            .OrderBy(g => g.Order)
            .SelectMany(g => g.Peaks)
            .ToArray();
    }

    private static async Task<PeaksGroup> GetFittedGroup(
        SpectraData spectra,
        PeaksGroup group,
        OptimizationSettings settings)
    {
        var startVector = new VectorN(
            group.Peaks.SelectMany(
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

        var fittedPeaks = ToPeaks(fittedVector);

        return group with { Peaks = fittedPeaks };

        static Func<VectorN, double> OptimizationFunc(SpectraData spectra, PeaksGroup group)
        {
            var startIndex = spectra.Points.X.ClosestIndexBinarySearch(group.StartX);
            var endIndex = spectra.Points.X.ClosestIndexBinarySearch(group.EndX);

            return OptimizationFunc;

            double OptimizationFunc(VectorN vector)
            {
                var peaks = ToPeaks(vector);

                Span<double> span = [];

                for (var i = startIndex; i < endIndex; i++)
                {
                    var delta = spectra.Points.Y[i] - peaks.GetPeaksValueAt(spectra.Points.X[i]);
                    span.Fill(delta);
                }

                return span.GetStandardDeviation();
            }
        }

        static IReadOnlyCollection<PeakData> ToPeaks(VectorN vector)
        {
            const int peakParametersCont = 4;

            if (vector.Dimension % peakParametersCont != 0)
            {
                throw new ArgumentException("Vector length must be divisible by peak parameters.");
            }

            var peaksCount = vector.Dimension / peakParametersCont;

            var result = new List<PeakData>();
            for (var index = 0; index < vector.Dimension; index += peakParametersCont)
            {
                var peak = new PeakData(
                    (float) vector[index],
                    (float) vector[index + 1],
                    (float) vector[index + 2],
                    (float) vector[index + 3]);

                result.Add(peak);
            }

            return result;
        }
    }

    private static IReadOnlyCollection<PeaksGroup> GetPeakGroups(IReadOnlyCollection<PeakData> peaks)
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
            (g, order) =>
            {
                var leftPeak = g.First();
                var rightPeak = g.Last();

                return new PeaksGroup(
                    Order: order,
                    Peaks: g,
                    StartX: (float) (leftPeak.Center - effectiveRadius[leftPeak]),
                    EndX: (float) (rightPeak.Center + effectiveRadius[rightPeak]));
            }).ToArray();
    }

    private sealed record PeaksGroup(
        int Order,
        IReadOnlyCollection<PeakData> Peaks,
        float StartX,
        float EndX
    );
}
