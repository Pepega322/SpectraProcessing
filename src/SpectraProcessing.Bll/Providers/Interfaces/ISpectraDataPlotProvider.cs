using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface ISpectraDataPlotProvider
{
    public Task<bool> IsDrew(SpectraData data);

    public Task<IReadOnlyList<SpectraDataPlot>> Draw(IReadOnlyList<SpectraData> data);

    public Task<IReadOnlyList<SpectraDataPlot>> Erase(IReadOnlyList<SpectraData> data);

    public Task PushOnTop(IReadOnlyCollection<SpectraData> data);

    public Task Resize();

    public Task Clear();
}
