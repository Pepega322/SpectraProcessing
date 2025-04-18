namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public sealed class VectorN
{
    public readonly float[] Values;

    public VectorN(int dimension)
    {
        Values = new float[dimension];
    }

    public VectorN(float[] values)
    {
        Values = values;
    }

    public int Dimension => Values.Length;

    public float this[int index]
    {
        get => Values[index];
        set => Values[index] = value;
    }

    public VectorNRefStruct ToVectorNRefStruct(in Span<float> buffer)
    {
        for (var d = 0; d < Dimension; d++)
        {
            buffer[d] = Values[d];
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
            Values[d] = vector.Values[d];
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
            Values[d] = vector.Values[d];
        }
    }

    public override string ToString() => $"({string.Join(", ", Values.ToArray())})";
}
