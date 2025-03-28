using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Domain.MathModeling;

public static class PeakModeling
{
    public static double GetPeakValueAt(this PeakData peak, double x)
    {
        return peak.GaussianContribution * Gaussian(x, peak)
            + (1 - peak.GaussianContribution) * Lorentzian(x, peak);

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

    public static double GetPeaksValueAt(this ICollection<PeakData> peaks, double x)
    {
        return peaks.Sum(p => p.GetPeakValueAt(x));
    }

    public static double GetPeakArea(this PeakData peak)
    {
        return peak.GaussianContribution * GaussianSquare(peak)
            + (1 - peak.GaussianContribution) * LorentzianSquare(peak);

        static double GaussianSquare(PeakData peak)
        {
            // constant = 2 * Math.Sqrt(Math.PI / Math.Log(2));
            const double constant = 4.2578680777249049;

            return constant * peak.Amplitude * peak.HalfWidth;
        }

        static double LorentzianSquare(PeakData peak)
        {
            const double constant = Math.PI / 2;

            return constant * peak.Amplitude * peak.HalfWidth;
        }
    }

    public static double GetPeakEffectiveRadius(this PeakData peak, double areaPercentage)
    {
        var gaussianWeight = GaussianAreaWeight(peak.GaussianContribution);

        return gaussianWeight * GaussianSquareRadius(peak, areaPercentage) +
            (1 - gaussianWeight) * LorentzianSquareRadius(peak, areaPercentage);

        static double GaussianAreaWeight(double gaussianContribution)
        {
            //gaussSquareDividedLorentzSquare = Math.Sqrt(Math.PI / Math.Log(2)) / Math.PI;
            const double gaussSquareDividedLorentzSquare = 0.67766075160310502;
            var gaussianArea = gaussianContribution * gaussSquareDividedLorentzSquare;
            return gaussianArea / (gaussianArea + (1 - gaussianContribution));
        }

        static double GaussianSquareRadius(PeakData peak, double areaPercentage)
        {
            // constant = 2 * Math.Sqrt(Math.Log(2));
            const double constant = 1.6651092223153954;
            return ErfInv(areaPercentage) * peak.HalfWidth / constant;
        }

        static double LorentzianSquareRadius(PeakData peak, double areaPercentage)
        {
            const double constant = Math.PI / 2;

            return peak.HalfWidth * Math.Tan(constant * areaPercentage);
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
