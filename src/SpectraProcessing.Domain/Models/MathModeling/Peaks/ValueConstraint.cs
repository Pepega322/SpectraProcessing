using SpectraProcessing.Domain.Extensions;

namespace SpectraProcessing.Domain.Models.MathModeling.Peaks;

public sealed record ValueConstraint
{
    private readonly float min;
    private readonly float max;

    public ValueConstraint(float min, float max)
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }

        if (min.ApproximatelyEqual(max, 1e-2f))
        {
            throw new Exception("Invalid value constraint");
        }

        this.min = min;
        this.max = max;
    }

    public float WithReflection(float value)
    {
        while (true)
        {
            if (value < min)
            {
                value = 2 * min - value;
                continue;
            }

            if (value > max)
            {
                value = 2 * max - value;
                continue;
            }

            return value;
        }
    }

    public float ApplyWithCut(float value)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}
