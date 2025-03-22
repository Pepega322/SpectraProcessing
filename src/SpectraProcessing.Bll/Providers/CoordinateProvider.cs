using ScottPlot;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Bll.Providers;

public sealed class CoordinateProvider(Plot plot) : ICoordinateProvider
{
    public Point<float> Coordinates { get; } = new(0, 0);

    public void UpdateCoordinates(int x, int y)
    {
        var c = plot.GetCoordinates(x, y);
        Coordinates.X = (float) c.X;
        Coordinates.Y = (float) c.Y;
    }
}
