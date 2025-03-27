using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Domain.MathModeling;

public static class PeakModeling
{
    public static double GaussianAndLorentzianMix(double x, PeakData estimate)
    {
        return estimate.GaussianContribution * Gaussian(x, estimate)
            + (1 - estimate.GaussianContribution) * Lorentzian(x, estimate);

        static double Gaussian(double x, PeakData estimate)
        {
            // constant = -4 * Math.Log(2)
            const double constant = -2.7725887222397811;

            var b = x - estimate.Center;

            var c = estimate.HalfWidth * estimate.HalfWidth;

            return estimate.Amplitude * Math.Exp(constant * b * b / c);
        }

        static double Lorentzian(double x, PeakData estimate)
        {
            var a = 2 * (x - estimate.Center) / estimate.HalfWidth;

            return estimate.Amplitude / (1 + a * a);
        }
    }

    public static double GaussianAndLorentzianMix(double x, ICollection<PeakData> estimates)
    {
        return estimates.Sum(e => GaussianAndLorentzianMix(x, e));
    }

    public static double GaussianAndLorentzianMixSquare(PeakData estimate)
    {
        return estimate.GaussianContribution * GaussianSquare(estimate)
            + (1 - estimate.GaussianContribution) * LorentzianSquare(estimate);

        static double GaussianSquare(PeakData estimate)
        {
            // constant = 2 * Math.Sqrt(Math.PI / Math.Log(2));
            const double constant = 4.2578680777249049;

            return constant * estimate.Amplitude * estimate.HalfWidth;
        }

        static double LorentzianSquare(PeakData estimate)
        {
            const double constant = Math.PI / 2;

            return constant * estimate.Amplitude * estimate.HalfWidth;
        }
    }

    public static double GaussianAndLorentzianMixSquareRadius(double squarePercentage, PeakData estimate)
    {
        var gaussianWeight = GaussianSquareWeight(estimate.GaussianContribution);

        return gaussianWeight * GaussianSquareRadius(squarePercentage, estimate) +
            (1 - gaussianWeight) * LorentzianSquareRadius(squarePercentage, estimate);

        static double GaussianSquareWeight(double gaussianContribution)
        {
            //gaussSquareDividedLorentzSquare = Math.Sqrt(Math.PI / Math.Log(2)) / Math.PI;
            const double gaussSquareDividedLorentzSquare = 0.67766075160310502;
            var gaussianSquare = gaussianContribution * gaussSquareDividedLorentzSquare;
            return gaussianSquare / (gaussianSquare + (1 - gaussianContribution));
        }

        static double GaussianSquareRadius(double squarePercentage, PeakData estimate)
        {
            // constant = 2 * Math.Sqrt(Math.Log(2));
            const double constant = 1.6651092223153954;
            return ErfInv(squarePercentage) * estimate.HalfWidth / constant;
        }

        static double LorentzianSquareRadius(double squarePercentage, PeakData estimate)
        {
            const double constant = Math.PI / 2;

            return estimate.HalfWidth * Math.Tan(constant * squarePercentage);
        }
    }

    private static double Erf(double x)
    {
        x = Math.Abs(x);

        var t = 1.0 / (1.0 + 0.3275911 * x);

        var y = 1.0 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) *
            t - 0.284496736) * t + 0.254829592) * t * Math.Exp(-x * x);

        return Math.Sign(x) * y;
    }

    private static double ErfInv(double x)
    {
        if (x < -1 || x > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "Input must be in [-1, 1]");
        }

        if (x == 0)
        {
            return 0;
        }

        const double a = 0.147;
        var ln = Math.Log(1 - x * x);
        var part1 = (2 / (Math.PI * a)) + (ln / 2);
        var part2 = ln / a;

        return Math.Sign(x) * Math.Sqrt(Math.Sqrt(part1 * part1 - part2) - part1);
    }
}
