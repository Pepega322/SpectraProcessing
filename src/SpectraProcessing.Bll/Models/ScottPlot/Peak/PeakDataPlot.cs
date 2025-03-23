using ScottPlot;
using SpectraProcessing.Bll.Math;
using SpectraProcessing.Bll.Models.ScottPlot.Plottables;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Models.ScottPlot.Peak;

public sealed class PeakDataPlot : IDataPlot
{
    private static readonly Color MarkerColor = Colors.Red;
    private static readonly Color PeakColor = Colors.Green;

    private readonly Lock locker = new();

    private readonly DraggableMarker leftMarker;
    private readonly DraggableMarker centerMarker;
    private readonly DraggableMarker rightMarker;

    public readonly PeakData Peak;

    public IPlottable Line { get; private set; }

    public IReadOnlyCollection<IPlottable> Markers
        =>
        [
            leftMarker,
            centerMarker,
            rightMarker,
        ];

    public PeakDataPlot(PeakData data)
    {
        const MarkerShape shape = MarkerShape.Cross;
        const float markerSize = 20f;

        Peak = data;

        using var builder = new Plot();

        leftMarker = new DraggableMarker(
            builder.Add.Marker(
                x: data.Center - data.HalfWidth / 2,
                y: data.Amplitude / 2,
                shape,
                markerSize,
                MarkerColor));

        centerMarker = new DraggableMarker(
            builder.Add.Marker(
                x: data.Center,
                y: data.Amplitude,
                shape,
                markerSize,
                MarkerColor));

        rightMarker = new DraggableMarker(
            builder.Add.Marker(
                x: data.Center + data.HalfWidth / 2,
                y: data.Amplitude / 2,
                shape,
                markerSize,
                MarkerColor));

        var line = builder.Add.Function(x => SpectraModeling.GaussianAndLorentzianMix(x, data));
        line.LineColor = PeakColor;
        Line = line;
    }

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
                halfWidth: Peak.HalfWidth,
                gaussianContribution: Peak.GaussianContribution);
        }
        else if (leftMarker.Dragged || rightMarker.Dragged)
        {
            var halfWidth = System.Math.Abs(value: (to.X - Peak.Center) * 2);

            OnPeakEstimateDataUpdate(
                center: Peak.Center,
                amplitude: Peak.Amplitude,
                halfWidth: halfWidth,
                gaussianContribution: Peak.GaussianContribution);
        }
    }

    private void OnPeakEstimateDataUpdate(
        float center,
        float amplitude,
        float halfWidth,
        float gaussianContribution)
    {
        locker.Enter();

        Peak.Amplitude = amplitude;
        Peak.Center = center;
        Peak.HalfWidth = halfWidth;
        Peak.GaussianContribution = gaussianContribution;

        var halfHeight = Peak.Amplitude / 2;

        DragMarkerTo(leftMarker, new Coordinates(Peak.Center - Peak.HalfWidth / 2, halfHeight));
        DragMarkerTo(centerMarker, new Coordinates(Peak.Center, Peak.Amplitude));
        DragMarkerTo(rightMarker, new Coordinates(Peak.Center + Peak.HalfWidth / 2, halfHeight));

        locker.Exit();

        return;

        void DragMarkerTo(DraggableMarker marker, Coordinates to)
        {
            marker.StartDrag(marker.Coordinates);
            marker.DragTo(to);
        }
    }
}
