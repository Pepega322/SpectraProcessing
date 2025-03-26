using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.Extensions;

public static class VectorNExtensions
{
    public static VectorN Sum(this IReadOnlyCollection<VectorN> vectors)
    {
        var dimension = vectors.Select(x => x.Dimension).Distinct().Single();

        var values = new double[dimension];

        for (var d = 0; d < dimension; d++)
        {
            values[d] = vectors.Sum(v => v[d]);
        }

        return new VectorN(values);
    }

    public static VectorN Scalar(this VectorN left, VectorN right)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new ArgumentException("Dimensions don't match");
        }

        var values = new double[left.Dimension];

        for (var d = 0; d < left.Dimension; d++)
        {
            values[d] = left[d] * right[d];
        }

        return new VectorN(values);
    }

    public static VectorN Normalize(this VectorN vector)
    {
        var length = vector.Length();

        var values = vector.Values.Select(v => v / length).ToArray();

        return new VectorN(values);
    }

    public static double Length(this VectorN vector)
    {
        return Math.Sqrt(vector.Values.Sum(x => x * x));
    }
}
