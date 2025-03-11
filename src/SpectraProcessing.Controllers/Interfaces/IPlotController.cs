using SpectraProcessing.Domain.SpectraData;
using SpectraProcessing.Domain.Storage;
using SpectraProcessing.Graphics.Formats;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IPlotController
{
    event Action? OnPlotAreaChanged;
    event Action? OnPlotStorageChanged;
    IReadOnlyCollection<DataSet<SpectraPlot>> Plots { get; }
    Task ContextDataSetAddToPlotArea(DataSet<Spectra> set);
    Task ContextDataAddToClearPlotArea(DataSet<Spectra> set);
    Task DataAddToPlotAreaToDefault(Spectra spectra);
    Task ContextDataAddToClearPlotToDefault(Spectra spectra);
    Task ChangePlotSetVisibility(DataSet<SpectraPlot> set, bool isVisible);
    Task ChangePlotVisibility(SpectraPlot plot, bool isVisible);
    Task ContextPlotSetHighlight(DataSet<SpectraPlot> set);
    Task PlotHighlight(SpectraPlot plot);
    Task ContextPlotSetDelete(DataSet<SpectraPlot> set);
    Task ContextPlotDelete(DataSet<SpectraPlot> ownerSet, SpectraPlot plot);
    void PlotAreaClear();
    void PlotAreaResize();
}
