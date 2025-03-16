using ScottPlot;
using ScottPlot.WinForms;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Application.Controllers;

public class CoordinateController(FormsPlot form) : ICoordinateController
{
    private Point<int> location;

    public Point<int> Location
    {
        get => location;
        set
        {
            location = value;
            var c = form.Plot.GetCoordinates(value.X, value.Y);
            Coordinates = new Point<float>((float) c.X, (float) c.Y);
            OnChange?.Invoke();
        }
    }

    public Pixel Pixel => new(location.X, location.Y);

    public Point<float> Coordinates { get; private set; }

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
