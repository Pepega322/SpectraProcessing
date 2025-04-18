namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public readonly ref struct VectorNRefStruct
{
    public readonly Span<float> Values;

    public int Dimension => Values.Length;

    public VectorNRefStruct(Span<float> values)
    {
        Values = values;
    }

    public VectorNRefStruct Add(in VectorNRefStruct other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            Values[d] += other.Values[d];
        }

        return this;
    }

    public VectorNRefStruct Add(VectorN other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            Values[d] += other.Values[d];
        }

        return this;
    }

    public VectorNRefStruct Substract(in VectorNRefStruct other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            Values[d] -= other.Values[d];
        }

        return this;
    }

    public VectorNRefStruct Substract(VectorN other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            Values[d] -= other.Values[d];
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

    public static VectorNRefStruct Difference(in VectorNRefStruct left, in VectorNRefStruct right, Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException($"Vectors must have the same dimension.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer);
    }

    public static VectorNRefStruct Difference(VectorN left, in VectorNRefStruct right, Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException($"Vectors must have the same dimension.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer);
    }

    public static VectorNRefStruct Difference(VectorN left, VectorN right, Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException($"Vectors must have the same dimension.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer);
    }

    public static VectorNRefStruct Difference(in VectorNRefStruct left, VectorN right, Span<float> buffer)
    {
        if (left.Dimension != right.Dimension)
        {
            throw new InvalidOperationException($"Vectors must have the same dimension.");
        }

        for (var d = 0; d < left.Dimension; d++)
        {
            buffer[d] = left.Values[d] - right.Values[d];
        }

        return new VectorNRefStruct(buffer);
    }
}
