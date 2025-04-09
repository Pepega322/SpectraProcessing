namespace SpectraProcessing.Domain.Models.MathModeling;

public readonly ref struct VectorNRefStruct
{
    private readonly Span<float> values;

    public ReadOnlySpan<float> Values => values;

    public int Dimension => values.Length;

    public VectorNRefStruct(Span<float> values)
    {
        this.values = values;
    }

    public VectorNRefStruct Add(in VectorNRefStruct other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            values[d] += other.Values[d];
        }

        return this;
    }

    public VectorNRefStruct Add(VectorN other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            values[d] += other.Values[d];
        }

        return this;
    }

    public VectorNRefStruct Substract(in VectorNRefStruct other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            values[d] -= other.Values[d];
        }

        return this;
    }

    public VectorNRefStruct Substract(VectorN other)
    {
        for (var d = 0; d < Dimension; d++)
        {
            values[d] -= other.Values[d];
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
