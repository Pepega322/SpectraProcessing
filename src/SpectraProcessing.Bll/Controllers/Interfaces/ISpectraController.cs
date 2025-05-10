using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface ISpectraController
{
    event Action? OnPlotAreaChanged;

    event Action? OnPlotStorageChanged;

    SpectraDataPlot? HighlightedData { get; }

    IReadOnlyCollection<DataSet<SpectraDataPlot>> Plots { get; }

    Task AddDataSetToPlot(DataSet<SpectraData> set);

    Task AddDataSetToClearPlot(DataSet<SpectraData> set);

    Task AddDataToPlotToDefault(SpectraData data);

    Task AddDataToClearPlotToDefault(SpectraData data);

    Task<bool> IsPlotVisible(SpectraDataPlot plot);

    Task<bool> IsPlotHighlighted(SpectraDataPlot plot);

    Task ChangePlotSetVisibility(DataSet<SpectraDataPlot> set, bool isVisible);

    Task ChangePlotVisibility(SpectraDataPlot plot, bool isVisible);

    Task ContextPlotSetHighlight(DataSet<SpectraDataPlot> set);

    Task<bool> PlotHighlight(SpectraDataPlot plot);

    Task PlotRemoveHighlight(SpectraDataPlot plot);

    Task ErasePlotSet(DataSet<SpectraDataPlot> set);

    Task ErasePlot(DataSet<SpectraDataPlot> ownerSet, SpectraDataPlot plot);

    Task PlotClear();

    Task PlotResize();
}
