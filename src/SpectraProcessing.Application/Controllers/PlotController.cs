using System.Collections.Concurrent;
using ScottPlot.WinForms;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain.Graphics;
using SpectraProcessing.Domain.SpectraData;
using SpectraProcessing.Domain.Storage;
using SpectraProcessing.Graphics.Formats;

namespace SpectraProcessing.Application.Controllers;

public class PlotController(
    FormsPlot form,
    IDataStorageController<SpectraPlot> plotStorageController,
    IPlotBuilder<Spectra, SpectraPlot> plotBuilder,
    IGraphicsController<SpectraPlot> graphicsController,
    ISpectraProcessingController spectraProcessingController
) : IPlotController
{
    public event Action? OnChange;
    public IEnumerable<DataSet<SpectraPlot>> Plots => plotStorageController.StorageData;

    private event Action? OnPlotClear = () =>
    {
        plotStorageController.Clear();
        graphicsController.ClearArea();
        spectraProcessingController.RedrawBorders();
    };

    public async Task ContextDataSetAddToPlotArea(DataSet<Spectra> set)
    {
        await Task.Run(() =>
        {
            var plots = new ConcurrentBag<SpectraPlot>();
            Parallel.ForEach(set.Data, data =>
            {
                var plt = plotBuilder.GetPlot(data);
                plots.Add(plt);
            });
            var plotSet = new DataSet<SpectraPlot>(set.Name, plots);
            plotStorageController.AddDataSet(plotSet);
            graphicsController.DrawDataSet(plotSet);
        });
        form.Refresh();
        OnChange?.Invoke();
    }

    public async Task ContextDataAddToClearPlotArea(DataSet<Spectra> set)
    {
        OnPlotClear?.Invoke();
        await ContextDataSetAddToPlotArea(set);
    }

    public async Task DataAddToPlotAreaToDefault(Spectra spectra)
    {
        await Task.Run(() =>
        {
            var plot = plotBuilder.GetPlot(spectra);
            plotStorageController.AddDataToDefaultSet(plot);
            graphicsController.DrawData(plot);
        });
        form.Refresh();
        OnChange?.Invoke();
    }

    public async Task ContextDataAddToClearPlotToDefault(Spectra spectra)
    {
        OnPlotClear?.Invoke();
        await Task.Run(() =>
        {
            var plot = plotBuilder.GetPlot(spectra);
            plotStorageController.AddDataToDefaultSet(plot);
            graphicsController.DrawData(plot);
        });
        form.Refresh();
        OnChange?.Invoke();
    }

    public async Task ChangePlotSetVisibility(DataSet<SpectraPlot> set, bool isVisible)
    {
        await Task.Run(() => graphicsController.ChangeDataSetVisibility(set, isVisible));
        form.Refresh();
        OnChange?.Invoke();
    }

    public async Task ChangePlotVisibility(SpectraPlot plot, bool isVisible)
    {
        await Task.Run(() => graphicsController.ChangeDataVisibility(plot, isVisible));
        form.Refresh();
    }

    public async Task ContextPlotSetHighlight(DataSet<SpectraPlot> set)
    {
        await Task.Run(() => graphicsController.HighlightDataSet(set));
        form.Refresh();
    }

    public async Task PlotHighlight(SpectraPlot plot)
    {
        await Task.Run(() => graphicsController.HighlightData(plot));
        form.Refresh();
    }

    public async Task ContextPlotSetDelete(DataSet<SpectraPlot> set)
    {
        await Task.Run(() =>
        {
            plotStorageController.DeleteDataSet(set);
            graphicsController.EraseDataSet(set);
        });
        form.Refresh();
        OnChange?.Invoke();
    }

    public async Task ContextPlotDelete(DataSet<SpectraPlot> ownerSet, SpectraPlot plot)
    {
        await Task.Run(() =>
        {
            plotStorageController.DeleteData(ownerSet, plot);
            graphicsController.EraseData(plot);
        });
        form.Refresh();
        OnChange?.Invoke();
    }

    public void PlotAreaClear()
    {
        OnPlotClear?.Invoke();
        OnChange?.Invoke();
        form.Refresh();
    }

    public void PlotAreaResize()
    {
        graphicsController.ResizeArea();
        form.Refresh();
    }
}
