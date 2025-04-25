using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Domain.Collections;

namespace SpectraProcessing.Bll.Models.ScottPlot.Plottables;

public sealed class DraggableMarker(Marker marker) : DraggablePlottableDecorator(marker)
{
    private Coordinates tempCoordinates = new(0, 0);

    public bool Dragged { get; set; }

    public void DragMarkerTo(float x, float y)
    {
        StartDrag(marker.Coordinates);
        tempCoordinates.X = x;
        tempCoordinates.Y = y;
        DragTo(tempCoordinates);
    }
}
