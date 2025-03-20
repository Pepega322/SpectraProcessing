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
    private readonly Lock _locker = new();

    public PeakEstimateData EstimateData => estimateData;

    public IReadOnlyCollection<IPlottable> Markers
        =>
        [
            leftMarker,
            centerMarker,
            rightMarker,
        ];

    public void UpdatePeakEstimateData(PeakEstimateData estimate)
    {
        OnPeakEstimateDataUpdate(
            center: estimate.Center,
            amplitude: estimate.Amplitude,
            halfWidth: estimate.HalfWidth,
            gaussianContribution: estimate.GaussianContribution);
    }

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
        if (centerMarker.Dragged)
        {
            var center = to.X;
            var amplitude = to.Y;

            OnPeakEstimateDataUpdate(
                center: center,
                amplitude: amplitude,
                halfWidth: estimateData.HalfWidth,
                gaussianContribution: estimateData.GaussianContribution);
        }
        else if (leftMarker.Dragged || rightMarker.Dragged)
        {
            var halfWidth = Math.Abs(value: (to.X - estimateData.Center) * 2);

            OnPeakEstimateDataUpdate(
                center: estimateData.Center,
                amplitude: estimateData.Amplitude,
                halfWidth: halfWidth,
                gaussianContribution: estimateData.GaussianContribution);
        }
    }

    private void OnPeakEstimateDataUpdate(
        float center,
        float amplitude,
        float halfWidth,
        float gaussianContribution)
    {
        _locker.Enter();

        estimateData.Amplitude = amplitude;
        estimateData.Center = center;
        estimateData.HalfWidth = halfWidth;
        estimateData.GaussianContribution = gaussianContribution;

        var halfHeight = estimateData.Amplitude / 2;

        DragMarkerTo(leftMarker, new Coordinates(estimateData.Center - estimateData.HalfWidth / 2, halfHeight));
        DragMarkerTo(centerMarker, new Coordinates(estimateData.Center, estimateData.Amplitude));
        DragMarkerTo(rightMarker, new Coordinates(estimateData.Center + estimateData.HalfWidth / 2, halfHeight));

        _locker.Exit();
        return;

        void DragMarkerTo(DraggableMarker marker, Coordinates to)
        {
            marker.StartDrag(marker.Coordinates);
            marker.DragTo(to);
        }
    }
}
