using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Domain.MathModeling.Peaks;

public static class PeakModeling
{
    public static float GetPeakValueAt(this IReadOnlyPeakData peak, float x)
        => GetPeakValueAt(
            x: x,
            center: peak.Center,
            halfWidth: peak.HalfWidth,
            amplitude: peak.Amplitude,
            gaussianContribution: peak.GaussianContribution);

    public static float GetPeakValueAt(
        float x,
        float center,
        float halfWidth,
        float amplitude,
        float gaussianContribution)
    {
        return gaussianContribution * Gaussian() + (1 - gaussianContribution) * Lorentzian();

        float Gaussian()
        {
            // constant = -4 * Math.Log(2)
            const float constant = -2.7725887222397811f;

            var b = x - center;

            var c = halfWidth * halfWidth;

            return amplitude * (float) Math.Exp(constant * b * b / c);
        }

        float Lorentzian()
        {
            var a = 2 * (x - center) / halfWidth;

            return amplitude / (1 + a * a);
        }
    }

    public static float GetPeaksValueAt(this IEnumerable<IReadOnlyPeakData> peaks, float x)
        => peaks.Sum(p => p.GetPeakValueAt(x));

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

    public static float GetPeakEffectiveRadius(this IReadOnlyPeakData peak)
        => GetPeakEffectiveRadius(peak.HalfWidth, peak.GaussianContribution);

    public static float GetPeakEffectiveRadius(float halfWidth, float gaussianContribution)
    {
        var gaussianWeight = GaussianAreaWeight();

        return gaussianWeight * GaussianSquareRadius(0.9995f) + (1 - gaussianWeight) * LorentzianSquareRadius(0.75f);

        float GaussianAreaWeight()
        {
            //gaussSquareDividedLorentzSquare = Math.Sqrt(Math.PI / Math.Log(2)) / Math.PI;
            const float gaussSquareDividedLorentzSquare = 0.67766075160310502f;
            var gaussianArea = gaussianContribution * gaussSquareDividedLorentzSquare;
            return gaussianArea / (gaussianArea + (1 - gaussianContribution));
        }

        float GaussianSquareRadius(float areaPercentage)
        {
            // constant = 2 * Math.Sqrt(Math.Log(2));
            const float constant = 1.6651092223153954f;
            return ErfInv(areaPercentage) * halfWidth / constant;
        }

        float LorentzianSquareRadius(float areaPercentage)
        {
            const float constant = (float) Math.PI / 2;

            return halfWidth * (float) Math.Tan(constant * areaPercentage);
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
