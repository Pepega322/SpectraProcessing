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
            coordinates = new Point<float>((float)c.X, (float)c.Y);
            OnChange?.Invoke();
        }
    }

    public event Action? OnChange;

    public async Task<Point<float>> GetCoordinateByClick()
    {
        var task = new Task<Point<float>>(() => Coordinates);
        void handler(object? o, MouseEventArgs a) => task.Start();
        form.MouseDown += handler;
        var result = await task;
        form.MouseDown -= handler;
        return result;
    }

    public async Task<Point<float>> GetCoordinateByKeyDown()
    {
        var task = new Task<Point<float>>(() => Coordinates);
        void handler(object? _, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Z) task.Start();
        }
        form.KeyDown += handler;
        var result = await task;
        form.KeyDown -= handler;
        return result;
    }
}
