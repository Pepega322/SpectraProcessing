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
    ISpectraDataPlotProvider spectraDataPlotProvider
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

    public async Task AddDataSetToPlot(DataSet<SpectraData> set)
    {
        var spectrasCopy = set.Data
            .Select(d => d.Copy())
            .ToArray();

        var plots = await spectraDataPlotProvider.Draw(spectrasCopy);

        var plotSet = new DataSet<SpectraDataPlot>(set.Name, plots);

        await storageProvider.AddDataSet(new StringKey(plotSet.Name), plotSet);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task AddDataSetToClearPlot(DataSet<SpectraData> set)
    {
        await ClearPlot();
        await AddDataSetToPlot(set);
        await PlotResize();
    }

    public async Task AddDataToPlotToDefault(SpectraData data)
    {
        var plot = (await spectraDataPlotProvider.Draw([data.Copy()])).Single();

        await storageProvider.AddDataToDefaultSet(plot);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task AddDataToClearPlotToDefault(SpectraData data)
    {
        await ClearPlot();
        await AddDataToPlotToDefault(data);
        await PlotResize();
    }

    public Task<bool> IsPlotVisible(SpectraDataPlot plot)
    {
        return spectraDataPlotProvider.IsDrew(plot.SpectraData);
    }

    public Task<bool> IsPlotHighlighted(SpectraDataPlot plot)
    {
        return Task.FromResult(plot.SpectraData.Equals(highlightedData?.SpectraData));
    }

    public async Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible)
    {
        var data = set.Data.Select(x => x.SpectraData).ToArray();

        if (isVisible)
        {
            await spectraDataPlotProvider.Draw(data);
        }
        else
        {
            await spectraDataPlotProvider.Erase(data);
        }

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ChangePlotVisibility(SpectraDataPlot plot, bool isVisible)
    {
        if (isVisible)
        {
            await spectraDataPlotProvider.Draw([plot.SpectraData]);
        }
        else
        {
            await spectraDataPlotProvider.Erase([plot.SpectraData]);
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

    public Task PlotRemoveHighlight(SpectraDataPlot plot)
    {
        return SetHighlighting(plot, false);
    }

    public async Task ErasePlotSet(DataSet<SpectraDataPlot> set)
    {
        var data = set.Data.Select(x => x.SpectraData).ToArray();

        await spectraDataPlotProvider.Erase(data);

        await storageProvider.DeleteDataSet(set);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ErasePlot(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot plot)
    {
        await spectraDataPlotProvider.Erase([plot.SpectraData]);

        await storageProvider.DeleteData(ownerSet, plot);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task PlotClear()
    {
        await ClearPlot();

        OnPlotStorageChanged?.Invoke();
        OnPlotAreaChanged?.Invoke();
    }

    public async Task PlotResize()
    {
        await spectraDataPlotProvider.Resize();

        OnPlotAreaChanged?.Invoke();
    }

    private async Task ClearPlot()
    {
        await storageProvider.Clear();
        highlightedData = null;
        highlightedSet = null;
        await spectraDataPlotProvider.Clear();
    }

    private async Task SetHighlighting(SpectraDataPlot dataPlot, bool isHighlighted)
    {
        if (isHighlighted)
        {
            await spectraDataPlotProvider.PushOnTop([dataPlot.SpectraData]);
            dataPlot.ChangeColor(HighlightColor);
        }
        else
        {
            dataPlot.ChangeColor(dataPlot.PreviousColor);
        }
    }
}
