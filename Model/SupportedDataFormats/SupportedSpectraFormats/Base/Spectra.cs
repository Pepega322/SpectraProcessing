using System.Drawing;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;

namespace Model.SupportedDataFormats.SupportedSpectraFormats.Base;
public abstract class Spectra : Data, IWriteable
{
    public int PointsCount => _xS.Length;
    protected double[] _xS { get; set; } = null!;
    protected double[] _yS { get; set; } = null!;

    protected Spectra()
    {
        //_xS = GetXS();
        //_yS = GetYS();
    }

    protected Spectra(Spectra reference)
    {
        _xS = reference._xS;
        _yS = reference._yS.ToArray();
    }

    public virtual IEnumerable<string> ToContents()
    {
        for (var i = 0; i < PointsCount; i++)
            yield return $"{_xS[i]} {_yS[i]}";
    }

    public abstract IEnumerable<string> ToOriginalContents();

    public abstract double[] GetXS(params string[] contents);

    public abstract double[] GetYS(params string[] contents);

    public (double[] xS, double[] yS) GetPoints()
    {
        return (_xS.ToArray(), _yS.ToArray());
    }
}
