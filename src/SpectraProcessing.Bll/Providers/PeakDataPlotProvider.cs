using ScottPlot;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Models.ScottPlot.Plottables;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Providers;

internal sealed class PeakDataPlotProvider(Plot plotForm) : IDataPlotProvider<PeakData, PeakDataPlot>
{
    public const MarkerShape Shape = MarkerShape.Cross;

    public const float MarkerSize = 20f;

    private static readonly Color Color = Colors.Red;

    private readonly Plot builder = new();

    private readonly ISet<PeakDataPlot> plotted = new HashSet<PeakDataPlot>();

    public Task<PeakDataPlot> GetPlot(PeakData plottableData)
    {
        var leftMarker = builder.Add.Marker(
            x: plottableData.Center - plottableData.HalfWidth / 2,
            y: plottableData.Amplitude / 2,
            Shape,
            MarkerSize,
            Color);

        var centerMarker = builder.Add.Marker(
            x: plottableData.Center,
            y: plottableData.Amplitude,
            Shape,
            MarkerSize,
            Color);

        var rightMarker = builder.Add.Marker(
            x: plottableData.Center + plottableData.HalfWidth / 2,
            y: plottableData.Amplitude / 2,
            Shape,
            MarkerSize,
            Color);

        var plot = new PeakDataPlot(
            peak: plottableData,
            leftMarker: new DraggableMarker(leftMarker),
            centerMarker: new DraggableMarker(centerMarker),
            rightMarker: new DraggableMarker(rightMarker));

        return Task.FromResult(plot);
    }

    public Task<bool> IsDrew(PeakDataPlot plot)
    {
        lock (plotted)
        {
            return Task.FromResult(plotted.Contains(plot));
        }
    }

    public Task Draw(PeakDataPlot plt)
    {
        lock (plotted)
        {
            if (!plotted.Add(plt))
            {
                return Task.CompletedTask;
            }
        }

        lock (plotForm)
        {
            foreach (var marker in plt.Markers)
            {
                plotForm.Add.Plottable(marker);
            }
        }

        return Task.CompletedTask;
    }

    public Task Erase(PeakDataPlot plt)
    {
        lock (plotted)
        {
            if (!plotted.Remove(plt))
            {
                return Task.CompletedTask;
            }
        }

        lock (plotForm)
        {
            foreach (var marker in plt.Markers)
            {
                plotForm.Remove(marker);
            }
        }

        return Task.CompletedTask;
    }

    public Task Resize()
    {
        lock (plotForm)
        {
            plotForm.Axes.AutoScaleX();
            plotForm.Axes.AutoScaleY();
        }

        return Task.CompletedTask;
    }

    public Task Clear()
    {
        lock (plotForm)
        {
            foreach (var marker in plotted.SelectMany(p => p.Markers))
            {
                plotForm.Remove(marker);
            }
        }

        lock (plotted)
        {
            plotted.Clear();
        }

        return Task.CompletedTask;
    }
}
