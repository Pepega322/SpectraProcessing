using ScottPlot.Plottables;

namespace SpectraProcessing.Bll.Models.ScottPlot.Plottables;

public sealed class DraggableMarker : Marker
{
    public bool Dragged { get; set; }

    public bool IsHit(float x, float y, float xRadius, float yRadius)
    {
        return Math.Abs(X - x) < xRadius && Math.Abs(Y - y) < yRadius;
    }

    public void DragTo(double x, double y)
    {
        X = x;
        Y = y;
    }
}
