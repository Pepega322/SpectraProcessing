using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IPeakDataPlotProvider
{
    public Task<IReadOnlyList<PeakDataPlot>> GetPlots(IReadOnlyList<PeakData> data);

    public Task<IReadOnlyList<PeakDataPlot>> Draw(IReadOnlyList<PeakData> data);

    public Task<IReadOnlyList<PeakDataPlot>> Erase(IReadOnlyList<PeakData> data);

    public Task Clear();
}
