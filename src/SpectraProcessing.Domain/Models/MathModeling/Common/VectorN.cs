namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public sealed class VectorN
{
    private readonly float[] values;

    public VectorN(int dimension)
    {
        values = new float[dimension];
    }

    public VectorN(float[] values)
    {
        this.values = values;
    }

    public int Dimension => values.Length;

    public float this[int index]
    {
        get => values[index];
        set => values[index] = value;
    }

    public VectorNRefStruct ToVectorNRefStruct(in Span<float> buffer)
    {
        for (var d = 0; d < Dimension; d++)
        {
            buffer[d] = values[d];
        }

        return new VectorNRefStruct(buffer);
    }

    public void Update(in VectorNRefStruct vector)
    {
        if (vector.Dimension != Dimension)
        {
            throw new ArgumentException("Dimensions don't match");
        }

        for (var d = 0; d < Dimension; d++)
        {
            values[d] = vector[d];
        }
    }

    public void Update(in VectorN vector)
    {
        if (vector.Dimension != Dimension)
        {
            throw new ArgumentException("Dimensions don't match");
        }

        for (var d = 0; d < Dimension; d++)
        {
            values[d] = vector[d];
        }
    }

    public override string ToString() => $"({string.Join(", ", values.ToArray())})";
}
