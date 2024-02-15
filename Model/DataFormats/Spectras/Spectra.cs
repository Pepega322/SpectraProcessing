namespace Model.DataFormats;
public abstract class Spectra : Data, IWriteable, ICopyable {
    public int PointCount => xS.Count;
    protected IReadOnlyList<float> xS = null!;
    protected float[] yS = null!;

    protected Spectra() {
    }

    protected Spectra(Spectra reference) {
        Name = $"{reference.Name} (copy)";
        xS = reference.xS;
        yS = reference.yS.ToArray();
    }

    public abstract Data CreateCopy();

    protected abstract (IReadOnlyList<float> xS, float[] yS) ReadPoints(params string[] contents);

    public virtual IEnumerable<string> ToContents() {
        for (var i = 0; i < xS.Count; i++)
            yield return $"{xS[i]} {yS[i]}";
    }

    public virtual IEnumerable<string> ToOriginalContents() => ToContents();

    public virtual (IReadOnlyList<float> xS, float[] yS) GetPoints() => (xS, yS.ToArray());

    public Spectra SubstractBaseLine() {
        var result = CreateCopy() as Spectra;
        if (result == null) throw new Exception("Cant substract baseline");
        var (a, b) = GetMNK();
        for (var i = 0; i < PointCount; i++)
            yS[i] -= a * xS[i] + b;
        return result;
    }

    //MNK - метод наименьших квадратов y = ax+b
    private (float a, float b) GetMNK() {
        var xSum = xS.Sum();
        var x2Sum = xS.Sum(e => e * e);
        var ySum = yS.Sum();
        var xySum = 0f;
        for (var i = 0; i < PointCount; i++)
            xySum += xS[i] * yS[i];
        var z = PointCount * x2Sum - xSum * xSum;
        var a = (PointCount * xySum - xSum * ySum) / z;
        var b = (ySum * x2Sum - xSum * xySum) / z;
        return (a, b);
    }
}
