using ScottPlot;
using ScottPlot.WinForms;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;

namespace SpectraProcessing.Application.Controllers;

internal sealed class WinformPeakController(
    FormsPlot plotView,
    ICoordinateProvider coordinateProvider,
    IPeakProcessingController peakProcessingController
) : IPeakController
{
    private Coordinates coordinates = new Coordinates(0, 0);

    public event Action? OnPeakChanges;

    public Task<PeakDataPlot?> TryGetPeak()
    {
        coordinates.X = coordinateProvider.Coordinates.X;
        coordinates.Y = coordinateProvider.Coordinates.Y;

        var pixel = plotView.Plot.GetPixel(coordinates);

        var hitPeak = peakProcessingController.CurrentPeaks
            .FirstOrDefault(p => p.TryHit(pixel, 20f));

        return Task.FromResult(hitPeak);
    }

    public async Task TryMovePeak()
    {
        var hitPeak = await TryGetPeak();

        if (hitPeak is null)
        {
            return;
        }

        plotView.Cursor = Cursors.Hand;
        plotView.UserInputProcessor.Disable();
        MouseEventHandler onMouseMove = (_, _) =>
        {
            hitPeak.TryMoveTo(coordinateProvider.Coordinates);
            OnPeakChanges?.Invoke();
        };
        plotView.MouseMove += onMouseMove;

        var tcs = new TaskCompletionSource();

        MouseEventHandler onMouseUp = (_, _) =>
        {
            plotView.MouseMove -= onMouseMove;
            hitPeak.ReleaseHit();
            plotView.UserInputProcessor.Enable();
            plotView.Cursor = Cursors.Arrow;
            OnPeakChanges?.Invoke();
            tcs.TrySetResult();
        };

        plotView.MouseUp += onMouseUp;

        await tcs.Task;

        plotView.MouseUp -= onMouseUp;
    }
}
