using System.Text;
using SpectraProcessing.Domain.Extensions;

namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed class VectorN
{
    private readonly double[] values;

    public IReadOnlyList<double> Values => values;

    public int Dimension => values.Length;

    public double this[int index]
    {
        get => values[index];
        set => values[index] = value;
    }

    public VectorN(IReadOnlyList<double> values)
    {
        this.values = values.ToArray();
    }

    public override bool Equals(object? obj) => obj is VectorN vectorN && vectorN == this;

    public override int GetHashCode() => values.GetHashCode();

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append($"Dimension: {Dimension}, ");
        builder.Append("Values: {");

        for (var d = 0; d < Dimension; d++)
        {
            builder.Append(values[d].ToString("0.##"));
            if (d < Dimension - 1)
            {
                builder.Append(", ");
            }
        }

        builder.Append("}");

        return builder.ToString();
    }

#region Operators

    public static VectorN operator +(VectorN left, VectorN right)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new ArgumentException("Dimensions don't match");
        }

        var values = new double[left.Dimension];

        for (var d = 0; d < left.Dimension; d++)
        {
            values[d] = left.Values[d] + right.Values[d];
        }

        return new VectorN(values);
    }

    public static VectorN operator -(VectorN left, VectorN right)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new ArgumentException("Dimensions don't match");
        }

        var values = new double[left.Dimension];

        for (var d = 0; d < left.Dimension; d++)
        {
            values[d] = left.Values[d] - right.Values[d];
        }

        return new VectorN(values);
    }

    public static VectorN operator *(VectorN v, double multiplier)
    {
        var values = new double[v.Dimension];

        for (var d = 0; d < v.Dimension; d++)
        {
            values[d] = v.Values[d] * multiplier;
        }

        return new VectorN(values);
    }

    public static VectorN operator *(double multiplier, VectorN v)
    {
        return v * multiplier;
    }

    public static VectorN operator /(VectorN v, double divider)
    {
        return v * (1 / divider);
    }

    public static bool operator ==(VectorN left, VectorN right)
    {
        if (left.Dimension != right.Dimension)
        {
            return false;
        }

        const double delta = 1e-6;
        for (var d = 0; d < left.Dimension; d++)
        {
            if (!left.Values[d].ApproximatelyEqual(right.Values[d], delta))
            {
                return false;
            }
        }

        return true;
    }

    public static bool operator !=(VectorN left, VectorN right)
    {
        return !(left == right);
    }

#endregion
}
