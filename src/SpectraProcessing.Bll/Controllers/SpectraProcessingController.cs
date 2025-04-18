using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

internal sealed class SpectraProcessingController : ISpectraProcessingController
{
    public event Action? OnPlotAreaChanged;

    public async Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras)
    {
        var smoothingTasks = spectras.Select(s => Task.Run(() => s.Points.Smooth()));

        await Task.WhenAll(smoothingTasks);

        OnPlotAreaChanged?.Invoke();
    }
}
