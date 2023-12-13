using Model.DataFormats.Base;
using Model.DataFormats.Interfaces;

namespace Model.DataFormats.Spectras.Base;
public abstract class Spectra : Data, IWriteable, ICopyable
{
    public int PointsCount => _xS.Count;
    protected  IReadOnlyList<double> _xS = null!;
    protected  double[] _yS = null!;

    protected Spectra()
    {
    }

    protected Spectra(Spectra reference)
    {
        Name = $"{reference.Name} (copy)";
        _xS = reference._xS;
        _yS = reference._yS.ToArray();
    }

    public abstract Data CreateCopy();
    protected abstract (IReadOnlyList<double> xS, double[] yS) ReadPoints(params string[] contents);
    public virtual IEnumerable<string> ToContents()
    {
        for (var i = 0; i < PointsCount; i++)
            yield return $"{_xS[i]} {_yS[i]}";
    }
    public virtual IEnumerable<string> ToOriginalContents() => ToContents();
    public virtual (IReadOnlyList<double> xS, double[] yS) GetPoints() => (_xS, _yS.ToArray());
}
