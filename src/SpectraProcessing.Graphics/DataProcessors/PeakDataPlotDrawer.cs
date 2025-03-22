using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Peak;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.DataProcessors;

public class PeakDataPlotDrawer(PlotArea plotForm) : IDataPlotDrawer<PeakDataPlot>
{
    private readonly ISet<PeakDataPlot> plotted = new HashSet<PeakDataPlot>();

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
