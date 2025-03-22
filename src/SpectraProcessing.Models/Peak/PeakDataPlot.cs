using ScottPlot;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Plottables;

namespace SpectraProcessing.Models.Peak;

public sealed class PeakDataPlot(
    PeakData peak,
    DraggableMarker leftMarker,
    DraggableMarker centerMarker,
    DraggableMarker rightMarker
) : IDataPlot
{
    private readonly Lock locker = new();

    public PeakData Peak => peak;

    public IReadOnlyCollection<IPlottable> Markers
        =>
        [
            leftMarker,
            centerMarker,
            rightMarker,
        ];

    public void UpdatePeakEstimateData(PeakData estimate)
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
                halfWidth: peak.HalfWidth,
                gaussianContribution: peak.GaussianContribution);
        }
        else if (leftMarker.Dragged || rightMarker.Dragged)
        {
            var halfWidth = Math.Abs(value: (to.X - peak.Center) * 2);

            OnPeakEstimateDataUpdate(
                center: peak.Center,
                amplitude: peak.Amplitude,
                halfWidth: halfWidth,
                gaussianContribution: peak.GaussianContribution);
        }
    }

    private void OnPeakEstimateDataUpdate(
        float center,
        float amplitude,
        float halfWidth,
        float gaussianContribution)
    {
        locker.Enter();

        peak.Amplitude = amplitude;
        peak.Center = center;
        peak.HalfWidth = halfWidth;
        peak.GaussianContribution = gaussianContribution;

        var halfHeight = peak.Amplitude / 2;

        DragMarkerTo(leftMarker, new Coordinates(peak.Center - peak.HalfWidth / 2, halfHeight));
        DragMarkerTo(centerMarker, new Coordinates(peak.Center, peak.Amplitude));
        DragMarkerTo(rightMarker, new Coordinates(peak.Center + peak.HalfWidth / 2, halfHeight));

        locker.Exit();

        return;

        void DragMarkerTo(DraggableMarker marker, Coordinates to)
        {
            marker.StartDrag(marker.Coordinates);
            marker.DragTo(to);
        }
    }
}
