using ScottPlot;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.PeakEstimate;

namespace SpectraProcessing.Controllers.Interfaces;

public interface ISpectraProcessingController
{
    event Action? OnPlotAreaChanged;

    Task AddPeakEstimate(PeakEstimateData estimate);

    Task<bool> TryHitPlot(Pixel pixel, float radius);

    Task<bool> TryMoveHitPlot(Point<float> to);

    Task ReleaseHitPlot();
}
