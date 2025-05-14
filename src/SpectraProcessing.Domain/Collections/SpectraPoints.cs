namespace SpectraProcessing.Domain.Collections;

public sealed record SpectraPoints
{
    public readonly float[] X;

    public readonly float[] Y;

    public int Count => X.Length;

    public SpectraPoints(IReadOnlyList<float> x, IReadOnlyList<float> y)
    {
        if (x.Count != y.Count)
        {
            throw new ArgumentException("Different points count in x and y collections");
        }

        X = x.ToArray();
        Y = y.ToArray();
    }

    public SpectraPoints Copy() => new(X, Y);

    // public void Smooth()
    // {
    //     Span<float> tempY = stackalloc float[Count];
    //
    //     tempY[0] = (Y[0] + 2 * Y[1]) / 3;
    //     tempY[Count - 1] = (Y[Count - 1] + 2 * Y[Count - 2]) / 3;
    //
    //     for (var i = 1; i < Count - 1; i++)
    //     {
    //         tempY[i] = (Y[i - 1] + Y[i] + Y[i + 1]) / 3;
    //     }
    //
    //     for (var i = 0; i < Count; i++)
    //     {
    //         Y[i] = tempY[i];
    //     }
    // }

    public void Smooth()
    {
        const int centerIndex = 2;
        Span<float> tempY = stackalloc float[Count];
        Span<float> nearPoints = stackalloc float[7];

        for (var i = 0; i < Count; i++)
        {
            nearPoints[centerIndex] = Y[i];

            for (var j = 1; j < 3; j++)
            {
                var leftIndex = i - j < 0
                    ? i + j
                    : i - j;

                var rightIndex = i + j >= Count
                    ? i - j
                    : i + j;

                nearPoints[centerIndex - j] = Y[leftIndex];
                nearPoints[centerIndex + j] = Y[rightIndex];
            }

            tempY[i] = (nearPoints[0] + 3 * nearPoints[1] + 6 * nearPoints[2] + 7 * nearPoints[3] + 6 * nearPoints[4] +
                3 * nearPoints[5] + nearPoints[6]) / 27;
        }

        for (var i = 0; i < Count; i++)
        {
            Y[i] = tempY[i];
        }
    }

    public void Transform(Func<float, float, float> transformRule)
    {
        for (var i = 0; i < Count; i++)
        {
            Y[i] = transformRule(X[i], Y[i]);
        }
    }

    public IEnumerable<string> ToContents()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return $"{X[i]:0.00} {Y[i]:0.00}";
        }
    }
}
