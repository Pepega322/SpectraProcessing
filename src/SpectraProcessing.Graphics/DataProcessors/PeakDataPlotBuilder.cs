using ScottPlot;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Peak;
using SpectraProcessing.Models.Plottables;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.DataProcessors;

public class PeakDataPlotBuilder : IDataPlotBuilder<PeakData, PeakDataPlot>
{
    private const MarkerShape shape = MarkerShape.Cross;

    public const float MarkerSize = 20f;

    private static readonly Color Color = Colors.Red;

    private readonly PlotArea builder = new();

    public Task<PeakDataPlot> GetPlot(PeakData plottableData)
    {
        var leftMarker = builder.Add.Marker(
            x: plottableData.Center - plottableData.HalfWidth / 2,
            y: plottableData.Amplitude / 2,
            shape,
            MarkerSize,
            Color);

        var centerMarker = builder.Add.Marker(
            x: plottableData.Center,
            y: plottableData.Amplitude,
            shape,
            MarkerSize,
            Color);

        var rightMarker = builder.Add.Marker(
            x: plottableData.Center + plottableData.HalfWidth / 2,
            y: plottableData.Amplitude / 2,
            shape,
            MarkerSize,
            Color);

        var plot = new PeakDataPlot(
            peak: plottableData,
            leftMarker: new DraggableMarker(leftMarker),
            centerMarker: new DraggableMarker(centerMarker),
            rightMarker: new DraggableMarker(rightMarker));

        return Task.FromResult(plot);
    }
}
