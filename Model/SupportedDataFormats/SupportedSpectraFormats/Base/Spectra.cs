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

    public abstract IEnumerable<string> ToContents();
    public virtual IEnumerable<string> PointsToContents() => _points.Select(p => $"{p.X} {p.X}").ToArray();
    public IEnumerable<PointF> GetPoints()
    {
        foreach (var point in _points)
            yield return point;
    }
}
