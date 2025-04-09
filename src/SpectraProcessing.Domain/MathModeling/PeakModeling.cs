using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Domain.MathModeling;

public static class PeakModeling
{
    public static float GetPeakValueAt(this IReadOnlyPeakData peak, float x)
    {
        return peak.GaussianContribution * Gaussian(x, peak)
            + (1 - peak.GaussianContribution) * Lorentzian(x, peak);

        static float Gaussian(float x, IReadOnlyPeakData estimate)
        {
            // constant = -4 * Math.Log(2)
            const float constant = -2.7725887222397811f;

            var b = x - estimate.Center;

            var c = estimate.HalfWidth * estimate.HalfWidth;

            return estimate.Amplitude * (float) Math.Exp(constant * b * b / c);
        }

        static float Lorentzian(float x, IReadOnlyPeakData estimate)
        {
            var a = 2 * (x - estimate.Center) / estimate.HalfWidth;

            return estimate.Amplitude / (1 + a * a);
        }
    }

    public static float GetPeaksValueAt(this IEnumerable<IReadOnlyPeakData> peaks, float x)
    {
        return peaks.Sum(p => p.GetPeakValueAt(x));
    }

    public static float GetPeakArea(this IReadOnlyPeakData peak)
    {
        return peak.GaussianContribution * GaussianSquare(peak)
            + (1 - peak.GaussianContribution) * LorentzianSquare(peak);

        static float GaussianSquare(IReadOnlyPeakData peak)
        {
            // constant = 2 * Math.Sqrt(Math.PI / Math.Log(2));
            const float constant = 4.2578680777249049f;

            return constant * peak.Amplitude * peak.HalfWidth;
        }

        static float LorentzianSquare(IReadOnlyPeakData peak)
        {
            const float constant = (float) Math.PI / 2;

            return constant * peak.Amplitude * peak.HalfWidth;
        }
    }

    public static float GetPeakEffectiveRadius(this IReadOnlyPeakData peak, float areaPercentage)
    {
        var gaussianWeight = GaussianAreaWeight(peak.GaussianContribution);

        return gaussianWeight * GaussianSquareRadius(peak, areaPercentage) +
            (1 - gaussianWeight) * LorentzianSquareRadius(peak, areaPercentage);

        static float GaussianAreaWeight(float gaussianContribution)
        {
            //gaussSquareDividedLorentzSquare = Math.Sqrt(Math.PI / Math.Log(2)) / Math.PI;
            const float gaussSquareDividedLorentzSquare = 0.67766075160310502f;
            var gaussianArea = gaussianContribution * gaussSquareDividedLorentzSquare;
            return gaussianArea / (gaussianArea + (1 - gaussianContribution));
        }

        static float GaussianSquareRadius(IReadOnlyPeakData peak, float areaPercentage)
        {
            // constant = 2 * Math.Sqrt(Math.Log(2));
            const float constant = 1.6651092223153954f;
            return ErfInv(areaPercentage) * peak.HalfWidth / constant;
        }

        static float LorentzianSquareRadius(IReadOnlyPeakData peak, float areaPercentage)
        {
            const float constant = (float) Math.PI / 2;

            return peak.HalfWidth * (float) Math.Tan(constant * areaPercentage);
        }
    }

    private static float Erf(float x)
    {
        x = Math.Abs(x);

        var t = 1.0 / (1.0 + 0.3275911 * x);

        var y = 1.0 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) *
            t - 0.284496736) * t + 0.254829592) * t * Math.Exp(-x * x);

        return Math.Sign(x) * (float) y;
    }

    private static float ErfInv(float x)
    {
        if (x < -1 || x > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "Input must be in [-1, 1]");
        }

        if (x == 0)
        {
            return 0;
        }

        const float a = 0.147f;
        var ln = Math.Log(1 - x * x);
        var part1 = (2 / (Math.PI * a)) + (ln / 2);
        var part2 = ln / a;

        return Math.Sign(x) * (float) Math.Sqrt(Math.Sqrt(part1 * part1 - part2) - part1);
    }
}
