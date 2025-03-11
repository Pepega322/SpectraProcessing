using SpectraProcessing.Domain.Graphics;
using SpectraProcessing.Graphics.Formats;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.Graphics;

public class ScottPlotDrawer(PlotArea plotForm) : IPlotDrawer<SctPlot>
{
    private readonly ISet<SctPlot> plotted = new HashSet<SctPlot>();

    public void Draw(SctPlot plt)
    {
        lock (plotted)
        {
            if (!plotted.Add(plt))
            {
                return;
            }
        }

        AddToArea(plt);
    }

    public void Erase(SctPlot plt)
    {
        lock (plotted)
        {
            if (!plotted.Contains(plt))
            {
                return;
            }

            plotted.Remove(plt);
        }

        RemoveFromArea(plt);
    }

    public void Resize()
    {
        lock (plotForm)
        {
            plotForm.Axes.AutoScaleX();
            plotForm.Axes.AutoScaleY();
        }
    }

    public void Clear()
    {
        lock (plotted)
        {
            plotted.Clear();
        }

        lock (plotForm)
        {
            plotForm.Clear();
        }
    }

    private void AddToArea(SctPlot plot)
    {
        lock (plotForm)
        {
            foreach (var plt in plot.Plottables)
            {
                plotForm.Add.Plottable(plt);
            }
        }
    }

    private void RemoveFromArea(SctPlot plot)
    {
        lock (plotForm)
        {
            foreach (var plt in plot.Plottables)
            {
                plotForm.Remove(plt);
            }
        }
    }
}
