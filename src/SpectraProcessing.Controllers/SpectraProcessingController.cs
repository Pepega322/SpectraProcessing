using ScottPlot;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.PeakEstimate;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Controllers;

public class SpectraProcessingController(
    IDataPlotBuilder<PeakEstimateData, PeakEstimateDataPlot> peakEstimateDataPlotBuilder,
    IDataPlotDrawer<SpectraDataPlot> spectraDataPlotDrawer,
    IDataPlotDrawer<PeakEstimateDataPlot> peakEstimateDataPlotDrawer
) : ISpectraProcessingController
{
    private readonly IList<PeakEstimateDataPlot> peakEstimateDataPlots = [];

    private PeakEstimateDataPlot? hitPlot;

    public event Action? OnPlotAreaChanged;

    public Task<bool> TryHitPlot(Pixel pixel, float radius)
    {
        hitPlot = peakEstimateDataPlots.FirstOrDefault(p => p.TryHit(pixel, radius));

        return Task.FromResult(hitPlot is not null);
    }

    public Task<bool> TryMoveHitPlot(Point<float> to)
    {
        if (hitPlot is null)
        {
            return Task.FromResult(false);
        }

        hitPlot.TryMoveTo(to);

        OnPlotAreaChanged?.Invoke();

        return Task.FromResult(true);
    }

    public Task ReleaseHitPlot()
    {
        hitPlot?.ReleaseHit();

        hitPlot = null;

        return Task.CompletedTask;
    }

    public async Task AddPeakEstimate(PeakEstimateData estimate)
    {
        var plot = await peakEstimateDataPlotBuilder.GetPlot(estimate);

        peakEstimateDataPlots.Add(plot);

        await peakEstimateDataPlotDrawer.Draw(plot);

        OnPlotAreaChanged?.Invoke();
    }
}
