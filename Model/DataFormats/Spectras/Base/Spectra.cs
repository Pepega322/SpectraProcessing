using Model.MathHelper;

namespace Model.DataFormats;
public abstract class Spectra : Data, IWriteable, ICopyable {
    public SpectraFormat Format { get; protected set; }
    public SpectraPoints Points { get; protected set; } = null!;
    public int PointCount => Points.Count;

    public static Spectra Parse(SpectraFormat format, string name, string[] contents) {
        Spectra spectra;
        try {
            spectra = format switch {
                SpectraFormat.ASP => ASP.Parse(name, contents),
                SpectraFormat.ESP => ESP.Parse(name, contents),
                _ => throw new NotSupportedException()
            };
        }
        catch { throw; }
        return spectra;
    }

    protected Spectra(string name, SpectraPoints points) : base(name) {
        Points = points;
    }

    public abstract Data CreateCopy();

    public virtual Spectra SubstractBaseLine() {
        var substracted = (Spectra)CreateCopy();
        var baseline = MathOperations.GetLinearRegression(Points);
        Func<float, float, float> transformationRule = (x, y) => y - baseline(x);
        substracted.Points = Points.Transform(transformationRule);
        return substracted;
    }

    public virtual PeakInfo ProcessPeak(PeakBorder border) {
        var left = MathOperations.GetClosestIndex(Points.X, border.Left);
        var right = MathOperations.GetClosestIndex(Points.X, border.Rigth);
        var baseline = MathOperations.GetLinearRegression(Points[left], Points[right]);
        Func<int, float> getHeigth = (index) => Points.Y[index] - baseline(Points.X[index]);
        Func<int, float> getDeltaX = (index) => Points.X[index + 1] - Points.X[index];

        var square = 0f;
        var maxHeigth = 0f;
        var heigth = getHeigth(left);
        for (var index = left; index < right; index++) {
            if (heigth > maxHeigth) maxHeigth = heigth;
            var nextHeigth = getHeigth(index + 1);
            var dS = MathOperations.GetQuadrangleSquare(heigth, nextHeigth, getDeltaX(index));
            if (dS > 0) square += dS;
            heigth = nextHeigth;
        }
        return new PeakInfo(this, Points.X[left], Points.X[right], square, maxHeigth);
    }

    public IEnumerable<string> ToContents() => Points.ToContents();
}
