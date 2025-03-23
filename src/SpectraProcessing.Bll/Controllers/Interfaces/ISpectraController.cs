using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface ISpectraController
{
    event Action? OnPlotAreaChanged;

    event Action? OnPlotStorageChanged;

    IReadOnlyCollection<DataSet<SpectraDataPlot>> Plots { get; }

    Task AddToPlotArea(DataSet<SpectraData> set);

    Task ContextDataAddToClearPlotArea(DataSet<SpectraData> set);

    Task DataAddToPlotAreaToDefault(SpectraData spectra);

    Task ContextDataAddToClearPlotToDefault(SpectraData spectra);

    Task<bool> IsPlotVisible(SpectraDataPlot dataPlot);

    Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible);

    Task ChangePlotVisibility(SpectraDataPlot dataPlot, bool isVisible);

    Task ContextPlotSetHighlight(DataSet<SpectraDataPlot> set);

    Task PlotHighlight(SpectraDataPlot dataPlot);

    Task ContextPlotSetDelete(DataSet<SpectraDataPlot> set);

    Task ContextPlotDelete(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot dataPlot);

    Task PlotAreaClear();

    Task PlotAreaResize();
}
