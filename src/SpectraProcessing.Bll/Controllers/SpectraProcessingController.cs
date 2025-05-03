using ScottPlot;
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
    private decimal currentWidth;
    private (SpectraData Spectra, SpectraData Baseline)? current;

    public event Action? OnPlotAreaChanged;

    public decimal CurrentWidth
    {
        get => currentWidth;
        set => ChangeWidth(value);
    }

    public async Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras)
    {
        var smoothingTasks = spectras.Select(s => Task.Run(() => s.Points.Smooth()));

        await Task.WhenAll(smoothingTasks);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task DrawBaseline(SpectraData spectraData)
    {
        var baselineY = await AirPLS.GetBaseline(spectraData.Points.Y, GetAirPlsSettings());

        var newBaseline = new SimpleSpectraData("baseline", new SpectraPoints(spectraData.Points.X, baselineY));

        if (current is not null)
        {
            await spectraDataPlotProvider.Erase([current.Value.Baseline]);
        }

        var plot = (await spectraDataPlotProvider.Draw([newBaseline])).Single();

        plot.ChangeColor(Colors.Red);

        current = (spectraData, newBaseline);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task ClearBaseline()
    {
        if (current is null)
        {
            return;
        }

        await spectraDataPlotProvider.Erase([current.Value.Baseline]);
        current = null;

        OnPlotAreaChanged?.Invoke();
    }

    public async Task SubstractBaseline(IReadOnlyCollection<SpectraData> spectras)
    {
        var settings = GetAirPlsSettings();

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

        if (current is not null && spectras.Any(s => s.Equals(current.Value.Spectra)))
        {
            await DrawBaseline(current.Value.Spectra);
        }

        OnPlotAreaChanged?.Invoke();
    }

    private AirPLSSettings GetAirPlsSettings()
    {
        var airPlsSettings = new AirPLSSettings
        {
            IterationsCount = spectraProcessingSettingsMonitor.AirPLSIterations,
            SmoothCoefficient = Math.Pow(
                10,
                1.75 * Math.Log((double) currentWidth) - 0.6),
            SmoothingTolerance = 0.001f,
        };

        return airPlsSettings;
    }

    private async void ChangeWidth(decimal newWidth)
    {
        currentWidth = newWidth;

        if (current is null)
        {
            return;
        }

        await DrawBaseline(current.Value.Spectra);
    }
}
