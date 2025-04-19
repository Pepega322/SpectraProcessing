using SpectraProcessing.Domain.Models.MathModeling.Baseline;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling.Baseline;

public static class AirPLS
{
    public static Task CorrectBaseline(SpectraData spectra, AirPLSBaselineCorrectionModel model)
    {
        return Task.Run(() => CorrectBaselineInternal(spectra, model));
    }

    private static void CorrectBaselineInternal(SpectraData spectra, AirPLSBaselineCorrectionModel model)
    {
        // var yPoints = spectra.Points.Y;
        // Span<float> weights = stackalloc float[yPoints.Length];
        // for (var i = 0; i < yPoints.Length; i++)
        // {
        //     weights[i] = 1f;
        // }
        //
        // var iteration = 0;
        // while (iteration < model.MaxIterationsCount)
        // {
        //
        //
        //
        //     iteration++;
        // }
        //
        //
        // throw new NotImplementedException();
        var baseline = AirPLSGpt(spectra.Points.Y, 50, 1e6f);

        for (int i = 0; i < spectra.Points.Y.Length; i++)
        {
            spectra.Points.Y[i] -= baseline[i];
        }


    }

    public static float[] AirPLSGpt(float[] yValues, int maxIterationsCount, float lambda)
    {
        int n = yValues.Length;
        float[] weights = new float[n];
        float[] baseline = new float[n];
        float[] z = new float[n];
        float[] residual = new float[n];

        // Инициализируем веса
        for (int i = 0; i < n; i++)
            weights[i] = 1f;

        float sumY = 0f;
        for (int i = 0; i < n; i++)
            sumY += Math.Abs(yValues[i]);

        for (int iter = 0; iter < maxIterationsCount; iter++)
        {
            // Сглаживание
            for (int smoothIter = 0; smoothIter < 10; smoothIter++)
            {
                Array.Copy(baseline, z, n);
                for (int i = 1; i < n - 1; i++)
                {
                    baseline[i] = (weights[i] * yValues[i] + lambda * (z[i - 1] + z[i + 1])) /
                        (weights[i] + 2 * lambda);
                }

                baseline[0] = baseline[1];
                baseline[n - 1] = baseline[n - 2];
            }

            // Вычисляем остатки и обновляем веса
            float sumNegativeResiduals = 0f;
            for (int i = 0; i < n; i++)
            {
                residual[i] = yValues[i] - baseline[i];
                if (residual[i] < 0)
                    sumNegativeResiduals += Math.Abs(residual[i]);
            }

            if (sumNegativeResiduals < 0.001f * sumY)
                break;

            for (int i = 0; i < n; i++)
            {
                if (residual[i] >= 0)
                    weights[i] = 0f;
                else
                    weights[i] = (float) Math.Exp(residual[i] / sumNegativeResiduals);
            }
        }

        return baseline;
    }
}
