using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Monitors.Interfaces;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.MathModeling.Baseline.AirPLS;
using SpectraProcessing.Domain.Models.MathModeling.Baseline;
using SpectraProcessing.Domain.Models.Spectra;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

internal sealed class SpectraProcessingController(
    ISpectraDataPlotProvider spectraDataPlotProvider,
    ISpectraProcessingSettingsMonitor spectraProcessingSettingsMonitor
) : ISpectraProcessingController
{
    private SpectraData? currentBaseline;

    public event Action? OnPlotAreaChanged;

    public async Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras)
    {
        var smoothingTasks = spectras.Select(s => Task.Run(() => s.Points.Smooth()));

        await Task.WhenAll(smoothingTasks);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task DrawBaseline(SpectraData spectraData)
    {
        var baselineY = await AirPLS.GetBaseline(spectraData.Points.Y, GetAirPLSSettings());

        var newBaseline = new SimpleSpectraData("baseline", new SpectraPoints(spectraData.Points.X, baselineY));

        if (currentBaseline is not null)
        {
            await spectraDataPlotProvider.Erase([currentBaseline]);
        }

        await spectraDataPlotProvider.Draw([newBaseline]);

        currentBaseline = newBaseline;

        OnPlotAreaChanged?.Invoke();
    }

    public async Task SubstractBaseline(IReadOnlyCollection<SpectraData> spectras)
    {
        var settings = GetAirPLSSettings();

        await Parallel.ForEachAsync(
            spectras,
            async (spectra, token) =>
            {
                var baselineY = await AirPLS.GetBaseline(spectra.Points.Y, settings);

                for (var i = 0; i < spectra.Points.Count; i++)
                {
                    spectra.Points.Y[i] -= baselineY[i];
                }
            });

        OnPlotAreaChanged?.Invoke();
    }

    private AirPLSSettings GetAirPLSSettings()
        => new()
        {
            IterationsCount = spectraProcessingSettingsMonitor.AirPLSIterations,
            SmoothCoefficient = Math.Pow(
                10,
                1.75 * Math.Log(spectraProcessingSettingsMonitor.AirPLSMaxPeaksWidth) - 0.6),
            SmoothingTolerance = 0.001f,
        };
}
