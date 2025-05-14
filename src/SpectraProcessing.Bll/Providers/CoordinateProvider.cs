using ScottPlot;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;

namespace SpectraProcessing.Bll.Providers;

internal sealed class CoordinateProvider(Plot plot) : ICoordinateProvider
{
    public Point<float> Coordinates { get; } = new(0, 0);

    public float Width => (float) plot.Axes.Bottom.Width;

    public float Heigth => (float) plot.Axes.Left.Height;

    public void UpdateCoordinates(int x, int y)
    {
        var c = plot.GetCoordinates(x, y);
        Coordinates.X = (float) c.X;
        Coordinates.Y = (float) c.Y;
    }
}
