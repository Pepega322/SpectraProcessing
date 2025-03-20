using ScottPlot;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.PeakEstimate;
using SpectraProcessing.Models.Plottables;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.DataProcessors;

public class PeakEstimateDataPlotBuilder : IDataPlotBuilder<PeakEstimateData, PeakEstimateDataPlot>
{
    private const MarkerShape shape = MarkerShape.Cross;

    private const float radius = 20f;

    private static long _counter;

    private static readonly IPalette Palette = new ScottPlot.Palettes.Category20();

    private readonly PlotArea builder = new();

    public Task<PeakEstimateDataPlot> GetPlot(PeakEstimateData plottableData)
    {
        var color = Palette.GetColor((int) Interlocked.Increment(ref _counter));

        var leftMarker = builder.Add.Marker(
            x: plottableData.Center - plottableData.HalfWidth / 2,
            y: plottableData.Amplitude / 2,
            shape,
            radius,
            color);

        var centerMarker = builder.Add.Marker(
            x: plottableData.Center,
            y: plottableData.Amplitude,
            shape,
            radius,
            color);

        var rightMarker = builder.Add.Marker(
            x: plottableData.Center + plottableData.HalfWidth / 2,
            y: plottableData.Amplitude / 2,
            shape,
            radius,
            color);

        var plot = new PeakEstimateDataPlot(
            estimateData: plottableData,
            leftMarker: new DraggableMarker(leftMarker),
            centerMarker: new DraggableMarker(centerMarker),
            rightMarker: new DraggableMarker(rightMarker));

        return Task.FromResult(plot);
    }
}
