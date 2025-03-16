using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.PeakEstimate;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.DataProcessors;

public class PeakEstimateDataPlotDrawer(PlotArea plotForm) : IDataPlotDrawer<PeakEstimateDataPlot>
{
    private readonly ISet<PeakEstimateDataPlot> plotted = new HashSet<PeakEstimateDataPlot>();

    public Task<bool> IsDrew(PeakEstimateDataPlot plot)
    {
        lock (plotted)
        {
            return Task.FromResult(plotted.Contains(plot));
        }
    }

    public Task Draw(PeakEstimateDataPlot plt)
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

    public Task Erase(PeakEstimateDataPlot plt)
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
