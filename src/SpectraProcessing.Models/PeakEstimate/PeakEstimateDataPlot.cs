using ScottPlot;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Plottables;

namespace SpectraProcessing.Models.PeakEstimate;

public sealed class PeakEstimateDataPlot(
    PeakEstimateData estimateData,
    DraggableMarker leftMarker,
    DraggableMarker centerMarker,
    DraggableMarker rightMarker
) : IDataPlot
{
    public PeakEstimateData EstimateData => estimateData;

    public IReadOnlyCollection<IPlottable> Markers
        =>
        [
            leftMarker,
            centerMarker,
            rightMarker,
        ];

    public bool TryHit(Pixel pixel, float radius)
    {
        if (leftMarker.IsHit(pixel, radius))
        {
            leftMarker.Dragged = true;
            return true;
        }

        if (centerMarker.IsHit(pixel, radius))
        {
            centerMarker.Dragged = true;
            return true;
        }

        if (rightMarker.IsHit(pixel, radius))
        {
            rightMarker.Dragged = true;
            return true;
        }

        return false;
    }

    public void ReleaseHit()
    {
        leftMarker.Dragged = false;
        centerMarker.Dragged = false;
        rightMarker.Dragged = false;
    }

    public void TryMoveTo(Point<float> to)
    {
        if (leftMarker.Dragged || rightMarker.Dragged)
        {
            estimateData.HalfWidth = to.X - estimateData.Center;

            if (estimateData.HalfWidth < 0)
            {
                estimateData.HalfWidth *= -1;
            }

            var leftX = estimateData.Center - estimateData.HalfWidth;
            var rightX = estimateData.Center + estimateData.HalfWidth;
            var halfHeight = estimateData.Amplitude / 2;

            leftMarker.StartDrag(leftMarker.Coordinates);
            rightMarker.StartDrag(rightMarker.Coordinates);

            leftMarker.DragTo(new Coordinates(leftX, halfHeight));
            rightMarker.DragTo(new Coordinates(rightX, halfHeight));

            return;
        }

        if (centerMarker.Dragged)
        {
            estimateData.Center = to.X;
            estimateData.Amplitude = to.Y;

            var leftX = estimateData.Center - estimateData.HalfWidth;
            var rightX = estimateData.Center + estimateData.HalfWidth;
            var halfHeight = estimateData.Amplitude / 2;

            leftMarker.StartDrag(leftMarker.Coordinates);
            centerMarker.StartDrag(centerMarker.Coordinates);
            rightMarker.StartDrag(rightMarker.Coordinates);

            leftMarker.DragTo(new Coordinates(leftX, halfHeight));
            centerMarker.DragTo(new Coordinates(estimateData.Center, estimateData.Amplitude));
            rightMarker.DragTo(new Coordinates(rightX, halfHeight));
        }
    }
}
