using ScottPlot;
using ScottPlot.WinForms;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Graphics.DataProcessors;
using SpectraProcessing.Models.Collections.Keys;
using SpectraProcessing.Models.Peak;

namespace SpectraProcessing.Application.Controllers;

public class PeakController(
    FormsPlot plotView,
    ICoordinateProvider coordinateProvider,
    IDataStorageController<SpectraKey, PeakDataPlot> peaksStorage
) : IPeakController
{
    public event Action? OnPeakChanges;

    public Task<PeakDataPlot?> TryGetPeak()
    {
        var coordinates = new Coordinates(
            coordinateProvider.Coordinates.X,
            coordinateProvider.Coordinates.Y);

        var pixel = plotView.Plot.GetPixel(coordinates);

        var hitPeak = peaksStorage.DefaultDataSet.Data
            .Concat(peaksStorage.StorageDataSets.SelectMany(x => x.Data))
            .FirstOrDefault(p => p.TryHit(pixel, PeakDataPlotBuilder.MarkerSize));

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
