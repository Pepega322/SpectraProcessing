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

    Task DataAddToPlotAreaToDefault(SpectraData data);

    Task ContextDataAddToClearPlotToDefault(SpectraData data);

    Task<bool> IsPlotVisible(SpectraDataPlot plot);

    Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible);

    Task ChangePlotVisibility(SpectraDataPlot plot, bool isVisible);

    Task ContextPlotSetHighlight(DataSet<SpectraDataPlot> set);

    Task PlotHighlight(SpectraDataPlot plot);

    Task ContextPlotSetDelete(DataSet<SpectraDataPlot> set);

    Task ContextPlotDelete(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot plot);

    Task PlotAreaClear();

    Task PlotAreaResize();
}
