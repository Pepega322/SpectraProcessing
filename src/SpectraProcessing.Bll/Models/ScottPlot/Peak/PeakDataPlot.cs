using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Plottables;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Models.ScottPlot.Peak;

public sealed class PeakDataPlot : IDataPlot
{
    private static int _counter;

    private static readonly IPalette Palette = global::ScottPlot.Palette.Default;
    private static readonly Color PeakColor = Colors.Green;

    private readonly Lock locker = new();

    private readonly DraggableMarker leftMarker;

    // private readonly DraggableMarker leftEffectiveRadiusMarker;
    private readonly DraggableMarker centerMarker;
    private readonly DraggableMarker rightMarker;
    // private readonly DraggableMarker rightEffectiveRadiusMarker;

    public PeakData Peak { get; }

    public FunctionPlot Line { get; }

    public IReadOnlyCollection<IPlottable> Markers
        =>
        [
            leftMarker,
            centerMarker,
            rightMarker,
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

        Line = PlottableCreator.CreateFunction(x => peak.GetPeakValueAt((float) x), PeakColor);

        Line.MinX = peak.Center - effectiveRadius;
        Line.MaxX = peak.Center + effectiveRadius;
    }

    public void UpdateMarkers()
    {
        OnPeakEstimateDataUpdate(
            center: Peak.Center,
            amplitude: Peak.Amplitude,
            halfWidth: Peak.HalfWidth,
            gaussianContribution: Peak.GaussianContribution);
    }

    public bool TryHit(float x, float y, float xRadius, float yRadius)
    {
        if (leftMarker.IsHit(x, y, xRadius, yRadius))
        {
            leftMarker.Dragged = true;
            return true;
        }

        if (centerMarker.IsHit(x, y, xRadius, yRadius))
        {
            centerMarker.Dragged = true;
            return true;
        }

        if (rightMarker.IsHit(x, y, xRadius, yRadius))
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
            var halfWidth = Math.Abs(value: (to.X - Peak.Center) * 2);

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
        var halfHalfWidth = Peak.HalfWidth / 2;

        var effectiveRadius = PeakModeling.GetPeakEffectiveRadius(
            Peak.HalfWidth,
            Peak.GaussianContribution);

        leftMarker.DragTo(Peak.Center - halfHalfWidth, halfHeight);
        Line.MinX = Peak.Center - effectiveRadius;
        centerMarker.DragTo(Peak.Center, Peak.Amplitude);
        rightMarker.DragTo(Peak.Center + halfHalfWidth, halfHeight);
        Line.MaxX = Peak.Center + effectiveRadius;

        locker.Exit();
    }
}
