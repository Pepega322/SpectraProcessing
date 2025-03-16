using ScottPlot;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.PeakEstimate;
using SpectraProcessing.Models.Plottables;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.DataProcessors;

public class PeakEstimateDataPlotBuilder : IDataPlotBuilder<PeakEstimateData, PeakEstimateDataPlot>
{
    private const MarkerShape shape = MarkerShape.Cross;

    private const float radius = 10f;

    private static readonly Color Color = Colors.Red;

    private readonly PlotArea builder = new();

    public Task<PeakEstimateDataPlot> GetPlot(PeakEstimateData plottableData)
    {
        var leftMarker = builder.Add.Marker(
            x: plottableData.Center - plottableData.HalfWidth,
            y: plottableData.Amplitude / 2,
            shape,
            radius,
            Color);

        var centerMarker = builder.Add.Marker(
            x: plottableData.Center,
            y: plottableData.Amplitude,
            shape,
            radius,
            Color);

        var rightMarker = builder.Add.Marker(
            x: plottableData.Center + plottableData.HalfWidth,
            y: plottableData.Amplitude / 2,
            shape,
            radius,
            Color);

        var plot = new PeakEstimateDataPlot(
            estimateData: plottableData,
            leftMarker: new DraggableMarker(leftMarker),
            centerMarker: new DraggableMarker(centerMarker),
            rightMarker: new DraggableMarker(rightMarker));

        return Task.FromResult(plot);
    }
}
