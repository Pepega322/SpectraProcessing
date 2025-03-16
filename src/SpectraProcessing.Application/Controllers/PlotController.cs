using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application.Controllers;

public class PlotController(
    IDataPlotBuilder<SpectraData, SpectraDataPlot> spectraDataPlotBuilder,
    IDataStorageController<SpectraDataPlot> spectraDataStorageController,
    IGraphicsController<SpectraDataPlot> spectraGraphicsController,
    ISpectraProcessingController spectraProcessingController
) : IPlotController
{
    public event Action? OnPlotAreaChanged;

    public event Action? OnPlotStorageChanged;

    public IReadOnlyCollection<DataSet<SpectraDataPlot>> Plots => spectraDataStorageController.StorageData;

    public async Task ContextDataSetAddToPlotArea(DataSet<SpectraData> set)
    {
        var plots = await Task.WhenAll(set.Data.Select(spectraDataPlotBuilder.GetPlot));

        var plotSet = new DataSet<SpectraDataPlot>(set.Name, plots);

        await spectraDataStorageController.AddDataSet(plotSet);

        await spectraGraphicsController.DrawDataSet(plotSet);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextDataAddToClearPlotArea(DataSet<SpectraData> set)
    {
        await ClearPlot();
        await ContextDataSetAddToPlotArea(set);
    }

    public async Task DataAddToPlotAreaToDefault(SpectraData spectraData)
    {
        var plot = await spectraDataPlotBuilder.GetPlot(spectraData);

        await spectraDataStorageController.AddDataToDefaultSet(plot);

        await spectraGraphicsController.DrawData(plot);

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
        return spectraGraphicsController.IsDrew(dataPlot);
    }

    public async Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible)
    {
        await spectraGraphicsController.ChangeDataSetVisibility(set, isVisible);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ChangePlotVisibility(SpectraDataPlot dataPlot, bool isVisible)
    {
        await spectraGraphicsController.ChangeDataVisibility(dataPlot, isVisible);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task ContextPlotSetHighlight(DataSet<SpectraDataPlot> set)
    {
        await spectraGraphicsController.HighlightDataSet(set);
        OnPlotAreaChanged?.Invoke();
    }

    public async Task PlotHighlight(SpectraDataPlot dataPlot)
    {
        await spectraGraphicsController.HighlightData(dataPlot);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task ContextPlotSetDelete(DataSet<SpectraDataPlot> set)
    {
        await spectraDataStorageController.DeleteDataSet(set);
        await spectraGraphicsController.EraseDataSet(set);

        OnPlotAreaChanged?.Invoke();
        OnPlotStorageChanged?.Invoke();
    }

    public async Task ContextPlotDelete(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot dataPlot)
    {
        await spectraDataStorageController.DeleteData(ownerSet, dataPlot);
        await spectraGraphicsController.EraseData(dataPlot);

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
        await spectraGraphicsController.ResizeArea();

        OnPlotAreaChanged?.Invoke();
    }

    private async Task ClearPlot()
    {
        await spectraDataStorageController.Clear();
        await spectraGraphicsController.ClearArea();
    }
}
