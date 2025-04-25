using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface ISpectraProcessingController
{
    event Action? OnPlotAreaChanged;

    decimal CurrentWidth { get; set; }

    Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras);

    Task ClearBaseline();

    Task DrawBaseline(SpectraData spectraData);

    Task SubstractBaseline(IReadOnlyCollection<SpectraData> spectras);
}
