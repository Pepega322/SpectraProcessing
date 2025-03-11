using System.Collections.Concurrent;
using SpectraProcessing.Controllers;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain.Graphics;
using SpectraProcessing.Domain.SpectraData;
using SpectraProcessing.Domain.Storage;
using SpectraProcessing.Graphics.Formats;

namespace SpectraProcessing.Application.Controllers;

public class PlotController(
    IDataStorageController<SpectraPlot> plotStorageController,
    IPlotBuilder<Spectra, SpectraPlot> plotBuilder,
    IGraphicsController<SpectraPlot> graphicsController,
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

    public IReadOnlyCollection<DataSet<SpectraPlot>> Plots => plotStorageController.StorageData;

    public async Task ContextDataSetAddToPlotArea(DataSet<Spectra> set)
    {
        var plots = new ConcurrentBag<SpectraPlot>();

        await Parallel.ForEachAsync(
            set.Data,
            (data, _) =>
            {
                plots.Add(plotBuilder.GetPlot(data));
                return ValueTask.CompletedTask;
            });

        var plotSet = new DataSet<SpectraPlot>(set.Name, plots);

        plotStorageController.AddDataSet(plotSet);
        graphicsController.DrawDataSet(plotSet);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextDataAddToClearPlotArea(DataSet<Spectra> set)
    {
        OnPlotClear?.Invoke();
        await ContextDataSetAddToPlotArea(set);
    }

    public Task DataAddToPlotAreaToDefault(Spectra spectra)
    {
        var plot = plotBuilder.GetPlot(spectra);
        plotStorageController.AddDataToDefaultSet(plot);
        graphicsController.DrawData(plot);
        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextDataAddToClearPlotToDefault(Spectra spectra)
    {
        OnPlotClear?.Invoke();
        return DataAddToPlotAreaToDefault(spectra);
    }

    public Task ChangePlotSetVisibility(DataSet<SpectraPlot> set, bool isVisible)
    {
        graphicsController.ChangeDataSetVisibility(set, isVisible);
        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ChangePlotVisibility(SpectraPlot plot, bool isVisible)
    {
        graphicsController.ChangeDataVisibility(plot, isVisible);
        OnPlotAreaChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextPlotSetHighlight(DataSet<SpectraPlot> set)
    {
        graphicsController.HighlightDataSet(set);
        OnPlotAreaChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task PlotHighlight(SpectraPlot plot)
    {
        graphicsController.HighlightData(plot);
        OnPlotAreaChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextPlotSetDelete(DataSet<SpectraPlot> set)
    {
        plotStorageController.DeleteDataSet(set);
        graphicsController.EraseDataSet(set);
        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextPlotDelete(DataSet<SpectraPlot> ownerSet, SpectraPlot plot)
    {
        plotStorageController.DeleteData(ownerSet, plot);
        graphicsController.EraseData(plot);
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
