using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IPlotController
{
    event Action? OnPlotAreaChanged;
    event Action? OnPlotStorageChanged;
    IReadOnlyCollection<DataSet<SpectraDataPlot>> Plots { get; }
    Task ContextDataSetAddToPlotArea(DataSet<SpectraData> set);
    Task ContextDataAddToClearPlotArea(DataSet<SpectraData> set);
    Task DataAddToPlotAreaToDefault(SpectraData spectra);
    Task ContextDataAddToClearPlotToDefault(SpectraData spectra);
    Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible);
    Task ChangePlotVisibility(SpectraDataPlot dataPlot, bool isVisible);
    Task ContextPlotSetHighlight(DataSet<SpectraDataPlot> set);
    Task PlotHighlight(SpectraDataPlot dataPlot);
    Task ContextPlotSetDelete(DataSet<SpectraDataPlot> set);
    Task ContextPlotDelete(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot dataPlot);
    void PlotAreaClear();
    void PlotAreaResize();
}
