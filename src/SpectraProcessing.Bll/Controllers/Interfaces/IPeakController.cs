using SpectraProcessing.Bll.Models.ScottPlot.Peak;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IPeakController
{
    event Action? OnPeakChanges;

    Task<PeakDataPlot?> TryGetPeak();

    Task TryMovePeak();
}
