using SpectraProcessing.Domain.Enums;
using SpectraProcessing.Domain.Models.MathModeling.Common;
using SpectraProcessing.Domain.Models.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling.Peaks;

public static class SpectraModeling
{
    public const int PeakParametersCount = 4;

    private static readonly ValueConstraint GaussianContributionConstraint = new(0, 1);

    public static   Task FitPeaks(
        this SpectraData spectra,
        IReadOnlyCollection<PeakData> peaks,
        NedlerMeadSettings settings)
    {
        var groups = peaks.GroupPeaksByEffectiveRadius();

        return Parallel.ForEachAsync(groups, (g, _) => g.FitPeaksGroup(spectra, settings));
    }

    private static List<List<PeakData>> GroupPeaksByEffectiveRadius(this IReadOnlyCollection<PeakData> peaks)
    {
        var effectiveRadius = peaks
            .ToDictionary(
                p => p,
                p => p.GetPeakEffectiveRadius());

        var peaksByLeftBorder = peaks
            .OrderBy(x => x.Center - effectiveRadius[x])
            .ToArray();

        var groups = new List<List<PeakData>>
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

    private static async ValueTask FitPeaksGroup(
        this List<PeakData> peaks,
        SpectraData spectra,
        NedlerMeadSettings settings)
    {
        var startValues = new float[peaks.Count * PeakParametersCount];
        var constraints = new Dictionary<int, ValueConstraint>();

        for (var i = 0; i < peaks.Count; i++)
        {
            var center = peaks[i].Center;
            var halfWidth = peaks[i].HalfWidth;

            var centerIndex = PeakParametersCount * i;
            startValues[centerIndex] = center;
            constraints[centerIndex] = new ValueConstraint(center - halfWidth / 2, center + halfWidth / 2);

            var halfWidthIndex = PeakParametersCount * i + 1;
            startValues[halfWidthIndex] = halfWidth;
            constraints[halfWidthIndex] = new ValueConstraint(0, halfWidth * 1.25f);

            var amplitudeIndex = PeakParametersCount * i + 2;
            startValues[amplitudeIndex] = peaks[i].Amplitude;
            constraints[amplitudeIndex] = new ValueConstraint(0, float.MaxValue);

            var gaussianContributionIndex = PeakParametersCount * i + 3;
            startValues[gaussianContributionIndex] = peaks[i].GaussianContribution;
            constraints[gaussianContributionIndex] = GaussianContributionConstraint;
        }

        var startVector = new VectorN(startValues);

        var optimizationModel = new NedlerMeadModel
        {
            Start = startVector,
            Constraints = constraints,
            BufferSize = spectra.Points.Count,
            Settings = settings,
        };

        var (startValue, endValue) = GetBorders(startVector);

        var funcForMin = FittingFunctions.GetOptimizationFunc(
            SpectraFittingOptimizationFunction.ThroughR2,
            spectra,
            startValue,
            endValue);

        startVector = await NelderMead.GetOptimized(optimizationModel, funcForMin);

        UpdatePeaks(startVector);

        return;

        void UpdatePeaks(VectorN vector)
        {
            for (var i = 0; i < peaks.Count; i++)
            {
                peaks[i].Center = vector[PeakParametersCount * i];
                peaks[i].HalfWidth = vector[PeakParametersCount * i + 1];
                peaks[i].Amplitude = vector[PeakParametersCount * i + 2];
                peaks[i].GaussianContribution = vector[PeakParametersCount * i + 3];
            }
        }
    }

    private static (float StartValue, float EndValue) GetBorders(VectorN peaksVector)
    {
        var peaksCount = peaksVector.Dimension / PeakParametersCount;

        var startValue = float.MaxValue;
        var endValue = float.MinValue;

        for (var i = 0; i < peaksCount; i++)
        {
            var center = peaksVector[PeakParametersCount * i];
            var halfWidth = peaksVector[PeakParametersCount * i + 1];
            var gaussianContribution = peaksVector[PeakParametersCount * i + 3];

            var effectiveRadius = PeakModeling.GetPeakEffectiveRadius(
                halfWidth: halfWidth,
                gaussianContribution: gaussianContribution);

            startValue = Math.Min(startValue, center - effectiveRadius);
            endValue = Math.Max(endValue, center + effectiveRadius);
        }

        return (startValue, endValue);
    }
}
