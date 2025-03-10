using ScottPlot;
using SpectraProcessing.Domain.Graphics;
using SpectraProcessing.Graphics.Formats;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.Graphics;

public class ScottPlotDrawer(PlotArea plotArea) : IPlotDrawer<SctPlot>
{
    private static readonly Color HighlightColor = Colors.Black;
    private readonly HashSet<SctPlot> plotted = [];

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

    public void SetHighlight(SctPlot plt, bool isHighlighted)
    {
        if (isHighlighted)
        {
            PushOnTop(plt);
            plt.ChangeColor(HighlightColor);
        }
        else
        {
            plt.ChangeColor(plt.PreviousColor);
        }
    }

    public void SetVisibility(SctPlot plot, bool isVisible)
    {
        foreach (var p in plot.Plottables)
        {
            p.IsVisible = isVisible;
        }
    }

    public void Resize()
    {
        lock (plotArea)
        {
            plotArea.Axes.AutoScaleX();
            plotArea.Axes.AutoScaleY();
        }
    }

    public void Clear()
    {
        lock (plotted) plotted.Clear();
        lock (plotArea) plotArea.Clear();
    }

    private void PushOnTop(SctPlot plt)
    {
        RemoveFromArea(plt);
        AddToArea(plt);
    }

    private void AddToArea(SctPlot plot)
    {
        lock (plotArea)
        {
            foreach (var plt in plot.Plottables)
            {
                plotArea.Add.Plottable(plt);
            }
        }
    }

    private void RemoveFromArea(SctPlot plot)
    {
        lock (plotArea)
        {
            foreach (var plt in plot.Plottables)
            {
                plotArea.Remove(plt);
            }
        }
    }
}
