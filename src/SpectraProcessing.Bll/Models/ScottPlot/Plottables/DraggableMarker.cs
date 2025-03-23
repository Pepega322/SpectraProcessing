using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Domain.Collections;

namespace SpectraProcessing.Bll.Models.ScottPlot.Plottables;

public sealed class DraggableMarker(Marker marker) : DraggablePlottableDecorator(marker)
{
    public Coordinates Coordinates => marker.Coordinates;

    public Point<float> Point => new(X, Y);

    public float X => (float) marker.X;

    public float Y => (float) marker.Y;

    public bool Dragged { get; set; }
}
