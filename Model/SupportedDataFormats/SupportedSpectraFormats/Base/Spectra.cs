using System.Drawing;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;
using Model.SupportedVisualFormats.Base;

namespace Model.SupportedDataFormats.SupportedSpectraFormats.Base;
internal abstract class Spectra : Data, IWriteable, IVisualizable
{
    protected static int s_firstPointLineIndex { get; set; }

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
    public abstract Visual GetVisualization();
    public virtual IEnumerable<string> PointsToContents() => _points.Select(p => $"{p.X} {p.X}").ToArray();
}
