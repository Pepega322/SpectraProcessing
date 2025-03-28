using SpectraProcessing.Domain.Extensions;

namespace SpectraProcessing.Domain.Models.MathModeling;

public sealed class Simplex
{
    private static readonly Comparer<SimplexPoint> Comparer
        = Comparer<SimplexPoint>.Create((x, y) => x.Value.CompareTo(y.Value));

    private readonly Func<VectorN, double> funcForMin;
    private readonly List<SimplexPoint> values;

    public double WorstValue => values[^1].Value;

    public VectorN Worst
    {
        get => values[^1].Vector;
        set
        {
            values[^1].Vector = value;
            values[^1].Value = GetFuncValue(value);
            values.Sort(Comparer);
        }
    }

    public double BestValue => values[0].Value;

    public VectorN Best => values[0].Vector;

    public double SecondWorstValue => values[^2].Value;

    public VectorN SecondWorst => values[^2].Vector;

    public VectorN Center
        => values
            .Take(values.Count - 1)
            .Select(x => x.Vector)
            .ToArray()
            .Sum() / (values.Count - 1);

    public Simplex(
        VectorN start,
        Func<VectorN, double> func,
        SimplexSettings settings)
    {
        funcForMin = func;
        values = new List<SimplexPoint>(start.Dimension + 1)
        {
            new() { Vector = start, Value = func(start) },
        };

        for (var d = 0; d < start.Dimension; d++)
        {
            var newVector = new VectorN(start.Values);

            newVector[d] = newVector[d].ApproximatelyEqual(0)
                ? settings.InitialShift
                : newVector[d] * (1 + settings.InitialShift);

            values.Add(new SimplexPoint { Vector = newVector, Value = func(newVector) });
        }

        values.Sort(Comparer);
    }

    public double GetFuncValue(VectorN vector) => funcForMin(vector);

    public void Shrink(double shrinkCoefficient)
    {
        foreach (var point in values.Skip(1))
        {
            var shrinked = Best + shrinkCoefficient * (point.Vector - Best);
            point.Vector = shrinked;
            point.Value = GetFuncValue(shrinked);
        }
    }

    private sealed record SimplexPoint
    {
        public required VectorN Vector { get; set; }

        public required double Value { get; set; }
    }
}
