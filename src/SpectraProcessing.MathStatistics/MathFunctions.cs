using SpectraProcessing.Models.PeakEstimate;

namespace SpectraProcessing.MathStatistics;

public static class MathFunctions
{
    public static double GaussianAndLorentzianMix(float x, PeakEstimateData estimate)
        => estimate.GaussianContribution * Gaussian(x, estimate)
            + (1 - estimate.GaussianContribution) * Lorentzian(x, estimate);

    private static double Gaussian(float x, PeakEstimateData estimate)
    {
        var a = -4 * Math.Log(2);

        var b = x - estimate.Center;

        var c = estimate.HalfWidth * estimate.HalfWidth;

        return estimate.Amplitude * Math.Exp(a * b * b / c);
    }

    private static double Lorentzian(float x, PeakEstimateData estimate)
    {
        var a = 2 * (x - estimate.Center) / estimate.HalfWidth;

        return estimate.Amplitude / (1 + a * a);
    }
}
