namespace Model.DataFormats;
public abstract class Spectra : Data, IWriteable, ICopyable {
    protected IReadOnlyList<float> xS = null!;
    protected float[] yS = null!;
    public int PointCount => xS.Count;

    protected Spectra(string name)
        : base(name) { }

    protected Spectra(Spectra reference)
        : this(reference.Name) {
        xS = reference.xS;
        yS = reference.yS.ToArray();
    }

    public static bool TryParse(SpectraFormat format, string name, string[] contents, out Spectra spectra, out string message) {
        spectra = null;
        message = null;
        try {
            spectra = format switch {
                SpectraFormat.ASP => new ASP(name, contents),
                SpectraFormat.ESP => new ESP(name, contents),
                _ => throw new NotSupportedException()
            };
        }
        catch (Exception ex) {
            message = ex.Message;
            return false;
        }
        return true;
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
        var copy = (Spectra)CreateCopy();
        copy.Name = $"{copy.Name} -b";
        var (a, b) = GetMNK();
        for (var i = 0; i < PointCount; i++)
            copy.yS[i] -= a * copy.xS[i] + b;
        return copy;
    }

    //MNK - метод наименьших квадратов y = ax+b - линия тренда
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
