using System.Drawing;
using Model.Command;
using Model.Visual;

namespace Model.Data.Spectra;
public abstract class Spectra : IData, IWriteable, IVisualizable
{
    protected readonly List<PointF> _points;

    protected Spectra()
    {
        _points = [];
    }

    protected Spectra(List<PointF> points)
    {
        _points = points;
    }

    public abstract IData CreateCopy();
    public abstract void Edit(IDataEditCommand command);
    public abstract IData GetInfo(IGetDataInfoCommand command);
    public abstract IEnumerable<string> ToContents();
    public abstract IVisual Visualize();
}
