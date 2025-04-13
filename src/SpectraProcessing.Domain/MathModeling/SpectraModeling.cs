using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling;

public static class SpectraModeling
{
    private const int peakParametersCount = 4;

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
            (group, _) => group.FitPeaksGroup(spectra, settings));

        // return Task.WhenAll(groups.Select(g => g.FitPeaksGroup(spectra, settings)));
    }

    private static List<List<PeakData>> GroupPeaksByEffectiveRadius(this IReadOnlyCollection<PeakData> peaks)
    {
        var effectiveRadius = peaks.ToDictionary(
            p => p,
            p => p.GetPeakEffectiveRadius());

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
        this List<PeakData> peaks,
        SpectraData spectra,
        OptimizationSettings settings)
    {
        var startValues = new float[peaks.Count * peakParametersCount];
        var constraints = new Dictionary<int, ValueConstraint>();

        for (var i = 0; i < peaks.Count; i++)
        {
            var center = peaks[i].Center;
            var halfWidth = peaks[i].HalfWidth;

            var centerIndex = peakParametersCount * i;
            startValues[centerIndex] = center;
            constraints[centerIndex] = new ValueConstraint(center - halfWidth, center + halfWidth);

            startValues[peakParametersCount * i + 1] = halfWidth;

            startValues[peakParametersCount * i + 2] = peaks[i].Amplitude;

            var gaussianContributionIndex = peakParametersCount * i + 3;
            startValues[gaussianContributionIndex] = peaks[i].GaussianContribution;
            constraints[gaussianContributionIndex] = new ValueConstraint(0, 1);
        }

        var startVector = new VectorN(startValues);

        var optimizationModel = new NedlerMeadOptimizationModel
        {
            Start = startVector,
            Constraints = constraints,
            BufferSize = spectra.Points.Count,
            Settings = settings,
        };

        var (startValue, endValue) = GetBorders(startVector);

        var optimizedVector = await NelderMead.GetOptimized(
            model: optimizationModel,
            funcForMin: GetOptimizationFuncThroughError(spectra, startValue, endValue)
            // funcForMin: GetOptimizationFuncThroughAICc(spectra, startValue, endValue)
            // funcForMin: GetOptimizationFuncThroughAIC(spectra, startValue, endValue)
            // funcForMin: GetOptimizationFuncThroughR2(spectra, startValue, endValue)
            // funcForMin: GetOptimizationFuncThroughS2(spectra, startValue, endValue)
        );

        UpdatePeaks(optimizedVector);

        return;

        static (float StartValue, float EndValue) GetBorders(VectorN peaksVector)
        {
            var peaksCount = peaksVector.Dimension / peakParametersCount;

            var startValue = float.MaxValue;
            var endValue = float.MinValue;

            for (var i = 0; i < peaksCount; i++)
            {
                var center = peaksVector.Values[peakParametersCount * i];
                var halfWidth = peaksVector.Values[peakParametersCount * i + 1];
                var gaussianContribution = peaksVector.Values[peakParametersCount * i + 3];

                var effectiveRadius = PeakModeling.GetPeakEffectiveRadius(
                    halfWidth: halfWidth,
                    gaussianContribution: gaussianContribution);

                startValue = Math.Min(startValue, center - effectiveRadius);
                endValue = Math.Max(endValue, center + effectiveRadius);
            }

            return (startValue, endValue);
        }

        void UpdatePeaks(VectorN vector)
        {
            for (var i = 0; i < peaks.Count; i++)
            {
                peaks[i].Center = vector.Values[peakParametersCount * i];
                peaks[i].HalfWidth = vector.Values[peakParametersCount * i + 1];
                peaks[i].Amplitude = vector.Values[peakParametersCount * i + 2];
                peaks[i].GaussianContribution = vector.Values[peakParametersCount * i + 3];
            }
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughR2(
        SpectraData spectra,
        float startValue,
        float endValue)
    {
        var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
        var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
        var length = endIndex - startIndex + 1;

        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            var deltas = buffer.Slice(startIndex, length);

            for (var i = 0; i < length; i++)
            {
                var pointIndex = startIndex + i;
                deltas[i] = spectra.Points.Y[pointIndex] - vector.GetPeaksValueAt(spectra.Points.X[pointIndex]);
            }

            var averageDelta = deltas.Sum() / length;
            var residualSumOfSquares = deltas.Sum(delta => delta * delta);

            for (var i = 0; i < length; i++)
            {
                deltas[i] = averageDelta - spectra.Points.Y[startIndex + i];
            }

            var totalSumOfSquares = deltas.Sum(delta => delta * delta);

            var rSquare = 1 - residualSumOfSquares / totalSumOfSquares;

            return 1 - rSquare;
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughS2(
        SpectraData spectra,
        float startValue,
        float endValue)
    {
        var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
        var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
        var length = endIndex - startIndex + 1;

        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            var deltas = buffer.Slice(startIndex, length);

            for (var i = 0; i < length; i++)
            {
                var pointIndex = startIndex + i;
                deltas[i] = spectra.Points.Y[pointIndex] - vector.GetPeaksValueAt(spectra.Points.X[pointIndex]);
            }

            var averageDelta = deltas.Sum() / length;

            for (var i = 0; i < length; i++)
            {
                deltas[i] -= averageDelta;
            }

            var deltaDeltaSumSquares = deltas.Sum(delta => delta * delta);

            var standardDeviationSquare = deltaDeltaSumSquares / (length - 1);

            return standardDeviationSquare;
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughAIC(
        SpectraData spectra,
        float startValue,
        float endValue)
    {
        var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
        var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
        var length = endIndex - startIndex + 1;

        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            var deltas = buffer.Slice(startIndex, length);

            for (var i = 0; i < length; i++)
            {
                var pointIndex = startIndex + i;
                deltas[i] = spectra.Points.Y[pointIndex] - vector.GetPeaksValueAt(spectra.Points.X[pointIndex]);
            }

            var sumOfResidualsSquares = deltas.Sum(delta => delta * delta);

            return GetAic(
                sumOfResidualsSquares: sumOfResidualsSquares,
                valuesCount: length,
                paramsCount: vector.Dimension);
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughAICc(
        SpectraData spectra,
        float startValue,
        float endValue)
    {
        var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
        var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
        var length = endIndex - startIndex + 1;

        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            var deltas = buffer.Slice(startIndex, length);

            for (var i = 0; i < length; i++)
            {
                var pointIndex = startIndex + i;
                deltas[i] = spectra.Points.Y[pointIndex] - vector.GetPeaksValueAt(spectra.Points.X[pointIndex]);
            }

            var sumOfResidualsSquares = deltas.Sum(delta => delta * delta);

            var paramCount = vector.Dimension;

            var aic = GetAic(
                sumOfResidualsSquares: sumOfResidualsSquares,
                valuesCount: length,
                paramsCount: paramCount);

            var aicC = aic + 2f * paramCount * (paramCount - 1) / (length - paramCount - 1);

            return aicC;
        }
    }

    private static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFuncThroughError(
        SpectraData spectra,
        float startValue,
        float endValue)
    {
        var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
        var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
        var length = endIndex - startIndex + 1;

        return OptimizationFunc;

        float OptimizationFunc(VectorNRefStruct vector, Span<float> buffer)
        {
            var deltas = buffer.Slice(startIndex, length);

            for (var i = 0; i < length; i++)
            {
                var pointIndex = startIndex + i;
                deltas[i] = spectra.Points.Y[pointIndex] - vector.GetPeaksValueAt(spectra.Points.X[pointIndex]);
            }

            var optimizationFunc = deltas.Sum(Math.Abs) / length;
            return optimizationFunc;
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

    private static float GetAic(
        float sumOfResidualsSquares,
        int valuesCount,
        int paramsCount)
    {
        // var const1 = Math.Log(2 * Math.PI) + 1;
        const double const1 = 2.8378770664093453f;

        var lnL = 0.5f * -valuesCount * (const1 - Math.Log(valuesCount) + Math.Log(sumOfResidualsSquares));

        var aic = 2 * paramsCount - 2 * lnL;

        return (float) aic;
    }
}
