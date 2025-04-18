using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface ISpectraProcessingController
{
    event Action? OnPlotAreaChanged;

    Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras);

    //TODO думай голова!
}
