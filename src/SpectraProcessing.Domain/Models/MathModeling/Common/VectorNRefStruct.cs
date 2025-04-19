using SpectraProcessing.Domain.Extensions;

namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public readonly ref struct VectorNRefStruct
{
    private readonly Span<float> values;

    public int Dimension => values.Length;

    public float this[int index]
    {
        get => values[index];
        set => values[index] = value;
    }

    public VectorNRefStruct(Span<float> buffer)
    {
        values = buffer;
    }

    public VectorNRefStruct Add(in VectorNRefStruct other, float multiplier = 1)
    {
        if (multiplier.ApproximatelyEqual(1))
        {
            for (var d = 0; d < Dimension; d++)
            {
                values[d] += other.values[d];
            }
        }
        else
        {
            for (var d = 0; d < Dimension; d++)
            {
                values[d] += multiplier * other.values[d];
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
                values[d] += other[d];
            }
        }
        else
        {
            for (var d = 0; d < Dimension; d++)
            {
                values[d] += multiplier * other[d];
            }
        }

        return this;
    }

    public VectorNRefStruct Multiply(float multiplier)
    {
        for (var d = 0; d < Dimension; d++)
        {
            values[d] *= multiplier;
        }

        return this;
    }

    public VectorNRefStruct Divide(float divider)
    {
        for (var d = 0; d < Dimension; d++)
        {
            values[d] /= divider;
        }

        return this;
    }

    public override string ToString()
    {
        return $"({string.Join(", ", values.ToArray())})";
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
            buffer[d] = left.values[d] - right.values[d];
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
            buffer[d] = left[d] - right.values[d];
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
            buffer[d] = left[d] - right[d];
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
            buffer[d] = left[d] - right[d];
        }

        return new VectorNRefStruct(buffer[..left.Dimension]);
    }
}
