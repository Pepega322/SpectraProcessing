using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Math;

public static class SpectraModeling
{
    public static double GaussianAndLorentzianMix(double x, PeakData estimate)
        => estimate.GaussianContribution * Gaussian(x, estimate)
            + (1 - estimate.GaussianContribution) * Lorentzian(x, estimate);

    public static double GaussianAndLorentzianMix(double x, IReadOnlyCollection<PeakData> estimates)
        => estimates.Sum(e => GaussianAndLorentzianMix(x, e));

    private static double Gaussian(double x, PeakData estimate)
    {
        var a = -4 * System.Math.Log(2);

        var b = x - estimate.Center;

        var c = estimate.HalfWidth * estimate.HalfWidth;

        return estimate.Amplitude * System.Math.Exp(a * b * b / c);
    }

    private static double Lorentzian(double x, PeakData estimate)
    {
        var a = 2 * (x - estimate.Center) / estimate.HalfWidth;

        return estimate.Amplitude / (1 + a * a);
    }
}
