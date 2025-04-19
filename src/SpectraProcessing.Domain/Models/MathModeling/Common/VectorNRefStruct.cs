using SpectraProcessing.Domain.Extensions;

namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public readonly ref struct VectorNRefStruct
{
    public readonly Span<float> Values;

    public int Dimension => Values.Length;

    public VectorNRefStruct(Span<float> buffer)
    {
        Values = buffer;
    }

    public VectorNRefStruct Add(in VectorNRefStruct other, float multiplier = 1)
    {
        if (multiplier.ApproximatelyEqual(1))
        {
            for (var d = 0; d < Dimension; d++)
            {
                Values[d] += other.Values[d];
            }
        }
        else
        {
            for (var d = 0; d < Dimension; d++)
            {
                Values[d] += multiplier * other.Values[d];
            }
        }


        return this;
    }

    public VectorNRefStruct Add(VectorN other, float multiplier = 1)
    {
        if (multiplier.ApproximatelyEqual(1))
        {
            for (var d = 0; d < Dimension; d++)
            {
                Values[d] += other.Values[d];
            }
        }
        else
        {
            for (var d = 0; d < Dimension; d++)
            {
                Values[d] += multiplier * other.Values[d];
            }
        }

        return this;
    }

    public VectorNRefStruct Multiply(float multiplier)
    {
        for (var d = 0; d < Dimension; d++)
        {
            Values[d] *= multiplier;
        }

        return this;
    }

    public VectorNRefStruct Divide(float divider)
    {
        for (var d = 0; d < Dimension; d++)
        {
            Values[d] /= divider;
        }

        return this;
    }

    public override string ToString()
    {
        return $"({string.Join(", ", Values.ToArray())})";
    }

    public static VectorNRefStruct Difference(
        in VectorNRefStruct left,
        in VectorNRefStruct right,
        in Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException("Vectors must have the same dimension.");
        }

        if (buffer.Length < left.Dimension)
        {
            throw new InvalidOperationException("Buffer too small.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer[..left.Dimension]);
    }

    public static VectorNRefStruct Difference(
        VectorN left,
        in VectorNRefStruct right,
        in Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException("Vectors must have the same dimension.");
        }

        if (buffer.Length < left.Dimension)
        {
            throw new InvalidOperationException("Buffer too small.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer[..left.Dimension]);
    }

    public static VectorNRefStruct Difference(
        VectorN left,
        VectorN right,
        in Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException("Vectors must have the same dimension.");
        }

        if (buffer.Length < left.Dimension)
        {
            throw new InvalidOperationException("Buffer too small.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer[..left.Dimension]);
    }

    public static VectorNRefStruct Difference(
        in VectorNRefStruct left,
        VectorN right,
        in Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException("Vectors must have the same dimension.");
        }

        if (buffer.Length < left.Dimension)
        {
            throw new InvalidOperationException("Buffer too small.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer[..left.Dimension]);
    }
}
