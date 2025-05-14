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
    public event Action? OnPeakChanges;

    public Task<PeakDataPlot?> TryGetPeak()
    {
        var hitPeak = peakProcessingController.CurrentPeaks
            .FirstOrDefault(p => p.TryHit(
                coordinateProvider.Coordinates.X,
                coordinateProvider.Coordinates.Y,
                coordinateProvider.Width / 100,
                coordinateProvider.Heigth / 100));

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
