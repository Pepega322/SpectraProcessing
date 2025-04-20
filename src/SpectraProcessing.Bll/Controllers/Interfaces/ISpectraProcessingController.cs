using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface ISpectraProcessingController
{
    event Action? OnPlotAreaChanged;

    Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras);

    Task DrawBaseline(SpectraData spectraData);

    Task SubstractBaseline(IReadOnlyCollection<SpectraData> spectras);
}
