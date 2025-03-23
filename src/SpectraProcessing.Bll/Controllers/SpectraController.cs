using ScottPlot;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Collections.Keys;
using SpectraProcessing.Models.Spectra.Abstractions;
using Color = ScottPlot.Color;

namespace SpectraProcessing.Bll.Controllers;

public class SpectraController(
    IDataStorageProvider<StringKey, SpectraDataPlot> storageProvider,
    IDataPlotBuilder<SpectraData, SpectraDataPlot> plotBuilder,
    IDataPlotDrawer<SpectraDataPlot> plotDrawer
) : ISpectraController
{
    private static readonly Color HighlightColor = Colors.Black;

    private SpectraDataPlot? highlightedData;

    private DataSet<SpectraDataPlot>? highlightedSet;

    public event Action? OnPlotAreaChanged;

    public event Action? OnPlotStorageChanged;

    public IReadOnlyCollection<DataSet<SpectraDataPlot>> Plots => storageProvider.StorageDataSets;

    public async Task AddToPlotArea(DataSet<SpectraData> set)
    {
        var plots = await Task.WhenAll(set.Data.Select(plotBuilder.GetPlot));

        await Task.WhenAll(plots.Select(plotDrawer.Draw));

        var plotSet = new DataSet<SpectraDataPlot>(set.Name, plots);

        await storageProvider.AddDataSet(new StringKey(plotSet.Name), plotSet);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextDataAddToClearPlotArea(DataSet<SpectraData> set)
    {
        await ClearPlot();
        await AddToPlotArea(set);
    }

    public async Task DataAddToPlotAreaToDefault(SpectraData spectraData)
    {
        var plot = await plotBuilder.GetPlot(spectraData);

        await plotDrawer.Draw(plot);

        await storageProvider.AddDataToDefaultSet(plot);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextDataAddToClearPlotToDefault(SpectraData spectraData)
    {
        await ClearPlot();
        await DataAddToPlotAreaToDefault(spectraData);
    }

    public Task<bool> IsPlotVisible(SpectraDataPlot dataPlot)
    {
        return plotDrawer.IsDrew(dataPlot);
    }

    public async Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible)
    {
        if (isVisible)
        {
            await Task.WhenAll(set.Data.Select(plotDrawer.Draw));
        }
        else
        {
            await Task.WhenAll(set.Data.Select(plotDrawer.Erase));
        }

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ChangePlotVisibility(SpectraDataPlot dataPlot, bool isVisible)
    {
        if (isVisible)
        {
            await plotDrawer.Draw(dataPlot);
        }
        else
        {
            await plotDrawer.Erase(dataPlot);
        }

        OnPlotAreaChanged?.Invoke();
    }

    public async Task ContextPlotSetHighlight(DataSet<SpectraDataPlot> set)
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

        OnPlotAreaChanged?.Invoke();
    }

    public async Task PlotHighlight(SpectraDataPlot dataPlot)
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

        OnPlotAreaChanged?.Invoke();
    }

    public async Task ContextPlotSetDelete(DataSet<SpectraDataPlot> set)
    {
        await Task.WhenAll(set.Data.Select(plotDrawer.Erase));

        await storageProvider.DeleteDataSet(new StringKey(set.Name), set);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextPlotDelete(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot dataPlot)
    {
        await plotDrawer.Erase(dataPlot);

        await storageProvider.DeleteData(ownerSet, dataPlot);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task PlotAreaClear()
    {
        await ClearPlot();

        OnPlotStorageChanged?.Invoke();
        OnPlotAreaChanged?.Invoke();
    }

    public async Task PlotAreaResize()
    {
        await plotDrawer.Resize();

        OnPlotAreaChanged?.Invoke();
    }

    private async Task ClearPlot()
    {
        await storageProvider.Clear();
        highlightedData = null;
        highlightedSet = null;
        await plotDrawer.Clear();
    }

    private async Task SetHighlighting(SpectraDataPlot dataPlot, bool isHighlighted)
    {
        if (isHighlighted)
        {
            await plotDrawer.Erase(dataPlot);
            dataPlot.ChangeColor(HighlightColor);
            await plotDrawer.Draw(dataPlot);
        }
        else
        {
            dataPlot.ChangeColor(dataPlot.PreviousColor);
        }
    }
}
