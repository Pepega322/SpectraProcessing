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

        return Parallel.ForEachAsync(
            groups,
            (group, ct) => group.FitPeaksGroup(spectra, settings));

        // return Task.WhenAll(groups.Select(g => g.FitPeaksGroup(spectra, settings)));
    }

    private static List<List<PeakData>> GroupPeaksByEffectiveRadius(this IReadOnlyCollection<PeakData> peaks)
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

    private static async ValueTask FitPeaksGroup(
        this List<PeakData> group,
        SpectraData spectra,
        OptimizationSettings settings)
    {
        var startValues = new float[group.Count * peakParametersCount];

        for (var i = 0; i < group.Count; i++)
        {
            var peak = group[i];
            startValues[peakParametersCount * i] = peak.Center;
            startValues[peakParametersCount * i + 1] = peak.HalfWidth;
            startValues[peakParametersCount * i + 2] = peak.Amplitude;
            // startValues[peakParametersCount * i + 3] = group[i].GaussianContribution;
        }

        var optimizedVector = await NelderMead.GetOptimized(
            start: new VectorN(startValues),
            funcForMin: GetOptimizationFuncThroughAICc(spectra),
            // funcForMin: GetOptimizationFuncThroughAIC(spectra),
            // funcForMin: GetOptimizationFuncThroughR2(spectra),
            // funcForMin: GetOptimizationFuncThroughS2(spectra),
            bufferSize: spectra.Points.Count,
            settings: settings);

        UpdatePeaks(optimizedVector);

        return;

        void UpdatePeaks(VectorN vector)
        {
            for (var i = 0; i < group.Count; i++)
            {
                var peak = group[i];
                peak.Center = vector.Values[peakParametersCount * i];
                peak.HalfWidth = vector.Values[peakParametersCount * i + 1];
                peak.Amplitude = vector.Values[peakParametersCount * i + 2];
                // peak.GaussianContribution = (float) vector.Values[peakParametersCount * i + 3];
            }
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughR2(SpectraData spectra)
    {
        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
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

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughS2(SpectraData spectra)
    {
        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            buffer.Clear();

            var (startValue, endValue) = vector.GetBorders();
            var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
            var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);

            var length = endIndex - startIndex;

            var averageDelta = 0f;
            for (var i = startIndex; i < endIndex; i++)
            {
                buffer[i] = spectra.Points.Y[i] - vector.GetPeaksValueAt(spectra.Points.X[i]);
                averageDelta += buffer[i];
            }

            averageDelta /= length;

            var deltasDeltas = 0f;
            for (var i = startIndex; i < endIndex; i++)
            {
                var deltaDelta = buffer[i] - averageDelta;
                deltasDeltas += deltaDelta * deltaDelta;
            }

            var standardDeviationSquare = deltasDeltas / (length - 1);

            return standardDeviationSquare;
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughAIC(SpectraData spectra)
    {
        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            buffer.Clear();

            var (startValue, endValue) = vector.GetBorders();
            var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
            var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
            var n = endIndex - startIndex;

            var sumOfResidualsSquares = 0f;
            for (var i = startIndex; i < endIndex; i++)
            {
                var delta = spectra.Points.Y[i] - vector.GetPeaksValueAt(spectra.Points.X[i]);
                sumOfResidualsSquares += delta * delta;
            }

            var lnL = 0.5f * -n * (float) (Math.Log(2 * Math.PI) + 1 - Math.Log(n) + Math.Log(sumOfResidualsSquares));

            var aic = 2 * vector.Dimension - 2 * lnL;

            return aic;
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughAICc(SpectraData spectra)
    {
        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            buffer.Clear();

            var (startValue, endValue) = vector.GetBorders();
            var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
            var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
            var n = endIndex - startIndex;

            var sumOfResidualsSquares = 0f;
            for (var i = startIndex; i < endIndex; i++)
            {
                var delta = spectra.Points.Y[i] - vector.GetPeaksValueAt(spectra.Points.X[i]);
                sumOfResidualsSquares += delta * delta;
            }

            var lnL = 0.5f * -n * (float) (Math.Log(2 * Math.PI) + 1 - Math.Log(n) + Math.Log(sumOfResidualsSquares));

            var paramCount = vector.Dimension;

            var aic = 2 * paramCount - 2 * lnL;

            var aicC = aic + 2 * paramCount * (paramCount - 1) / (n - paramCount - 1f);

            return aicC;
        }
    }

    private static float GetPeaksValueAt(this in VectorNRefStruct peaksVector, float x)
    {
        var peaksCount = peaksVector.Dimension / peakParametersCount;

        var value = 0f;

        for (var i = 0; i < peaksCount; i++)
        {
            var center = peaksVector.Values[peakParametersCount * i];
            var halfWidth = peaksVector.Values[peakParametersCount * i + 1];
            var amplitude = peaksVector.Values[peakParametersCount * i + 2];
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
            var halfWidth = peaksVector.Values[peakParametersCount * i + 1];
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
