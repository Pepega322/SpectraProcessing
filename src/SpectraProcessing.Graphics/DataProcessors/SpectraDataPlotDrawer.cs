using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Spectra.Abstractions;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.DataProcessors;

public class SpectraDataPlotDrawer(PlotArea plotForm) : IDataPlotDrawer<SpectraDataPlot>
{
    private readonly ISet<SpectraDataPlot> plotted = new HashSet<SpectraDataPlot>();

    public Task Draw(SpectraDataPlot plt)
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
            plotForm.Add.Plottable(plt.Plottable);
        }

        return Task.CompletedTask;
    }

    public Task Erase(SpectraDataPlot plt)
    {
        lock (plotted)
        {
            if (!plotted.Contains(plt))
            {
                return Task.CompletedTask;
            }

            plotted.Remove(plt);
        }

        lock (plotForm)
        {
            plotForm.Remove(plt.Plottable);
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
            foreach (var plt in plotted)
            {
                plotForm.Remove(plt.Plottable);
            }
        }

        lock (plotted)
        {
            plotted.Clear();
        }

        return Task.CompletedTask;
    }
}
