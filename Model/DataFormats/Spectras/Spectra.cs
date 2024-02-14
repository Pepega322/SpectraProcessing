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
}
