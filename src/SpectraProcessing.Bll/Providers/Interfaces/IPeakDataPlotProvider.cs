using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IPeakDataPlotProvider
{
    public IReadOnlyCollection<PeakDataPlot> GetPlots(IEnumerable<PeakData> data);

    public void Draw(IEnumerable<PeakDataPlot> data);

    public void Erase(IEnumerable<PeakDataPlot> data);

    public void Clear();
}
