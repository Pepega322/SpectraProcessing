using ScottPlot;
using ScottPlot.WinForms;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections.Keys;

namespace SpectraProcessing.Application.Controllers;

internal sealed class WinformPeakController(
    FormsPlot plotView,
    ICoordinateProvider coordinateProvider,
    IDataStorageProvider<SpectraKey, PeakDataPlot> peaksStorage
) : IPeakController
{
    public event Action? OnPeakChanges;

    public Task<PeakDataPlot?> TryGetPeak()
    {
        var coordinates = new Coordinates(
            coordinateProvider.Coordinates.X,
            coordinateProvider.Coordinates.Y);

        var pixel = plotView.Plot.GetPixel(coordinates);

        var hitPeak = peaksStorage.DefaultSet.Data
            .Concat(peaksStorage.Sets.Values.SelectMany(x => x.Data))
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
        MouseEventHandler onMouseMove = (_, _) => { hitPeak.TryMoveTo(coordinateProvider.Coordinates); };
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
