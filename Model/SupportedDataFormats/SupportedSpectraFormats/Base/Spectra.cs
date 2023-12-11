using System.Drawing;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;

namespace Model.SupportedDataFormats.SupportedSpectraFormats.Base;
public abstract class Spectra : Data, IWriteable
{
    protected readonly List<PointF> _points;

    protected Spectra()
    {
        _points = [];
    }

    protected Spectra(List<PointF> points)
    {
        _points = points.ToList();
    }

    public virtual IEnumerable<string> ToContents() => _points.Select(p => $"{p.X} {p.Y}").ToArray();

    public abstract IEnumerable<string> ToOriginalContents();

    public IEnumerable<PointF> GetPoints()
    {
        foreach (var point in _points)
            yield return point;
    }
}
