using Model.MathHelper;

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
        var baseline = MathOperations.GetLinearRegression((IList<float>)xS, yS);
        for (var i = 0; i < PointCount; i++)
            copy.yS[i] -= baseline(copy.xS[i]);
        return copy;
    }

    public virtual Peak CalculatePeak(float xStart, float xEnd) {
        if (xEnd < xStart) (xStart, xEnd) = (xEnd, xStart);
        var left = MathOperations.GetClosestIndex((IList<float>)xS, xStart);
        var right = MathOperations.GetClosestIndex((IList<float>)xS, xEnd);
        var baseLine = MathOperations.GetLinearRegression([xS[left], xS[right]], [yS[left], yS[right]]);
        var square = 0f;
        var maxHeigth = 0f;
        for (var index = left; index < right; index++) {
            var heigth = yS[index] - baseLine(xS[index]);
            if (heigth > maxHeigth) maxHeigth = heigth;
            var nextHeigth = yS[index + 1] - baseLine(xS[index + 1]);
            var deltaX = xS[index + 1] - xS[index];
            square += MathOperations.GetQuadrangleSquare(heigth, nextHeigth, deltaX);
        }
        return new Peak(this, xS[left], yS[right], maxHeigth, square);
    }
}
