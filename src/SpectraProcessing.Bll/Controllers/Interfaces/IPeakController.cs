using SpectraProcessing.Models.Peak;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IPeakController
{
    event Action? OnPeakChanges;

    Task<PeakDataPlot?> TryGetPeak();

    Task TryMovePeak();
}
