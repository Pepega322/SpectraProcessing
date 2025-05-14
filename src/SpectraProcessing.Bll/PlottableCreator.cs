using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Plottables;

namespace SpectraProcessing.Bll;

internal static class PlottableCreator
{
    private static long _counter;
    private static readonly IPalette Palette = new ScottPlot.Palettes.Category20();

    private const float lineWidth = 2f;
    private const float defaultMarkerSize = 20f;

    public static Signal CreateSignal(float[] ys, float delta, Color? color = null)
    {
        return new Signal(new SignalSourceGenericArray<float>(ys, delta))
        {
            Color = color ?? GetNextColor(),
            LineWidth = lineWidth,
        };
    }

    public static SignalXY CreateSignalXY(float[] xs, float[] ys, Color? color = null)
    {
        return new SignalXY(new SignalXYSourceGenericArray<float, float>(xs, ys))
        {
            Color = color ?? GetNextColor(),
            LineWidth = lineWidth,
        };
    }

    public static FunctionPlot CreateFunction(Func<double, double> func, Color? color = null)
    {
        return new FunctionPlot(new FunctionSource(func))
        {
            LineStyle =
            {
                Color = color ?? GetNextColor(),
            },
            LineWidth = lineWidth,
        };
    }

    public static DraggableMarker CreateDraggableMarker(
        double x,
        double y,
        MarkerShape shape,
        Color? color = null,
        float markerSize = defaultMarkerSize)
    {
        return new DraggableMarker
        {
            MarkerShape = shape,
            MarkerSize = markerSize,
            Color = color ?? GetNextColor(),
            Location = new Coordinates(x, y),
        };
    }

    private static Color GetNextColor() => Palette.GetColor((int) Interlocked.Increment(ref _counter));
}
