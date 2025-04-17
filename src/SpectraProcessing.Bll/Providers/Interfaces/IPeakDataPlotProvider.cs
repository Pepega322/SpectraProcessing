using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IPeakDataPlotProvider
{
    public IEnumerable<PeakDataPlot> GetPlots(IEnumerable<PeakData> data);

    public IEnumerable<PeakDataPlot> Draw(IEnumerable<PeakData> data);

    public IEnumerable<PeakDataPlot> Erase(IEnumerable<PeakData> data);

    public void Clear();
}
