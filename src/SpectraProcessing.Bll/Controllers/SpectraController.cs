using ScottPlot;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;
using Color = ScottPlot.Color;

namespace SpectraProcessing.Bll.Controllers;

internal sealed class SpectraController(
    IDataStorageProvider<StringKey, SpectraDataPlot> storageProvider,
    IDataPlotProvider<SpectraData, SpectraDataPlot> plotProvider
) : ISpectraController
{
    private static readonly Color HighlightColor = Colors.Black;

    private SpectraDataPlot? highlightedData;

    private DataSet<SpectraDataPlot>? highlightedSet;

    public event Action? OnPlotAreaChanged;

    public event Action? OnPlotStorageChanged;

    public IReadOnlyCollection<DataSet<SpectraDataPlot>> Plots
        => new[] { storageProvider.DefaultSet }
            .Concat(storageProvider.Sets.Values)
            .ToArray();

    public async Task AddToPlotArea(DataSet<SpectraData> set)
    {
        var plots = await plotProvider.Draw(set.Data);

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

    public async Task DataAddToPlotAreaToDefault(SpectraData data)
    {
        var plot = (await plotProvider.Draw([data])).Single();

        await storageProvider.AddDataToDefaultSet(plot);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextDataAddToClearPlotToDefault(SpectraData data)
    {
        await ClearPlot();
        await DataAddToPlotAreaToDefault(data);
    }

    public Task<bool> IsPlotVisible(SpectraDataPlot plot)
    {
        return plotProvider.IsDrew(plot.SpectraData);
    }

    public async Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible)
    {
        var data = set.Data.Select(x => x.SpectraData).ToArray();

        if (isVisible)
        {
            await plotProvider.Draw(data);
        }
        else
        {
            await plotProvider.Erase(data);
        }

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ChangePlotVisibility(SpectraDataPlot plot, bool isVisible)
    {
        if (isVisible)
        {
            await plotProvider.Draw([plot.SpectraData]);
        }
        else
        {
            await plotProvider.Erase([plot.SpectraData]);
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

    public async Task<bool> PlotHighlight(SpectraDataPlot plot)
    {
        var isHighlighted = true;

        if (highlightedData is not null)
        {
            await SetHighlighting(highlightedData, false);
        }

        if (Equals(highlightedData, plot))
        {
            highlightedData = null;
            isHighlighted = false;
        }
        else
        {
            highlightedData = plot;
            await SetHighlighting(plot, true);
        }

        OnPlotAreaChanged?.Invoke();

        return isHighlighted;
    }

    public async Task ContextPlotSetDelete(DataSet<SpectraDataPlot> set)
    {
        var data = set.Data.Select(x => x.SpectraData).ToArray();

        await plotProvider.Erase(data);

        await storageProvider.DeleteDataSet(set);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextPlotDelete(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot plot)
    {
        await plotProvider.Erase([plot.SpectraData]);

        await storageProvider.DeleteData(ownerSet, plot);

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
        await plotProvider.Resize();

        OnPlotAreaChanged?.Invoke();
    }

    private async Task ClearPlot()
    {
        await storageProvider.Clear();
        highlightedData = null;
        highlightedSet = null;
        await plotProvider.Clear();
    }

    private async Task SetHighlighting(SpectraDataPlot dataPlot, bool isHighlighted)
    {
        if (isHighlighted)
        {
            await plotProvider.PushOnTop([dataPlot.SpectraData]);
            dataPlot.ChangeColor(HighlightColor);
        }
        else
        {
            dataPlot.ChangeColor(dataPlot.PreviousColor);
        }
    }
}
