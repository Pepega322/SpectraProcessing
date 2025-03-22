using ScottPlot;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

public sealed class SpectraDataGraphicsController(IDataPlotDrawer<SpectraDataPlot> drawer)
    : IGraphicsController<SpectraDataPlot>
{
    private static readonly Color HighlightColor = Colors.Black;

    private SpectraDataPlot? highlightedData;

    private DataSet<SpectraDataPlot>? highlightedSet;

    public Task<bool> IsDrew(SpectraDataPlot plot)
    {
        return drawer.IsDrew(plot);
    }

    public Task DrawData(SpectraDataPlot dataPlot)
    {
        return drawer.Draw(dataPlot);
    }

    public Task DrawDataSet(DataSet<SpectraDataPlot> set)
    {
        return Task.WhenAll(set.Data.Select(drawer.Draw));
    }

    public Task EraseData(SpectraDataPlot dataPlot)
    {
        return drawer.Erase(dataPlot);
    }

    public Task EraseDataSet(DataSet<SpectraDataPlot> set)
    {
        return Task.WhenAll(set.Data.Select(drawer.Erase));
    }

    public Task ChangeDataVisibility(SpectraDataPlot dataPlot, bool isVisible)
    {
        return isVisible ? drawer.Draw(dataPlot) : drawer.Erase(dataPlot);
    }

    public Task ChangeDataSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible)
    {
        return Task.WhenAll(set.Data.Select(plot => ChangeDataVisibility(plot, isVisible)));
    }

    public async Task HighlightData(SpectraDataPlot dataPlot)
    {
        if (highlightedData is not null)
        {
            await SetHighlighting(highlightedData, false);
        }

        if (Equals(highlightedData, dataPlot))
        {
            highlightedData = null;
        }
        else
        {
            highlightedData = dataPlot;
            await SetHighlighting(dataPlot, true);
        }
    }

    public async Task HighlightDataSet(DataSet<SpectraDataPlot> set)
    {
        if (highlightedSet is not null)
        {
            await Task.WhenAll(highlightedSet.Data.Select(data => SetHighlighting(data, false)));
        }

        if (Equals(highlightedSet, set))
        {
            highlightedSet = null;
        }
        else
        {
            highlightedSet = set;
            await Task.WhenAll(set.Data.Select(data => SetHighlighting(data, true)));
        }
    }

    public Task ClearArea()
    {
        highlightedData = null;
        highlightedSet = null;
        return drawer.Clear();
    }

    public Task ResizeArea()
    {
        return drawer.Resize();
    }

    private async Task SetHighlighting(SpectraDataPlot dataPlot, bool isHighlighted)
    {
        if (isHighlighted)
        {
            await drawer.Erase(dataPlot);
            dataPlot.ChangeColor(HighlightColor);
            await drawer.Draw(dataPlot);
        }
        else
        {
            dataPlot.ChangeColor(dataPlot.PreviousColor);
        }
    }
}
