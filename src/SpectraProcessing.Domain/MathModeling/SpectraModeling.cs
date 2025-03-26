using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Domain.MathModeling;

public static class SpectraModeling
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

    private static double GaussianAndLorentzianMixSquareRadius(double squarePercentage, PeakData estimate)
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

            return ArcErl(squarePercentage) * estimate.HalfWidth / constant;
        }

        static double LorentzianSquareRadius(double squarePercentage, PeakData estimate)
        {
            const double constant = Math.PI / 2;

            return estimate.HalfWidth * Math.Tan(constant * squarePercentage);
        }
    }

    private static double ArcErl(double x)
    {
        // constant0 = Math.Sqrt(Math.PI) / 2;
        const double constant0 = 0.88622692545275794;
        const double constant1 = constant0 * 0.26179938779914941;
        const double constant2 = constant0 * 0.14393173084921979;
        const double constant3 = constant0 * 0.097663619503920551;
        const double constant4 = constant0 * 0.073299079366380859;
        const double constant5 = constant0 * 0.058372500878584518;

        Span<double> coefficients =
        [
            constant0,
            constant1,
            constant2,
            constant3,
            constant4,
            constant5,
        ];

        double result = 0;

        var pow = 1;

        foreach (var c in coefficients)
        {
            result += c * Math.Pow(x, pow);
            pow += 2;
        }

        return result;
    }

    private static double Erl(double x)
    {
        // constant0 = 2 / Math.Sqrt(Math.PI);
        const double constant0 = 1.1283791670955126;
        const double constant1 = -constant0 / 3;
        const double constant2 = constant0 / 10;
        const double constant3 = -constant0 / 42;
        const double constant4 = constant0 / 216;
        const double constant5 = -constant0 / 1320;

        Span<double> coefficients =
        [
            constant0,
            constant1,
            constant2,
            constant3,
            constant4,
            constant5,
        ];

        double result = 0;

        var pow = 1;

        foreach (var c in coefficients)
        {
            result += c * Math.Pow(x, pow);
            pow += 2;
        }

        return result;
    }
}
