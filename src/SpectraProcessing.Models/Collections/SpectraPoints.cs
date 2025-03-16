namespace SpectraProcessing.Models.Collections;

public sealed record SpectraPoints
{
    public IReadOnlyList<float> X { get; init; }

    public IReadOnlyList<float> Y { get; init; }

    public int Count => X.Count;

    public SpectraPoints(IReadOnlyList<float> x, IReadOnlyList<float> y)
    {
        if (x.Count != y.Count)
        {
            throw new ArgumentException("Different points count in x and y collections");
        }

        X = x;
        Y = y;
    }

    public SpectraPoints Transform(Func<float, float, float> transformRule)
    {
        var transformedY = Enumerable.Range(0, Count)
            .Select(i => transformRule(X[i], Y[i]))
            .ToArray();

        return new SpectraPoints(X, transformedY);
    }

    public IEnumerable<string> ToContents()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return $"{X[i]: 0.00} {Y[i]: 0.00}";
        }
    }
}
