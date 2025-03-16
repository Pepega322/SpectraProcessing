using ScottPlot;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Controllers;

public sealed class ScottSpectraGraphicsController(IDataPlotDrawer<SpectraDataPlot> drawer) : IGraphicsController<SpectraDataPlot>
{
    private static readonly Color HighlightColor = Colors.Black;

    private SpectraDataPlot? highlightedData;
    private DataSet<SpectraDataPlot>? highlightedSet;

    public void DrawData(SpectraDataPlot dataPlot)
    {
        drawer.Draw(dataPlot);
    }

    public void DrawDataSet(DataSet<SpectraDataPlot> set)
    {
        foreach (var plot in set.Data)
        {
            DrawData(plot);
        }
    }

    public void EraseData(SpectraDataPlot dataPlot)
    {
        drawer.Erase(dataPlot);
    }

    public void EraseDataSet(DataSet<SpectraDataPlot> set)
    {
        foreach (var plot in set.Data)
        {
            EraseData(plot);
        }
    }

    public void ChangeDataVisibility(SpectraDataPlot dataPlot, bool isVisible)
    {
        if (isVisible)
        {
            drawer.Draw(dataPlot);
        }
        else
        {
            drawer.Erase(dataPlot);
        }
    }

    public void ChangeDataSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible)
    {
        Parallel.ForEach(set.Data, plot => ChangeDataVisibility(plot, isVisible));
    }

    public void HighlightData(SpectraDataPlot dataPlot)
    {
        if (highlightedData is not null)
        {
            SetHighlighting(highlightedData, false);
        }

        if (Equals(highlightedData, dataPlot))
        {
            highlightedData = null;
        }
        else
        {
            highlightedData = dataPlot;
            SetHighlighting(dataPlot, true);
        }
    }

    public void HighlightDataSet(DataSet<SpectraDataPlot> set)
    {
        if (highlightedSet is not null)
        {
            Parallel.ForEach(highlightedSet.Data, data => SetHighlighting(data, false));
        }

        if (Equals(highlightedSet, set))
        {
            highlightedSet = null;
        }
        else
        {
            highlightedSet = set;
            Parallel.ForEach(set.Data, data => SetHighlighting(data, true));
        }
    }

    public void ClearArea()
    {
        drawer.Clear();
        highlightedData = null;
        highlightedSet = null;
    }

    public void ResizeArea()
    {
        drawer.Resize();
    }

    private void SetHighlighting(SpectraDataPlot dataPlot, bool isHighlighted)
    {
        if (isHighlighted)
        {
            drawer.Erase(dataPlot);
            dataPlot.ChangeColor(HighlightColor);
            drawer.Draw(dataPlot);
        }
        else
        {
            dataPlot.ChangeColor(dataPlot.PreviousColor);
        }
    }
}
