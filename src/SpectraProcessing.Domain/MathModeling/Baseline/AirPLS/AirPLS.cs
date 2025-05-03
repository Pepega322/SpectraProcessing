using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling.Baseline;
using SpectraProcessing.Domain.Models.MathModeling.Common;

namespace SpectraProcessing.Domain.MathModeling.Baseline.AirPLS;

public static partial class AirPLS
{
    public static Task<float[]> GetBaseline(float[] yValues, AirPLSSettings settings)
    {
        return Task.Run(() => GetBaselineInternal(yValues, settings));
    }

    private static float[] GetBaselineInternal(float[] yValues, AirPLSSettings settings)
    {
        var length = yValues.Length;

        var baseline = new float[length];

        var yVector = new VectorN(yValues);
        var yVectorLength = yVector.GetLength();
        var weightVector = new VectorNRefStruct(length, stackalloc float[length].Set(_ => 1f));

        var penaltyMatrix = new PenaltyMatrix(length);
        Span<float> vectorBuffer = stackalloc float[length];
        Span<float> otherVectorBuffer = stackalloc float[length];
        Span<double> lMatrixBuffer = stackalloc double[3 * length - 3];

        for (var iteration = 0; iteration < settings.IterationsCount; iteration++)
        {
            //Этап 1 получаем текущий baseline
            var lMatrix = LMatrix.Create(weightVector, penaltyMatrix, settings.SmoothCoefficient, lMatrixBuffer);

            var weightedYVector = GetWeightedYVector(weightVector, vectorBuffer);

            var intermediateVector = GetIntermediateVector(weightedYVector, lMatrix, otherVectorBuffer);

            UpdateBaselineVector(intermediateVector, lMatrix);

            //Этап 2 находим отрицательные остатки (у - baseline)
            var negativeResidualsVectorLength = GetNegativeResidualsVectorLength();

            //Этап 3 проверяем прошёл ли критерий завершения
            if (negativeResidualsVectorLength < settings.SmoothingTolerance * yVectorLength)
            {
                break;
            }

            //Этап 4 перевешиваем
            for (var i = 0; i < length; i++)
            {
                var delta = yVector[i] - baseline[i];

                weightVector[i] += delta <= 0
                    ? MathF.Exp(iteration * delta / negativeResidualsVectorLength)
                    : 0;
            }
        }

        return baseline;

        VectorNRefStruct GetWeightedYVector(
            in VectorNRefStruct weight,
            in Span<float> buffer)
        {
            var weightedYValues = new VectorNRefStruct(length, buffer);

            for (var d = 0; d < length; d++)
            {
                buffer[d] = yVector[d] * weight[d];
            }

            return weightedYValues;
        }

        VectorNRefStruct GetIntermediateVector(
            in VectorNRefStruct weightedYVector,
            in LMatrix lMatrix,
            in Span<float> buffer)
        {
            var intermediateVector = new VectorNRefStruct(length, buffer);

            for (var i = 0; i < length; i++)
            {
                double y = weightedYVector[i];

                for (var j = i - 2; j < i; j++)
                {
                    if (j < 0)
                    {
                        continue;
                    }

                    y -= intermediateVector[j] * lMatrix[i, j];
                }

                intermediateVector[i] = (float) (y / lMatrix[i, i]);
            }

            return intermediateVector;
        }

        void UpdateBaselineVector(
            in VectorNRefStruct intermediateVector,
            in LMatrix lMatrix)
        {
            for (var i = length - 1; i >= 0; i--)
            {
                var b = intermediateVector[i];

                for (var j = i + 2; j > i; j--)
                {
                    if (j >= length)
                    {
                        continue;
                    }

                    b -= (float) (baseline[j] * lMatrix[j, i]);
                }

                baseline[i] = (float) (b / lMatrix[i, i]);
            }
        }

        float GetNegativeResidualsVectorLength()
        {
            var negativeResidualsVectorLength = 0f;

            for (var i = 0; i < length; i++)
            {
                var delta = yVector[i] - baseline[i];

                if (delta < 0)
                {
                    negativeResidualsVectorLength += delta * delta;
                }
            }

            negativeResidualsVectorLength = MathF.Sqrt(negativeResidualsVectorLength);

            return negativeResidualsVectorLength;
        }
    }
}
