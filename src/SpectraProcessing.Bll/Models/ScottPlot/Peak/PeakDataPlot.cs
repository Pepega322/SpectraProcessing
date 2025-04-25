using ScottPlot;
using SkiaSharp;
using SpectraProcessing.Bll.Models.ScottPlot.Plottables;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Models.ScottPlot.Peak;

public sealed class PeakDataPlot : IDataPlot
{
    private static int _counter = 0;

    private static readonly IPalette Palette = global::ScottPlot.Palette.Default;
    private static readonly Color PeakColor = Colors.Green;

    private readonly Lock locker = new();

    private readonly DraggableMarker leftMarker;

    private readonly DraggableMarker leftEffectiveRadiusMarker;
    private readonly DraggableMarker centerMarker;
    private readonly DraggableMarker rightMarker;
    private readonly DraggableMarker rightEffectiveRadiusMarker;
    private readonly DraggableMarker baselineMarker;

    public PeakData Peak { get; private set; }

    public IPlottable Line { get; private set; }

    public IReadOnlyCollection<IPlottable> Markers
        =>
        [
            leftMarker,
            leftEffectiveRadiusMarker,
            centerMarker,
            rightMarker,
            rightEffectiveRadiusMarker,
            baselineMarker,
        ];

    public PeakDataPlot(PeakData peak)
    {
        Peak = peak;

        var markerColor = Palette.GetColor(Interlocked.Increment(ref _counter));

        leftMarker = PlottableCreator.CreateDraggableMarker(
            x: peak.Center - peak.HalfWidth / 2,
            y: peak.Amplitude / 2,
            MarkerShape.Cross,
            markerColor);

        var effectiveRadius = PeakModeling.GetPeakEffectiveRadius(
            peak.HalfWidth,
            peak.GaussianContribution);

        leftEffectiveRadiusMarker = PlottableCreator.CreateDraggableMarker(
            x: peak.Center - effectiveRadius,
            y: 0,
            MarkerShape.OpenTriangleUp,
            markerColor,
            15f);

        centerMarker = PlottableCreator.CreateDraggableMarker(
            x: peak.Center,
            y: peak.Amplitude,
            MarkerShape.Cross,
            markerColor);

        rightMarker = PlottableCreator.CreateDraggableMarker(
            x: peak.Center + peak.HalfWidth / 2,
            y: peak.Amplitude / 2,
            MarkerShape.Cross,
            markerColor);

        rightEffectiveRadiusMarker = PlottableCreator.CreateDraggableMarker(
            x: peak.Center + effectiveRadius,
            y: 0,
            MarkerShape.OpenTriangleUp,
            markerColor,
            15f);

        baselineMarker = PlottableCreator.CreateDraggableMarker(
            x: peak.Center,
            y: 0,
            MarkerShape.Cross,
            markerColor);

        Line = PlottableCreator.CreateFunction(x => peak.GetPeakValueAt((float) x), PeakColor);
    }

    public void UpdateMarkers()
    {
        OnPeakEstimateDataUpdate(
            center: Peak.Center,
            amplitude: Peak.Amplitude,
            halfWidth: Peak.HalfWidth,
            gaussianContribution: Peak.GaussianContribution,
            baseline: Peak.Baseline);
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

        if (baselineMarker.IsHit(pixel, radius))
        {
            baselineMarker.Dragged = true;
            return true;
        }

        return false;
    }

    public void ReleaseHit()
    {
        leftMarker.Dragged = false;
        centerMarker.Dragged = false;
        rightMarker.Dragged = false;
        baselineMarker.Dragged = false;
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
                gaussianContribution: Peak.GaussianContribution,
                Peak.Baseline);
        }
        else if (leftMarker.Dragged || rightMarker.Dragged)
        {
            var halfWidth = Math.Abs((to.X - Peak.Center) * 2);

            OnPeakEstimateDataUpdate(
                center: Peak.Center,
                amplitude: Peak.Amplitude,
                halfWidth: halfWidth,
                gaussianContribution: Peak.GaussianContribution,
                baseline: Peak.Baseline);
        }
        else if (baselineMarker.Dragged)
        {
            var baseline = to.Y;

            OnPeakEstimateDataUpdate(
                center: Peak.Center,
                amplitude: Peak.Amplitude,
                halfWidth: Peak.HalfWidth,
                gaussianContribution: Peak.GaussianContribution,
                baseline: baseline);
        }
    }

    private void OnPeakEstimateDataUpdate(
        float center,
        float amplitude,
        float halfWidth,
        float gaussianContribution,
        float baseline)
    {
        locker.Enter();

        Peak.Amplitude = amplitude;
        Peak.Center = center;
        Peak.HalfWidth = halfWidth;
        Peak.GaussianContribution = gaussianContribution;
        Peak.Baseline = baseline;

        var halfHeight = Peak.Amplitude / 2;

        var effectiveRadius = PeakModeling.GetPeakEffectiveRadius(
            Peak.HalfWidth,
            Peak.GaussianContribution);

        leftMarker.DragMarkerTo(Peak.Center - Peak.HalfWidth / 2, Peak.Baseline + halfHeight);
        leftEffectiveRadiusMarker.DragMarkerTo(Peak.Center - effectiveRadius, Peak.Baseline);
        centerMarker.DragMarkerTo(Peak.Center, Peak.Baseline + Peak.Amplitude);
        rightMarker.DragMarkerTo(Peak.Center + Peak.HalfWidth / 2, Peak.Baseline + halfHeight);
        rightEffectiveRadiusMarker.DragMarkerTo(Peak.Center + effectiveRadius, Peak.Baseline);
        baselineMarker.DragMarkerTo(Peak.Center, Peak.Baseline);

        locker.Exit();
    }
}
