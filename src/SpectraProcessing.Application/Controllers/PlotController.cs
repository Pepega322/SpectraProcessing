using System.Collections.Concurrent;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application.Controllers;

public class PlotController(
    IDataStorageController<SpectraDataPlot> plotStorageController,
    IDataPlotBuilder<SpectraData, SpectraDataPlot> dataPlotBuilder,
    IGraphicsController<SpectraDataPlot> graphicsController,
    ISpectraProcessingController spectraProcessingController
) : IPlotController
{
    private event Action? OnPlotClear = () =>
    {
        plotStorageController.Clear();
        graphicsController.ClearArea();
        // spectraProcessingController.RedrawBorders();
    };

    public event Action? OnPlotAreaChanged;

    public event Action? OnPlotStorageChanged;

    public IReadOnlyCollection<DataSet<SpectraDataPlot>> Plots => plotStorageController.StorageData;

    public async Task ContextDataSetAddToPlotArea(DataSet<SpectraData> set)
    {
        var plots = await Task.WhenAll(set.Data.Select(dataPlotBuilder.GetPlot));

        var plotSet = new DataSet<SpectraDataPlot>(set.Name, plots);

        plotStorageController.AddDataSet(plotSet);
        graphicsController.DrawDataSet(plotSet);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextDataAddToClearPlotArea(DataSet<SpectraData> set)
    {
        OnPlotClear?.Invoke();
        await ContextDataSetAddToPlotArea(set);
    }

    public async Task DataAddToPlotAreaToDefault(SpectraData spectraData)
    {
        var plot = await dataPlotBuilder.GetPlot(spectraData);

        plotStorageController.AddDataToDefaultSet(plot);

        graphicsController.DrawData(plot);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public Task ContextDataAddToClearPlotToDefault(SpectraData spectraData)
    {
        OnPlotClear?.Invoke();
        return DataAddToPlotAreaToDefault(spectraData);
    }

    public Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible)
    {
        graphicsController.ChangeDataSetVisibility(set, isVisible);
        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ChangePlotVisibility(SpectraDataPlot dataPlot, bool isVisible)
    {
        graphicsController.ChangeDataVisibility(dataPlot, isVisible);
        OnPlotAreaChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextPlotSetHighlight(DataSet<SpectraDataPlot> set)
    {
        graphicsController.HighlightDataSet(set);
        OnPlotAreaChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task PlotHighlight(SpectraDataPlot dataPlot)
    {
        graphicsController.HighlightData(dataPlot);
        OnPlotAreaChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextPlotSetDelete(DataSet<SpectraDataPlot> set)
    {
        plotStorageController.DeleteDataSet(set);
        graphicsController.EraseDataSet(set);
        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextPlotDelete(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot dataPlot)
    {
        plotStorageController.DeleteData(ownerSet, dataPlot);
        graphicsController.EraseData(dataPlot);
        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
        return Task.CompletedTask;
    }

    public void PlotAreaClear()
    {
        OnPlotClear?.Invoke();
        OnPlotStorageChanged?.Invoke();
        OnPlotAreaChanged?.Invoke();
    }

    public void PlotAreaResize()
    {
        graphicsController.ResizeArea();
        OnPlotAreaChanged?.Invoke();
    }
}
