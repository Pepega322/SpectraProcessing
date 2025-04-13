using SpectraProcessing.Domain.Enums;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling;

public static class FittingFunctions
{
    public static Func<VectorNRefStruct, Span<float>, float> GetOptimizationFunc(
        SpectraFittingOptimizationFunction spectraFittingOptimizationFunction,
        SpectraData spectra,
        float startValue,
        float endValue)
    {
        var startIndex = spectra.Points.X.ClosestIndexBinarySearch(startValue);
        var endIndex = spectra.Points.X.ClosestIndexBinarySearch(endValue);
        var length = endIndex - startIndex + 1;


        return spectraFittingOptimizationFunction switch
        {
            SpectraFittingOptimizationFunction.ThroughR2    => ThroughR2,
            SpectraFittingOptimizationFunction.ThroughS2    => ThroughS2,
            SpectraFittingOptimizationFunction.ThroughAIC   => ThroughAIC,
            SpectraFittingOptimizationFunction.ThroughAICc  => ThroughAICc,
            SpectraFittingOptimizationFunction.ThroughError => ThroughError,
            _                                               => throw new ArgumentOutOfRangeException(),
        };

        float ThroughR2(VectorNRefStruct vector, Span<float> buffer)
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

        float ThroughS2(VectorNRefStruct vector, Span<float> buffer)
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

        float ThroughAIC(VectorNRefStruct vector, Span<float> buffer)
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

        float ThroughAICc(VectorNRefStruct vector, Span<float> buffer)
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

        float ThroughError(VectorNRefStruct vector, Span<float> buffer)
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
        var peaksCount = peaksVector.Dimension / SpectraModeling.PeakParametersCount;

        var value = 0f;

        for (var i = 0; i < peaksCount; i++)
        {
            var center = peaksVector.Values[SpectraModeling.PeakParametersCount * i];
            var halfWidth = peaksVector.Values[SpectraModeling.PeakParametersCount * i + 1];
            var amplitude = peaksVector.Values[SpectraModeling.PeakParametersCount * i + 2];
            var gaussianContribution = peaksVector.Values[SpectraModeling.PeakParametersCount * i + 3];

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
