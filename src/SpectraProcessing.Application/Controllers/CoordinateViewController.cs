using ScottPlot.WinForms;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain;

namespace SpectraProcessing.Application.Controllers;

public class CoordinateController(FormsPlot form) : ICoordinateController
{
    private Point<float> coordinates = new(0f, 0f);

    public Point<float> Coordinates
    {
        get => coordinates;
        set
        {
            var c = form.Plot.GetCoordinates(value.X, value.Y);
            coordinates = new Point<float>((float) c.X, (float) c.Y);
            OnChange?.Invoke();
        }
    }

    public event Action? OnChange;

    public Task<Point<float>> GetCoordinateByClick()
    {
        var cts = new TaskCompletionSource<Point<float>>();

        form.MouseDown += Handler;

        return cts.Task;

        void Handler(object? o, MouseEventArgs a)
        {
            form.MouseDown -= Handler;
            cts.TrySetResult(Coordinates);
        }
    }

    public Task<Point<float>> GetCoordinateByKeyDown()
    {
        var cts = new TaskCompletionSource<Point<float>>();

        form.KeyDown += Handler;

        return cts.Task;

        void Handler(object? _, KeyEventArgs e)
        {
            if (e.KeyData is not Keys.Z)
            {
                return;
            }

            form.KeyDown -= Handler;
            cts.TrySetResult(Coordinates);
        }
    }
}
