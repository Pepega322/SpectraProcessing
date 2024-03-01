using Model.MathHelper;

namespace Model.DataFormats;

public class SpectraPoints : IWriteable {
    public IReadOnlyList<float> X { get; init; }
    public IReadOnlyList<float> Y { get; init; }
    public int Count => X.Count;
    public Point<float> this[int index] {
        get {
            if (index < 0 || index >= X.Count)
                throw new ArgumentOutOfRangeException("index");
            return new(X[index], Y[index]);
        }
    }

    public SpectraPoints(IList<float> x, IList<float> y) {
        if (x.Count != y.Count) throw new ArgumentException();
        X = x.AsReadOnly();
        Y = y.AsReadOnly();
    }

    public SpectraPoints Transform(Func<float, float, float> transformRule) {
        var transformedY = new List<float>();
        for (var i = 0; i < Count; i++)
            transformedY.Add(transformRule(X[i], Y[i]));
        return new SpectraPoints((IList<float>)X, transformedY);
    }

    public IEnumerable<string> ToContents() {
        for (var i = 0; i < Count; i++)
            yield return $"{X[i]} {Y[i]}";
    }
}
